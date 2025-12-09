using System;
using GameCoreLib;
using UnityEngine;
using UnityEngine.Events;

public class MarbleDetectorAuth : GameComponentAuth<MarbleDetectorComponent>
{
    [Serializable]
    public class DetectedMarbleEvent : UnityEvent<PlayerMarbleAuth> { }

    [SerializeField]
    DetectedMarbleEvent OnMarbleDetected;

    public bool CollisionEnterDetection = true;
    public bool TriggerEnterDetection = true;

    public bool CollisionStayDetection = false;
    public bool TriggerStayDetection = false;
}
