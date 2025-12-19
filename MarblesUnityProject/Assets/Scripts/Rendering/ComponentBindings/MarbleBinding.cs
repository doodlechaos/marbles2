using GameCoreLib;
using TMPro;
using UnityEngine;

/// <summary>
/// Unity component that binds to a MarbleComponent and displays marble-related visuals.
/// Handles account customization registration/unregistration during pool lifecycle.
/// </summary>
public class MarbleBinding : GCBinding<MarbleComponent>, IAccountCustomizationConsumer
{
    [SerializeField]
    private TextMeshPro _usernameText;

    
    [SerializeField]
    private TextMeshPro _pointsText;

    private int _lastDisplayedPoints = int.MinValue;
    private ulong _registeredAccountId;

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
        if (_usernameText != null)
        {
            _usernameText.SetText(visual.Username);
        }
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
    }
}
