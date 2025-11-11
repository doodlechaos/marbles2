using SpacetimeDB;
using System;
using System.Collections.Generic;
using System.Linq;

public static partial class Module
{
    /// <summary>
    /// Main tick function - called every simulation step
    /// Order is important here
    /// </summary>
    private static void OnSeqTick(ReducerContext ctx)
    {
        StepServer(ctx);

        var baseCfg = GetBaseCfg(ctx);
        var stepsSinceLastBatch = GetStepsSinceLastBatch(ctx);

        if (stepsSinceLastBatch >= baseCfg.physicsStepsPerBatch)
        {
            OnBatchStepInterval(ctx, stepsSinceLastBatch);
        }

        var stepsSinceLastAuthFrame = GetStepsSinceLastAuthFrame(ctx);
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
        var cfg = GetBaseCfg(ctx);
        var lastTimestamp = GetLastAuthFrameTimestamp(ctx);

        if (cfg.targetStepsPerSecond <= 0)
        {
            throw new Exception("targetStepsPerSecond must be > 0");
        }

        if (lastTimestamp.HasValue)
        {
            var elapsed = ctx.Timestamp.TimeDurationSince(lastTimestamp.Value);
            

            double expectedIntervalSec = (double)cfg.stepsPerAuthFrame / (double)cfg.targetStepsPerSecond;
            double actualIntervalSec = elapsed.Microseconds / 1000000.0;
            double errorSec = Math.Abs(actualIntervalSec - expectedIntervalSec);

            if (cfg.logAuthFrameTimeDiffs)
            {
                Log.Info($"Auth frame interval {actualIntervalSec:F6}s (expected {expectedIntervalSec:F6}s, error {errorSec:F6}s)");
            }

            if (errorSec >= cfg.authFrameTimeErrorThresholdSec)
            {
                Log.Error($"Auth frame interval deviated by {errorSec:F6}s (actual {actualIntervalSec:F6}s, expected {expectedIntervalSec:F6}s, threshold {cfg.authFrameTimeErrorThresholdSec:F6}s). last_timestamp: {lastTimestamp.Value}, ctx.timestamp: {ctx.Timestamp}");
            }
        }
        

        SetLastAuthFrameTimestamp(ctx, ctx.Timestamp);

        // Collect and delete all the pending input frames and insert them into a single auth frame
        var inputFrames = ctx.Db.InputFrame.Iter().ToList();
        // TODO: Implement proper serialization of input frames
        var authFrame = new AuthFrame
        {
            Seq = GetSeq(ctx),
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
                var threshold = WrappingSub(publicSnapshot.Value.Seq, (ushort)(cfg.physicsStepsPerBatch * 2));
                if (IsBehind(frame.Seq, threshold))
                {
                    ctx.Db.AuthFrame.Seq.Delete(frame.Seq);
                }
            }
        }

        SetStepsSinceLastAuthFrame(ctx, 0);
    }

    /// <summary>
    /// Runs a batch simulation when enough steps have accumulated
    /// </summary>
    private static void OnBatchStepInterval(ReducerContext ctx, ushort stepsSinceLastBatch)
    {
        var batchStartSeq = WrappingSub(GetSeq(ctx), stepsSinceLastBatch);
        Log.Info($"Running batch sim from [{batchStartSeq}-{WrappingAdd(batchStartSeq, stepsSinceLastBatch)}]");

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
                BinaryData = SerializeGameManager()
            };
        }

        if (snapshot.Seq != batchStartSeq)
        {
            Log.Warn($"snapshot seq [{snapshot.Seq}] did not match expected batch start [{batchStartSeq}]; resetting counters");
            SetSeq(ctx, snapshot.Seq);
            SetStepsSinceLastBatch(ctx, 0);
            SetStepsSinceLastAuthFrame(ctx, 0);
            return;
        }

        // Step it by the batch number of physics steps
        DeserializeGameManager(snapshot.BinaryData);

        // TODO: Implement proper deserialization and processing of input events
        var batchEvents = new List<byte[]>();
        for (ushort s = 0; s < stepsSinceLastBatch; s++)
        {
            var simSeq = WrappingAdd(batchStartSeq, s);
            var inputFrameOpt = ctx.Db.InputFrame.Seq.Find(simSeq);
            
            byte[] events;
            if (inputFrameOpt.HasValue)
            {
                events = inputFrameOpt.Value.InputEvents;
            }
            else
            {
                events = Array.Empty<byte>();
            }
            
            batchEvents.Add(events);
        }

        var outputEvents = BatchStepGameManager(batchEvents);
        ProcessOutputEvents(ctx, outputEvents);

        // Verify seq consistency
        var gameManagerSeq = GetGameManagerSeq();
        var currentSeq = GetSeq(ctx);
        if (gameManagerSeq != currentSeq)
        {
            Log.Error($"GameManager seq {gameManagerSeq} does not match current seq {currentSeq}");
        }

        // Serialize the simulated snap back into a row
        var simulatedSnap = new GameCoreSnap
        {
            Id = 0,
            Seq = GetGameManagerSeq(),
            BinaryData = SerializeGameManager()
        };

        ctx.Db.GameCoreSnap.Id.Delete(0);
        ctx.Db.GameCoreSnap.Insert(simulatedSnap);

        SetStepsSinceLastBatch(ctx, 0);
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
                        account.Points = SaturatingAdd(account.Points, outputEvent.Points);
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
        StepSeq(ctx);

        var cfg = GetBaseCfg(ctx);
        
        // Move all the collected inputs into the public input chain table
        var rows = ctx.Db.InputCollector.Iter().ToList();

        // TODO: Implement proper serialization of input events
        var eventsList = new List<byte[]>();
        foreach (var row in rows)
        {
            ctx.Db.InputCollector.Delete(row);
            
            if (row.delaySeqs <= 0)
            {
                eventsList.Add(row.inputEvent);
            }
            else
            {
                ctx.Db.InputCollector.Insert(new InputCollector
                {
                    delaySeqs = (ushort)Math.Max(0, row.delaySeqs - 1),
                    inputEvent = row.inputEvent
                });
            }
        }

        // Serialize all events into a single byte array
        var serializedEvents = SerializeEventsList(eventsList);
        
        var newInputFrame = new InputFrame
        {
            Seq = GetSeq(ctx),
            InputEvents = serializedEvents
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
                if (IsBehind(frame.Seq, publicSnapshot.Seq))
                {
                    ctx.Db.InputFrame.Seq.Delete(frame.Seq);
                }
            }
        }

        SetStepsSinceLastBatch(ctx, SaturatingAdd(GetStepsSinceLastBatch(ctx), 1));
        SetStepsSinceLastAuthFrame(ctx, SaturatingAdd(GetStepsSinceLastAuthFrame(ctx), 1));
    }

    // Placeholder for game manager operations
    // These should be implemented based on your actual game logic
    private static byte[] SerializeGameManager()
    {
        // TODO: Implement actual serialization from GameManager
        return Array.Empty<byte>();
    }

    private static void DeserializeGameManager(byte[] data)
    {
        // TODO: Implement actual deserialization to GameManager
    }

    private static ushort GetGameManagerSeq()
    {
        // TODO: Get the current sequence number from GameManager
        return 0;
    }

    private static List<OutputToSTDB> BatchStepGameManager(List<byte[]> batchEvents)
    {
        // TODO: Implement batch stepping through game simulation
        return new List<OutputToSTDB>();
    }

    private static byte[] SerializeEventsList(List<byte[]> events)
    {
        // TODO: Implement proper serialization
        // For now, just concatenate or return empty
        if (events.Count == 0)
            return Array.Empty<byte>();
        
        // Simple concatenation (replace with proper serialization)
        var totalLength = events.Sum(e => e.Length);
        var result = new byte[totalLength];
        var offset = 0;
        foreach (var evt in events)
        {
            Array.Copy(evt, 0, result, offset, evt.Length);
            offset += evt.Length;
        }
        return result;
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
