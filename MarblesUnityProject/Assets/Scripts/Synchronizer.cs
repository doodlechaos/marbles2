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

    private void FixedUpdate()
    {
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
}
