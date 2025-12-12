using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;
using UnityEngine.Pool;

public class GameTileRenderer : TileRenderer
{
    [SerializeField]
    private DoorAnimationCfg _cfg;

    private ObjectPool<AnimationDoor> _doorPool;
    private List<AnimationDoor> _spinDoors = new List<AnimationDoor>();
    private Transform _spinningDoorsRoot;
    private Transform _openCloseDoor;

    private void Awake()
    {
        _doorPool = new ObjectPool<AnimationDoor>(
            () => Instantiate(_cfg.AnimationDoorPrefab, transform),
            (door) => door.gameObject.SetActive(true),
            (door) => door.gameObject.SetActive(false),
            (door) => DestroyImmediate(door.gameObject),
            true,
            10,
            100
        );
        _spinningDoorsRoot = new GameObject("SpinningDoorsRoot").transform;
        _spinningDoorsRoot.SetParent(transform);
        _spinningDoorsRoot.localPosition = new Vector3(0, 0, _cfg.SpinRootPosZOffset);
    }

    public override void Render(TileBase tile)
    {
        if (tile is not GameTileBase gameTile)
        {
            Debug.LogError($"Tile is not a GameTileBase: {tile.GetType().Name}");
            return;
        }

        UpdateDoors(gameTile);

        base.Render(gameTile);
    }

    private void UpdateDoors(GameTileBase gameTile)
    {
        //If we're in closing door state, interpolate between the start and end positions based on the t steps for this state
        if (gameTile.State == GameTileState.ClosingDoor)
        {
            float t = gameTile.StateSteps / (GameTileBase.CLOSING_DOOR_DURATION_SEC * 60.0f);
            EnsureOpenCloseDoorSpawned();
            _openCloseDoor.transform.position = Vector3.LerpUnclamped(
                transform.position - new Vector3(_cfg.OpenDoorOffset, 0, 0),
                transform.position,
                t
            );
        }
        //If we're in spinning door state, make sure we have enough animation doors spawned, and set their position based on the t steps for this state
        if (gameTile.State == GameTileState.Spinning)
        {
            float t = gameTile.StateSteps / (GameTileBase.SPINNING_DURATION_SEC * 60.0f);

            t = _cfg.SpinAnimationCurve.Evaluate(t);

            EnsureSpinningDoorsSpawned(_cfg.AnimationDoorCount);

            float rootStartRotationX = (_spinDoors.Count - 1) * _cfg.SpinDegreesPerDoor;
            float rootEndRotationX = 0;
            Debug.Log(
                $"_cfg.AnimationDoorCount: {_cfg.AnimationDoorCount} \n _spinDoors.Count: {_spinDoors.Count} \n rootStartRotationX: {rootStartRotationX} rootEndRotationX: {rootEndRotationX} t: {t}"
            );

            float x =
                rootStartRotationX + Mathf.DeltaAngle(rootStartRotationX, rootEndRotationX) * t;
            _spinningDoorsRoot.localRotation = Quaternion.Euler(x, 0f, 0f);
        }
        else
        {
            _spinningDoorsRoot.gameObject.SetActive(false);
        }

        //If we're in opening door state, interpolate between the start and end positions based on the t steps for this state
        if (gameTile.State == GameTileState.OpeningDoor)
        {
            float t = gameTile.StateSteps / (GameTileBase.OPENING_DOOR_DURATION_SEC * 60.0f);
            EnsureOpenCloseDoorSpawned();
            _openCloseDoor.transform.position = Vector3.LerpUnclamped(
                transform.position,
                transform.position - new Vector3(_cfg.OpenDoorOffset, 0, 0),
                t
            );
        }
    }

    private void EnsureOpenCloseDoorSpawned()
    {
        if (_openCloseDoor != null)
            return;
        _openCloseDoor = _doorPool.Get().transform;
        _openCloseDoor.gameObject.SetActive(true);
    }

    private void EnsureSpinningDoorsSpawned(int count)
    {
        if (_spinningDoorsRoot.gameObject.activeSelf)
            return;

        _spinningDoorsRoot.gameObject.SetActive(true);

        _spinningDoorsRoot.eulerAngles = new Vector3(0, 0, 0);

        for (int i = _spinDoors.Count - 1; i >= 0; i--)
        {
            _doorPool.Release(_spinDoors[i]);
            _spinDoors.RemoveAt(i);
        }

        for (int i = 0; i < count; i++)
        {
            AnimationDoor door = _doorPool.Get();
            _spinDoors.Add(door);

            door.transform.position = transform.position;
            door.transform.localRotation = Quaternion.identity;
            door.transform.SetParent(_spinningDoorsRoot);

            _spinningDoorsRoot.Rotate(new Vector3(_cfg.SpinDegreesPerDoor, 0, 0), Space.World);
            //_spinningDoorsRoot.eulerAngles =
            //   _spinningDoorsRoot.eulerAngles + new Vector3(_cfg.SpinDegreesPerDoor, 0, 0);
        }
    }
}
