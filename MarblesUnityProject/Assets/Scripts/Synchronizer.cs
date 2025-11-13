using System;
using System.Collections.Generic;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB.Types;
using UnityEngine;

public class Synchronizer : MonoBehaviour
{
    [SerializeField]
    private ushort safeSeqEdge;

    [SerializeField]
    private ushort latestServerSeq;

    [SerializeField]
    private ushort clientTargetSeq;

    [SerializeField]
    private short targetToSafeEdgeDist;

    [SerializeField]
    private bool restoreRequestedFlag = false;

    private void Start()
    {
        GameManager.Conn.Db.GameCoreSnap.OnInsert += (EventContext ctx, GameCoreSnap row) =>
        {
            GameManager.Inst.GameCore = MemoryPackSerializer.Deserialize<GameCore>(
                row.BinaryData.ToArray()
            );
            Debug.Log("GameCoreSnap inserted! Deserializing and restoring game core.");
            restoreRequestedFlag = false;
        };
        GameManager.Conn.Db.AuthFrame.OnInsert += (EventContext ctx, AuthFrame row) =>
        {
            foreach (InputFrame inputFrame in row.Frames)
            {
                if (inputFrame.Seq.IsAhead(latestServerSeq))
                    latestServerSeq = inputFrame.Seq;
                else
                    Debug.Log(
                        "AuthFrame inserted! Skipping input frame "
                            + inputFrame.Seq
                            + " because it's behind the latest server seq "
                            + latestServerSeq
                    );
            }
        };
    }

    private void FixedUpdate()
    {
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
                Debug.LogError("No input frame found for seq " + GameManager.Inst.GameCore.Seq);
                break;
            }

            List<InputEvent> inputEvents = MemoryPackSerializer.Deserialize<List<InputEvent>>(
                inputFrame.InputEventsList.ToArray()
            );

            GameManager.Inst.GameCore.Step(inputEvents);
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
        foreach (AuthFrame authFrame in GameManager.Conn.Db.AuthFrame.Iter())
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
        ushort? oldestSeq = GetOldestSeq();
        if (oldestSeq == null)
            return true;

        if (GameManager.Inst.GameCore.Seq.IsBehind(oldestSeq.Value))
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
        GameManager
            .Conn.SubscriptionBuilder()
            .OnError(
                (ErrorContext ctx, System.Exception ex) =>
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
        foreach (AuthFrame authFrame in GameManager.Conn.Db.AuthFrame.Iter())
        {
            foreach (InputFrame inputFrame in authFrame.Frames)
            {
                if (oldestSeq == null || inputFrame.Seq.IsBehind(oldestSeq.Value))
                {
                    oldestSeq = inputFrame.Seq;
                }
            }
        }
        return oldestSeq;
    }
}
