using GameCoreLib;
using TMPro;
using UnityEngine;

/// <summary>
/// Unity component that binds to a MarbleComponent and displays marble-related visuals.
/// </summary>
public class MarbleBinding : GCBinding<MarbleComponent>, IAccountCustomizationConsumer
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshPro _usernameText;

    [SerializeField]
    private TextMeshPro _pointsText;

    [Header("3D References")]
    [SerializeField] 
    private Renderer _marbleRenderer;
    
    [SerializeField]
    private string _shaderTextureProperty = "_MainTex"; // Usually "_MainTex" or "_BaseMap" for URP

    private int _lastDisplayedPoints = int.MinValue;
    private ulong _registeredAccountId;
    
    // Cache the block to avoid GC allocation every frame/update
    private MaterialPropertyBlock _propBlock;

    public MarbleComponent Marble => BoundComponent;

    protected override void OnBindingChanged(
        MarbleComponent previousComponent,
        MarbleComponent newComponent
    )
    {
        // Unregister from previous account
        if (_registeredAccountId != 0)
        {
            AccountCustomizationCache.Inst?.UnregisterConsumer(_registeredAccountId, this);
            _registeredAccountId = 0;
        }

        // Reset display state
        _lastDisplayedPoints = int.MinValue;

        if (newComponent != null)
        {
            // Register with new account
            _registeredAccountId = newComponent.AccountId;
            AccountCustomizationCache.Inst?.RegisterConsumer(_registeredAccountId, this);

            RefreshPointsDisplay();
        }
        else
        {
            ClearDisplay();
        }
    }

    public void ApplyAccountCustomization(AccountVisual visual)
    {
        // 1. Update Text
        if (_usernameText != null)
        {
            _usernameText.SetText(visual.Username);
        }

        // 2. Update 3D Texture efficiently
        UpdateMarbleTexture(visual.PfpSprite);
    }

    private void UpdateMarbleTexture(Sprite pfpSprite)
    {
        if (_marbleRenderer == null || pfpSprite == null) return;

        // Lazy initialization
        if (_propBlock == null)
            _propBlock = new MaterialPropertyBlock();

        // Get the current state of the renderer's properties
        _marbleRenderer.GetPropertyBlock(_propBlock);

        // Extract the underlying Texture2D from the Sprite
        // The cache creates sprites from textures, so this reference is valid.
        Texture2D texture = pfpSprite.texture;

        // Set the texture to the block
        _propBlock.SetTexture(_shaderTextureProperty, texture);

        // Apply the block to the renderer
        _marbleRenderer.SetPropertyBlock(_propBlock);
    }

    protected override void UpdateVisuals()
    {
        if (BoundComponent.Points != _lastDisplayedPoints)
        {
            RefreshPointsDisplay();
        }
    }

    private void RefreshPointsDisplay()
    {
        if (_pointsText == null || BoundComponent == null)
            return;

        _lastDisplayedPoints = BoundComponent.Points;
        _pointsText.text = _lastDisplayedPoints.ToString();
    }

    private void ClearDisplay()
    {
        if (_pointsText != null)
            _pointsText.text = string.Empty;

        if (_usernameText != null)
            _usernameText.text = string.Empty;
            
        // Optional: Reset texture to default on clear
        if (_marbleRenderer != null && _propBlock != null)
        {
             _propBlock.Clear();
             _marbleRenderer.SetPropertyBlock(_propBlock);
        }
    }
}