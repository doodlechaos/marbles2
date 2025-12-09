using System;
using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;

public class MarbleDetectorAuth
    : GameComponentAuth<MarbleDetectorComponent>,
        IComponentReferenceAuthoring
{
    [SerializeField]
    [Tooltip(
        "Components that receive marble detection signals (must implement IMarbleSignalReceiver)"
    )]
    private List<GameComponentAuth> MarbleSignalReceivers = new List<GameComponentAuth>();

    [Header("Detection Settings")]
    public bool CollisionEnterDetection = true;
    public bool TriggerEnterDetection = true;
    public bool CollisionStayDetection = false;
    public bool TriggerStayDetection = false;

    protected override MarbleDetectorComponent CreateComponent()
    {
        return new MarbleDetectorComponent
        {
            CollisionEnterDetection = CollisionEnterDetection,
            TriggerEnterDetection = TriggerEnterDetection,
            CollisionStayDetection = CollisionStayDetection,
            TriggerStayDetection = TriggerStayDetection,
        };
    }

    public void ResolveReferences(GCComponent component, ComponentExportContext context)
    {
        if (component is not MarbleDetectorComponent detector)
            return;

        detector.ReceiverComponentIds.Clear();

        foreach (var receiverAuth in MarbleSignalReceivers)
        {
            if (receiverAuth == null)
                continue;

            if (context.TryGetComponentId(receiverAuth, out var componentId))
            {
                detector.ReceiverComponentIds.Add(componentId);
            }
            else
            {
                Debug.LogWarning(
                    $"[MarbleDetectorAuth] Could not resolve component ID for receiver: {receiverAuth.name}"
                );
            }
        }
    }

    void OnValidate()
    {
        for (int i = MarbleSignalReceivers.Count - 1; i >= 0; i--)
        {
            var receiver = MarbleSignalReceivers[i];
            if (receiver == null)
            {
                Debug.LogWarning($"MarbleSignalReceiver {i} is null");
                continue;
            }
            if (receiver is not IMarbleSignalReceiver)
            {
                Debug.LogError(
                    $"MarbleSignalReceiver {receiver.name} does not implement the IMarbleSignalReceiver interface"
                );
                MarbleSignalReceivers.RemoveAt(i);
            }
        }
    }
}
