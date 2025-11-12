using System;
using System.Collections.Generic;
using System.Linq;
using GameCoreLib;
using SpacetimeDB;
using MemoryPack;

public static partial class Module
{
    /// <summary>
    /// Main tick function - called every simulation step
    /// Order is important here
    /// </summary>
    private static void OnSeqTick(ReducerContext ctx)
    {
        StepServer(ctx);

        BaseCfg baseCfg = BaseCfg.GetSingleton(ctx);
        ushort stepsSinceLastBatch = StepsSinceLastBatch.Get(ctx).Value;

        if (stepsSinceLastBatch >= baseCfg.physicsStepsPerBatch)
        {
            OnBatchStepInterval(ctx, stepsSinceLastBatch);
        }

        ushort stepsSinceLastAuthFrame = StepsSinceLastAuthFrame.GetSingleton(ctx).Value;
        if (stepsSinceLastAuthFrame >= baseCfg.stepsPerAuthFrame)
        {
            BroadcastAuthFrame(ctx);
        }
    }

    /// <summary>
    /// Broadcasts an authoritative frame to clients
    /// </summary>
    private static void BroadcastAuthFrame(ReducerContext ctx)
    {
        BaseCfg cfg = BaseCfg.GetSingleton(ctx);
        Timestamp lastTimestamp = LastAuthFrameTimestamp.GetSingleton(ctx).LastAuthFrameTime;

        if (cfg.targetStepsPerSecond <= 0)
        {
            throw new Exception("targetStepsPerSecond must be > 0");
        }


        var elapsed = ctx.Timestamp.TimeDurationSince(lastTimestamp);


        double expectedIntervalSec = (double)cfg.stepsPerAuthFrame / (double)cfg.targetStepsPerSecond;
        double actualIntervalSec = elapsed.ToSeconds();
        double errorSec = Math.Abs(actualIntervalSec - expectedIntervalSec);

        if (cfg.logAuthFrameTimeDiffs)
        {
            Log.Info($"Auth frame interval {actualIntervalSec:F6}s (expected {expectedIntervalSec:F6}s, error {errorSec:F6}s)");
        }

        if (errorSec >= cfg.authFrameTimeErrorThresholdSec)
        {
            Log.Error($"Auth frame interval deviated by {errorSec:F6}s (actual {actualIntervalSec:F6}s, expected {expectedIntervalSec:F6}s, threshold {cfg.authFrameTimeErrorThresholdSec:F6}s). last_timestamp: {lastTimestamp}, ctx.timestamp: {ctx.Timestamp}");
        }


        LastAuthFrameTimestamp.Set(ctx, ctx.Timestamp);

        // Collect and delete all the pending input frames and insert them into a single auth frame
        var inputFrames = ctx.Db.InputFrame.Iter().ToList();
        // TODO: Implement proper serialization of input frames
        var authFrame = new AuthFrame
        {
            Seq = Seq.GetSingleton(ctx).Value,
            Frames = inputFrames
        };

        ctx.Db.AuthFrame.Seq.Delete(authFrame.Seq);
        ctx.Db.AuthFrame.Insert(authFrame);

        // Delete any that are older than the public snapshot seq
        var publicSnapshot = ctx.Db.GameCoreSnap.Id.Find(0);
        if (publicSnapshot.HasValue)
        {
            var allAuthFrames = ctx.Db.AuthFrame.Iter().ToList();
            foreach (var frame in allAuthFrames)
            {
                ushort threshold = publicSnapshot.Value.Seq.WrappingSub((ushort)(cfg.physicsStepsPerBatch * 2));
                if (frame.Seq.IsBehind(threshold))
                {
                    ctx.Db.AuthFrame.Seq.Delete(frame.Seq);
                }
            }
        }

        StepsSinceLastAuthFrame.Set(ctx, 0);
    }

    /// <summary>
    /// Runs a batch simulation when enough steps have accumulated
    /// </summary>
    private static void OnBatchStepInterval(ReducerContext ctx, ushort stepsSinceLastBatch)
    {
        ushort seq = Seq.GetSingleton(ctx).Value;
        ushort batchStartSeq = seq.WrappingSub(stepsSinceLastBatch);
        Log.Info($"Running batch sim from [{batchStartSeq}-{seq.WrappingAdd(stepsSinceLastBatch)}]");

        // Get or create the most recent snapshot
        var snapshotOpt = ctx.Db.GameCoreSnap.Id.Find(0);
        GameCoreSnap snapshot;

        if (snapshotOpt.HasValue)
        {
            snapshot = snapshotOpt.Value;
        }
        else
        {
            snapshot = new GameCoreSnap
            {
                Id = 0,
                Seq = batchStartSeq,
                BinaryData = MemoryPackSerializer.Serialize(new GameCore())
            };
        }

        if (snapshot.Seq != batchStartSeq)
        {
            Log.Warn($"snapshot seq [{snapshot.Seq}] did not match expected batch start [{batchStartSeq}]; resetting counters");
            Seq.Set(ctx, snapshot.Seq);
            StepsSinceLastBatch.Set(ctx, 0);
            StepsSinceLastAuthFrame.Set(ctx, 0);
            return;
        }

        // Step it by the batch number of physics steps
        GameCore? core = MemoryPackSerializer.Deserialize<GameCore>(snapshot.BinaryData);
        if (core == null)
        {
            Log.Error("Failed to deserialize GameCore");
            return;
        }

        // TODO: Implement proper deserialization and processing of input events
        var batchEvents = new List<byte[]>();
        for (ushort s = 0; s < stepsSinceLastBatch; s++)
        {
            var simSeq = batchStartSeq.WrappingAdd(s);
            var inputFrameOpt = ctx.Db.InputFrame.Seq.Find(simSeq);

            byte[] events;
            if (inputFrameOpt.HasValue)
            {
                events = inputFrameOpt.Value.InputEventsList;
            }
            else
            {
                events = Array.Empty<byte>();
            }

            batchEvents.Add(events);
        }

        var outputEvents = BatchStepGameCore(batchEvents);
        ProcessOutputEvents(ctx, outputEvents);

        // Verify seq consistency
        var gameManagerSeq = core.Seq;
        var currentSeq = Seq.GetSingleton(ctx).Value;
        if (gameManagerSeq != currentSeq)
        {
            Log.Error($"GameManager seq {gameManagerSeq} does not match current seq {currentSeq}");
        }

        // Serialize the simulated snap back into a row
        var simulatedSnap = new GameCoreSnap
        {
            Id = 0,
            Seq = core.Seq,
            BinaryData = MemoryPackSerializer.Serialize(core)
        };

        ctx.Db.GameCoreSnap.Id.Delete(0);
        ctx.Db.GameCoreSnap.Insert(simulatedSnap);

        StepsSinceLastBatch.Set(ctx, 0);
    }

    /// <summary>
    /// Processes output events from the game simulation
    /// </summary>
    private static void ProcessOutputEvents(ReducerContext ctx, List<OutputToSTDB> outputEvents)
    {
        foreach (var outputEvent in outputEvents)
        {
            Log.Info($"Processing output event in server: {outputEvent.EventType}");

            switch (outputEvent.EventType)
            {
                case OutputToSTDBEventType.AddPointsToAccount:
                    {
                        var accountOpt = ctx.Db.Account.Id.Find(outputEvent.AccountId);
                        if (!accountOpt.HasValue)
                        {
                            throw new Exception($"Account {outputEvent.AccountId} not found");
                        }

                        var account = accountOpt.Value;
                        account.Points = account.Points.SaturatingAdd(outputEvent.Points);
                        ctx.Db.Account.Id.Update(account);
                        break;
                    }

                case OutputToSTDBEventType.NewKing:
                    // Handle new king event
                    break;

                case OutputToSTDBEventType.DeterminismHash:
                    {
                        Log.Info($"Determinism hash: {outputEvent.HashString}");
                        ctx.Db.DeterminismCheck.Id.Delete(0);
                        ctx.Db.DeterminismCheck.Insert(new DeterminismCheck
                        {
                            Id = 0,
                            Seq = outputEvent.Seq,
                            HashString = outputEvent.HashString
                        });
                        break;
                    }

                case OutputToSTDBEventType.GameTileFinished:
                    {
                        CloseAndCycleGameTile(ctx, outputEvent.WorldId);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// Insert an input frame no matter what
    /// </summary>
    private static void StepServer(ReducerContext ctx)
    {
        Seq.Step(ctx);

        var cfg = BaseCfg.GetSingleton(ctx);

        // Move all the collected inputs into the public input chain table
        var rows = ctx.Db.InputCollector.Iter().ToList();

        var eventsList = new List<InputEvent>();
        foreach (InputCollector row in rows)
        {
            ctx.Db.InputCollector.Delete(row);

            if (row.delaySeqs <= 0)
            {
                InputEvent? inputEvent = MemoryPackSerializer.Deserialize<InputEvent>(row.inputEventData);
                if (inputEvent != null)
                {
                    eventsList.Add(inputEvent);
                }

            }
            else
            {
                ctx.Db.InputCollector.Insert(new InputCollector
                {
                    delaySeqs = (ushort)Math.Max(0, row.delaySeqs - 1),
                    inputEventData = row.inputEventData
                });
            }
        }

        // Serialize all events into a single byte array
        byte[] eventsListData = MemoryPackSerializer.Serialize(eventsList);

        var newInputFrame = new InputFrame
        {
            Seq = Seq.GetSingleton(ctx).Value,
            InputEventsList = eventsListData
        };

        if (cfg.logInputFrameTimes)
        {
            Log.Info($"Input frame inserted: [{newInputFrame.Seq}] at time {ctx.Timestamp}");
        }

        if (eventsList.Count > 0)
        {
            Log.Info($"[{newInputFrame.Seq}] Inserted input_frame");
        }

        ctx.Db.InputFrame.Seq.Delete(newInputFrame.Seq);
        ctx.Db.InputFrame.Insert(newInputFrame);

        // Delete any that are older than the latest snapshot seq
        var publicSnapshotOpt = ctx.Db.GameCoreSnap.Id.Find(0);
        if (publicSnapshotOpt.HasValue)
        {
            var publicSnapshot = publicSnapshotOpt.Value;
            var allInputFrames = ctx.Db.InputFrame.Iter().ToList();

            foreach (var frame in allInputFrames)
            {
                if (frame.Seq.IsBehind(publicSnapshot.Seq))
                {
                    ctx.Db.InputFrame.Seq.Delete(frame.Seq);
                }
            }
        }

        ushort stepsSinceLastBatch = StepsSinceLastBatch.Get(ctx).Value;
        ushort stepsSinceLastAuthFrame = StepsSinceLastAuthFrame.GetSingleton(ctx).Value;
        StepsSinceLastBatch.Set(ctx, stepsSinceLastBatch.WrappingAdd(1));
        StepsSinceLastAuthFrame.Set(ctx, stepsSinceLastAuthFrame.WrappingAdd(1));
    }




    private static List<OutputToSTDB> BatchStepGameCore(List<byte[]> batchEvents)
    {
        //Deserialize the GameCore from the snapshot singleton

        // TODO: Implement batch stepping through game simulation
        return new List<OutputToSTDB>();
    }


    // Placeholder for game tile operations
    private static void CloseAndCycleGameTile(ReducerContext ctx, byte worldId)
    {
        // TODO: Implement game tile closing and cycling logic
        Log.Info($"Closing and cycling game tile for world {worldId}");
    }
}

// Placeholder structs - these should be properly defined based on your game logic
public struct OutputToSTDB
{
    public OutputToSTDBEventType EventType;
    public ulong AccountId;
    public uint Points;
    public ushort Seq;
    public string HashString;
    public byte WorldId;
}

public enum OutputToSTDBEventType
{
    AddPointsToAccount,
    NewKing,
    DeterminismHash,
    GameTileFinished
}
