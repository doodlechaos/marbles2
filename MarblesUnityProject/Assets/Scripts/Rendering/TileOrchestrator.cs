using UnityEngine;

public class TileOrchestrator : MonoBehaviour
{
    public Transform ThroneTileOrigin;
    public Transform GameTile1Origin;
    public Transform GameTile2Origin;

    private Vector3 _throneTileTargetPosition;
    private Vector3 _gameTile1TargetPosition;
    private Vector3 _gameTile2TargetPosition;

    [SerializeField]
    private float _lerpSpeed = 7f;

    [SerializeField]
    private float _tileWidth = 10f;

    [SerializeField]
    private float _tileHeight = 20f;

    [SerializeField]
    private float _tileSpacing = 1f;

    private int _lastScreenWidth;
    private int _lastScreenHeight;

    void Start()
    {
        UpdateTargetPositions();
    }

    void Update()
    {
        // Check if the Screen dimensions have changed
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            UpdateTargetPositions();
        }

        // Lerp tiles to their target positions
        ThroneTileOrigin.position = Vector3.Lerp(
            ThroneTileOrigin.position,
            _throneTileTargetPosition,
            Time.deltaTime * _lerpSpeed
        );
        GameTile1Origin.position = Vector3.Lerp(
            GameTile1Origin.position,
            _gameTile1TargetPosition,
            Time.deltaTime * _lerpSpeed
        );
        GameTile2Origin.position = Vector3.Lerp(
            GameTile2Origin.position,
            _gameTile2TargetPosition,
            Time.deltaTime * _lerpSpeed
        );
    }

    private void UpdateTargetPositions()
    {
        // All tiles are on z = 0 in the x-y plane
        if (Screen.height > Screen.width)
        {
            // Vertical layout (portrait): Throne Tile on top, game tile 1 below, game tile 2 at bottom
            float verticalOffset = _tileHeight + _tileSpacing;
            _throneTileTargetPosition = new Vector3(0f, verticalOffset, 0f);
            _gameTile1TargetPosition = new Vector3(0f, 0f, 0f);
            _gameTile2TargetPosition = new Vector3(0f, -verticalOffset, 0f);
        }
        else
        {
            // Horizontal layout (landscape): GameTile1 on left, throne tile in middle, game tile 2 on right
            float horizontalOffset = _tileWidth + _tileSpacing;
            _gameTile1TargetPosition = new Vector3(-horizontalOffset, 0f, 0f);
            _throneTileTargetPosition = new Vector3(0f, 0f, 0f);
            _gameTile2TargetPosition = new Vector3(horizontalOffset, 0f, 0f);
        }
    }
}
