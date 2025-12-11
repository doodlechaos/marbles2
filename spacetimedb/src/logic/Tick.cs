using System;
using System.Collections.Generic;
using System.Linq;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

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
        ushort stepsSinceLastBatch = StepsSinceLastBatch.Get(ctx);

        if (stepsSinceLastBatch >= baseCfg.physicsStepsPerBatch)
        {
            OnBatchStepInterval(ctx, stepsSinceLastBatch);
        }

        ushort stepsSinceLastAuthFrame = StepsSinceLastAuthFrame.Get(ctx);
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

        double expectedIntervalSec =
            (double)cfg.stepsPerAuthFrame / (double)cfg.targetStepsPerSecond;
        double actualIntervalSec = elapsed.ToSeconds();
        double errorSec = Math.Abs(actualIntervalSec - expectedIntervalSec);

        if (cfg.logAuthFrameTimeDiffs)
        {
            Log.Info(
                $"Auth frame interval {actualIntervalSec:F6}s (expected {expectedIntervalSec:F6}s, error {errorSec:F6}s)"
            );
        }

        if (errorSec >= cfg.authFrameTimeErrorThresholdSec)
        {
            Log.Warn(
                $"Auth frame interval deviated by {errorSec:F6}s (actual {actualIntervalSec:F6}s, expected {expectedIntervalSec:F6}s, threshold {cfg.authFrameTimeErrorThresholdSec:F6}s). last_timestamp: {lastTimestamp}, ctx.timestamp: {ctx.Timestamp}"
            );
        }

        LastAuthFrameTimestamp.Set(ctx, ctx.Timestamp);

        // Collect and delete all the pending input frames and insert them into a single auth frame
        var inputFrames = ctx.Db.InputFrame.Iter().ToList();

        var authFrame = new AuthFrame { Seq = Seq.Get(ctx), Frames = inputFrames };

        ctx.Db.AuthFrame.Seq.Delete(authFrame.Seq);
        ctx.Db.AuthFrame.Insert(authFrame);

        // Delete any that are older than the public snapshot seq
        var publicSnapshot = ctx.Db.GameCoreSnap.Id.Find(0);
        if (publicSnapshot.HasValue)
        {
            var allAuthFrames = ctx.Db.AuthFrame.Iter().ToList();
            foreach (var frame in allAuthFrames)
            {
                ushort threshold = publicSnapshot.Value.Seq.WrappingSub(
                    (ushort)(cfg.physicsStepsPerBatch * 2)
                );
                if (frame.Seq.IsBehind(threshold))
                {
                    ctx.Db.AuthFrame.Seq.Delete(frame.Seq);
                }
            }
        }

        StepsSinceLastAuthFrame.Clear(ctx);
    }

    /// <summary>
    /// Runs a batch simulation when enough steps have accumulated
    /// </summary>
    private static void OnBatchStepInterval(ReducerContext ctx, ushort stepsSinceLastBatch)
    {
        ushort seq = Seq.Get(ctx);
        ushort batchStartSeq = seq.WrappingSub(stepsSinceLastBatch);
        Log.Info(
            $"Running batch sim from [{batchStartSeq}-{batchStartSeq.WrappingAdd(stepsSinceLastBatch)}]"
        );

        // Get or create the most recent snapshot
        var snapshotOpt = ctx.Db.GameCoreSnap.Id.Find(0);
        GameCoreSnap snapshot;

        if (snapshotOpt.HasValue)
        {
            snapshot = snapshotOpt.Value;
        }
        else
        {
            GameCore gameCore = new GameCore();

            // Get random GameTile templates and initialize them with their slot IDs
            gameCore.GameTile1 = GetRandomGameTile(ctx);
            gameCore.GameTile1.Initialize(1);

            gameCore.GameTile2 = GetRandomGameTile(ctx);
            gameCore.GameTile2.Initialize(2);

            snapshot = new GameCoreSnap
            {
                Id = 0,
                Seq = batchStartSeq,
                BinaryData = MemoryPackSerializer.Serialize(gameCore),
            };
        }

        if (snapshot.Seq != batchStartSeq)
        {
            Log.Warn(
                $"snapshot seq [{snapshot.Seq}] did not match expected batch start [{batchStartSeq}]; resetting counters"
            );
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

        //It's possible we could optimize this in the future by just doing one deserialization for the list of list of inputs events instead of one for each input frame
        for (ushort s = 0; s < stepsSinceLastBatch; s++)
        {
            var simSeq = batchStartSeq.WrappingAdd(s);
            var inputFrameOpt = ctx.Db.InputFrame.Seq.Find(simSeq);

            List<InputEvent> inputEvents;
            if (inputFrameOpt.HasValue)
            {
                inputEvents =
                    MemoryPackSerializer.Deserialize<List<InputEvent>>(
                        inputFrameOpt.Value.InputEventsList.ToArray()
                    ) ?? new List<InputEvent>();
            }
            else
            {
                inputEvents = new List<InputEvent>();
            }
            OutputEventBuffer outputEvents = core.Step(inputEvents);
            ProcessOutputEvents(ctx, outputEvents);
        }

        // Verify seq consistency
        var gameManagerSeq = core.Seq;
        var currentSeq = Seq.Get(ctx);
        if (gameManagerSeq != currentSeq)
        {
            Log.Error($"GameManager seq {gameManagerSeq} does not match current seq {currentSeq}");
        }

        // Serialize the simulated snap back into a row
        var simulatedSnap = new GameCoreSnap
        {
            Id = 0,
            Seq = core.Seq,
            BinaryData = MemoryPackSerializer.Serialize(core),
        };

        ctx.Db.GameCoreSnap.Id.Delete(0);
        ctx.Db.GameCoreSnap.Insert(simulatedSnap);

        StepsSinceLastBatch.Set(ctx, 0);
    }

    private static void ProcessOutputEvents(ReducerContext ctx, OutputEventBuffer outputEvents)
    {
        foreach (OutputToServerEvent outputToServerEvent in outputEvents.Server)
        {
            Log.Info($"Processing output event in server: {outputToServerEvent.GetType().Name}");
            if (outputToServerEvent is OutputToServerEvent.StateUpdatedTo stateUpdatedTo)
            {
                ProcessStateUpdate(ctx, stateUpdatedTo);
            }
            else if (outputToServerEvent is OutputToServerEvent.NewKing newKing)
            {
                Throne throne = Throne.Inst(ctx);
                throne.KingAccountId = newKing.AccountId;
                ctx.Db.Throne.Id.Update(throne);
                Log.Info($"New king crowned: {newKing.AccountId}");
            }
            else if (outputToServerEvent is OutputToServerEvent.DeterminismHash determinismHash)
            {
                DeterminismSnapS determinismSnap = DeterminismSnapS.Inst(ctx);
                determinismSnap.HashString = determinismHash.HashString;
                determinismSnap.Seq = determinismHash.Seq;
                ctx.Db.DeterminismSnapS.Id.Update(determinismSnap);
                Log.Info(
                    $"DeterminismHash: [{determinismHash.HashString}] at seq [{determinismHash.Seq}]"
                );
            }
        }
    }

    private static void ProcessStateUpdate(
        ReducerContext ctx,
        OutputToServerEvent.StateUpdatedTo stateUpdatedTo
    )
    {
        byte worldId = stateUpdatedTo.WorldId;
        GameTileState state = stateUpdatedTo.State;

        if (state == GameTileState.Finished)
        {
            Log.Info($"Detected Tile {worldId} finished - spinning to a new level");
            // Tile finished - spin to a new level
            ctx.Db.InputCollector.Insert(
                new InputCollector
                {
                    delaySeqs = 0,
                    inputEventData = new InputEvent.SpinToNewGameTile(
                        GetRandomGameTile(ctx),
                        worldId
                    ).ToBinary(),
                }
            );
        }
        else if (state == GameTileState.Bidding)
        {
            Log.Info($"Detected Tile {worldId} entered Bidding state");
            // A tile just entered Bidding state.
            // Signal that the OTHER tile (currently in bidding) can now start gameplay.
            BiddingStateS biddingState = BiddingStateS.Inst(ctx);

            // Only set the flag if this is the "other" tile (not the current bidding tile)
            // This means: the tile that was in Gameplay/ScoreScreen/Spinning has now reached Bidding
            if (worldId != biddingState.CurrBidWorldId)
            {
                biddingState.OtherTileReadyForBidding = true;
                ctx.Db.BiddingStateS.Id.Update(biddingState);
                Log.Info(
                    $"Tile {worldId} entered Bidding - other tile ready flag set. Current bidding tile: {biddingState.CurrBidWorldId}"
                );
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

        List<InputEvent> eventsList = new List<InputEvent>();
        foreach (InputCollector row in rows)
        {
            ctx.Db.InputCollector.Delete(row);

            if (row.delaySeqs <= 0)
            {
                try
                {
                    Log.Info($"Deserializing InputEvent from {row.inputEventData.Length} bytes");
                    InputEvent? inputEvent = MemoryPackSerializer.Deserialize<InputEvent>(
                        row.inputEventData
                    );

                    if (inputEvent != null)
                    {
                        eventsList.Add(inputEvent);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(
                        $"Failed to deserialize InputEvent: {e.Message}. Skipping this input."
                    );
                }
            }
            else
            {
                ctx.Db.InputCollector.Insert(
                    new InputCollector
                    {
                        delaySeqs = (ushort)Math.Max(0, row.delaySeqs - 1),
                        inputEventData = row.inputEventData,
                    }
                );
            }
        }

        // Serialize all events into a single byte array
        byte[] eventsListData = InputEventSerialization.SerializeList(eventsList);

        var newInputFrame = new InputFrame { Seq = Seq.Get(ctx), InputEventsList = eventsListData };

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

        StepsSinceLastBatch.Inc(ctx);
        StepsSinceLastAuthFrame.Inc(ctx);
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
    GameTileFinished,
}
