using System;
using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;
using UnityEngine.Events;

public class MarbleDetectorAuth : GameComponentAuth<MarbleDetectorComponent>
{
    [SerializeField]
    private List<GameComponentAuth> MarbleSignalReceivers = new List<GameComponentAuth>();

    public bool CollisionEnterDetection = true;
    public bool TriggerEnterDetection = true;

    public bool CollisionStayDetection = false;
    public bool TriggerStayDetection = false;

    void OnValidate()
    {
        for (int i = MarbleSignalReceivers.Count - 1; i >= 0; i--)
        {
            //Check that they implement the MarbleSignalReceiver interface
            var receiver = MarbleSignalReceivers[i];
            if (receiver == null)
            {
                Debug.LogWarning($"MarbleSignalReceiver {i} is null");
                continue;
            }
            if (!(receiver is IMarbleSignalReceiver))
            {
                Debug.LogError(
                    $"MarbleSignalReceiver {receiver.name} does not implement the IMarbleSignalReceiver interface"
                );
                MarbleSignalReceivers.RemoveAt(i);
            }
        }
    }
}
