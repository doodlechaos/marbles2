using UnityEngine;

[CreateAssetMenu(fileName = "DoorAnimationCfg", menuName = "Marbles/DoorAnimationCfg")]
public class DoorAnimationCfg : ScriptableObject
{
    [SerializeField]
    public AnimationDoor AnimationDoorPrefab;

    [SerializeField]
    public int AnimationDoorCount = 10;

    [SerializeField]
    public float OpenDoorOffset = 10f;

    [SerializeField]
    public float SpinDegreesPerDoor = 10f;

    [SerializeField]
    public float SpinRootPosZOffset = 70f;

    [SerializeField]
    public AnimationCurve SpinAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);
}
