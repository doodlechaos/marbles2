using GameCoreLib;
using SpacetimeDB.Types;
using UnityEngine;
using MemoryPack;
using System.Collections.Generic;
public class Synchronizer : MonoBehaviour
{
    [SerializeField] private ushort safeSeqEdge;
    [SerializeField] private ushort latestServerSeq;

    [SerializeField] private ushort clientTargetSeq;
    [SerializeField] private short targetToSafeEdgeDist;

    private void FixedUpdate()
    {
        RefreshSafeSeqEdge();

        clientTargetSeq = clientTargetSeq.LerpTo(safeSeqEdge, 0.1f);
        targetToSafeEdgeDist = clientTargetSeq.ClosestDiffTo(safeSeqEdge);

        ushort clientSeq = GameManager.Inst.GameCore.Seq;

        while (clientSeq.IsBehind(clientTargetSeq))
        {
            InputFrame inputFrame = FindInputFrame(clientSeq);
            if (inputFrame == null)
            {
                Debug.LogError("No input frame found for seq " + clientSeq);
                break;
            }

            List<InputEvent> inputEvents = MemoryPackSerializer.Deserialize<List<InputEvent>>(inputFrame.InputEventsList);

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
