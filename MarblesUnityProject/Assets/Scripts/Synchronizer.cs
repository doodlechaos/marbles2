using System;
using System.Collections.Generic;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class Synchronizer : MonoBehaviour
{
    [SerializeField]
    private ushort safeSeqEdge;

    [SerializeField]
    private ushort latestServerSeq;

    [SerializeField]
    private bool latestServerSeqInitialized = false;

    [SerializeField]
    private ushort clientTargetSeq;

    [SerializeField]
    private short targetToSafeEdgeDist;

    [SerializeField]
    private bool restoreRequestedFlag = false;

    public void SetCallbacks(DbConnection conn)
    {
        // Reset state for fresh sync
        safeSeqEdge = 0;
        latestServerSeq = 0;
        latestServerSeqInitialized = false;
        clientTargetSeq = 0;
        targetToSafeEdgeDist = 0;
        restoreRequestedFlag = false;

        conn.Db.GameCoreSnap.OnInsert += OnGameCoreSnapInsert;
        conn.Db.GameCoreSnap.OnUpdate += OnGameCoreSnapUpdate;
        conn.Db.AuthFrame.OnInsert += OnAuthFrameInsert;

        Debug.Log("[Synchronizer] Callbacks registered");
    }

    public void CleanupCallbacks(DbConnection conn)
    {
        conn.Db.GameCoreSnap.OnInsert -= OnGameCoreSnapInsert;
        conn.Db.GameCoreSnap.OnUpdate -= OnGameCoreSnapUpdate;
        conn.Db.AuthFrame.OnInsert -= OnAuthFrameInsert;
        Debug.Log("[Synchronizer] Cleaned up callbacks");
    }

    private void OnGameCoreSnapInsert(EventContext ctx, GameCoreSnap row)
    {
        ApplySnapshot(row.BinaryData.ToArray());
    }

    private void OnGameCoreSnapUpdate(EventContext ctx, GameCoreSnap oldRow, GameCoreSnap newRow)
    {
        ApplySnapshot(newRow.BinaryData.ToArray());
    }

    private void OnAuthFrameInsert(EventContext ctx, AuthFrame row)
    {
        foreach (InputFrame inputFrame in row.Frames)
        {
            if (!latestServerSeqInitialized)
            {
                // First frame establishes our baseline. We have to do this, otherwise the "IsAhead" won't bootstrap correctly.
                latestServerSeq = inputFrame.Seq;
                latestServerSeqInitialized = true;
            }
            else if (inputFrame.Seq.IsAhead(latestServerSeq))
            {
                // Normal wrap-aware “max” update.
                latestServerSeq = inputFrame.Seq;
            }
        }
    }

    private void OnDestroy()
    {
        if (STDB.Conn != null)
            CleanupCallbacks(STDB.Conn);
    }

    private void ApplySnapshot(byte[] gameCoreData)
    {
        if (!restoreRequestedFlag)
            return;

        GameManager.Inst.GameCore = MemoryPackSerializer.Deserialize<GameCore>(gameCoreData);
        safeSeqEdge = GameManager.Inst.GameCore.Seq;
        restoreRequestedFlag = false;

        // Debug logging for deserialized state
        var gc = GameManager.Inst.GameCore;
        Debug.Log(
            $"RESTORED FROM SNAPSHOT TO SEQ: {gc.Seq}\n"
                + $"  ThroneTile: {(gc.ThroneTile != null ? "present" : "null")}, "
                + $"TileRoot.ChildCount={gc.ThroneTile?.TileRoot?.Children?.Count ?? 0}\n"
                + $"  GameTile1: {(gc.GameTile1 != null ? gc.GameTile1.GetType().Name : "null")}, "
                + $"TileRoot.ChildCount={gc.GameTile1?.TileRoot?.Children?.Count ?? 0}\n"
                + $"  GameTile2: {(gc.GameTile2 != null ? gc.GameTile2.GetType().Name : "null")}, "
                + $"TileRoot.ChildCount={gc.GameTile2?.TileRoot?.Children?.Count ?? 0}"
        );
    }

    private void FixedUpdate()
    {
        if (STDB.Conn == null)
            return;
        if (!STDB.Conn.IsActive)
            return;

        if (!EnsureClientHasntFallenTooFarBehind())
            return;

        RefreshSafeSeqEdge();

        clientTargetSeq = clientTargetSeq.LerpTo(safeSeqEdge, 0.1f);
        targetToSafeEdgeDist = clientTargetSeq.ClosestDiffTo(safeSeqEdge);

        while (GameManager.Inst.GameCore.Seq.IsBehind(clientTargetSeq))
        {
            InputFrame inputFrame = FindInputFrame(GameManager.Inst.GameCore.Seq);
            if (inputFrame == null)
            {
                Debug.LogWarning("No input frame found for seq " + GameManager.Inst.GameCore.Seq);
                break;
            }

            List<InputEvent> inputEvents = MemoryPackSerializer.Deserialize<List<InputEvent>>(
                inputFrame.InputEventsList.ToArray()
            );

            OutputEventBuffer outputEvents = GameManager.Inst.GameCore.Step(inputEvents);
            HandleGCClientOutputEvents(outputEvents);
        }
    }

    private void HandleGCClientOutputEvents(OutputEventBuffer outputEvents)
    {
        foreach (OutputToClientEvent outputToClientEvent in outputEvents.Client)
        {
            Debug.Log("OutputToClientEvent: " + outputToClientEvent.GetType().Name);
            if (outputToClientEvent is OutputToClientEvent.DeterminismHash determinismHash)
            {
                Debug.Log(
                    $"DeterminismHash: [{determinismHash.HashString}] at seq [{determinismHash.Seq}]"
                );
            }
        }
    }

    private void RefreshSafeSeqEdge()
    {
        while (true)
        {
            ushort nextSeq = safeSeqEdge.WrappingAdd(1);
            if (FindInputFrame(nextSeq) == null)
            {
                break;
            }
            safeSeqEdge = nextSeq;
        }
    }

    private InputFrame FindInputFrame(ushort seq)
    {
        foreach (AuthFrame authFrame in STDB.Conn.Db.AuthFrame.Iter())
        {
            foreach (InputFrame inputFrame in authFrame.Frames)
            {
                if (inputFrame.Seq == seq)
                    return inputFrame;
            }
        }
        return null;
    }

    private bool EnsureClientHasntFallenTooFarBehind()
    {
        // If we don't know the server's seq yet, we can't sensibly compare;
        // either:
        //   - just allow stepping, or
        //   - trigger an initial restore exactly once.
        if (!latestServerSeqInitialized)
        {
            if (!restoreRequestedFlag)
            {
                Debug.Log("[Synchronizer] No latestServerSeq yet, requesting initial restore");
                RequestRestore();
            }
            return false; // wait for snapshot before stepping
        }

        ushort? oldestSeq = GetOldestSeq();

        if (oldestSeq == null)
        {
            Debug.LogError("No oldest seq found!");
            return true;
        }

        var diff = Math.Abs(GameManager.Inst.GameCore.Seq.ClosestDiffTo(latestServerSeq));

        if (
            diff > 120
            || GameManager.Inst.GameCore.Seq.IsBehind(oldestSeq.Value)
            || GameManager.Inst.GameCore.Seq.IsAhead(latestServerSeq)
        )
        {
            if (!restoreRequestedFlag)
            {
                RequestRestore();
                return false;
            }
        }

        return true;
    }

    private void RequestRestore()
    {
        Debug.LogWarning("Client has fallen too far behind, requesting restore");
        restoreRequestedFlag = true;
        STDB.Conn.SubscriptionBuilder()
            .OnError(
                (ErrorContext ctx, Exception ex) =>
                {
                    Debug.LogError($"Subscription error: {ex}");
                }
            )
            .OnApplied(
                (SubscriptionEventContext ctx) =>
                {
                    Debug.Log("Subscription applied!");
                }
            )
            .Subscribe(new string[] { "SELECT * FROM GameCoreSnap" });
    }

    private ushort? GetOldestSeq()
    {
        ushort? oldestSeq = null;
        foreach (AuthFrame authFrame in STDB.Conn.Db.AuthFrame.Iter())
        {
            foreach (InputFrame inputFrame in authFrame.Frames)
            {
                if (oldestSeq == null || inputFrame.Seq.IsBehind(oldestSeq.Value))
                {
                    oldestSeq = inputFrame.Seq;
                }
                else
                {
                    /*                     Debug.Log(
                                            "AuthFrame inserted! Skipping input frame "
                                                + inputFrame.Seq
                                                + " because it's ahead of the oldest seq "
                                                + oldestSeq.Value
                                        ); */
                }
            }
        }
        oldestSeq = oldestSeq ?? GameManager.Inst.GameCore.Seq;
        return oldestSeq;
    }
}
