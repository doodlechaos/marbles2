using GameCoreLib;
using TMPro;
using UnityEngine;

/// <summary>
/// Unity component that binds to a MarbleComponent and displays marble-related visuals.
/// Automatically bound by TileBinding when a marble prefab is instantiated.
/// </summary>
public class MarbleBinding : GCBinding<MarbleComponent>, IAccountCustomizationConsumer
{

    [SerializeField]
    private TextMeshPro _usernameText;

    [SerializeField]
    private TextMeshPro pointsText;

    // Cache the last displayed points to avoid unnecessary updates
    private int lastDisplayedPoints = int.MinValue;

    /// <summary>
    /// Convenience accessor for the bound MarbleComponent.
    /// </summary>
    public MarbleComponent Marble => BoundComponent;

    protected override void OnBindingChanged(
        MarbleComponent previousComponent,
        MarbleComponent newComponent
    )
    {
        // Reset cache when binding changes
        lastDisplayedPoints = int.MinValue;

        // Immediately update visuals with new component data
        if (newComponent != null)
        {
            RefreshPointsDisplay();
        }
        else
        {
            ClearPointsDisplay();
        }
    }

    public void ApplyAccountCustomization(AccountVisual visual)
    {
        _usernameText.SetText(visual.Username);
    }

    protected override void UpdateVisuals()
    {
        // Only update if points have changed (simple optimization)
        if (BoundComponent.Points != lastDisplayedPoints)
        {
            RefreshPointsDisplay();
        }
    }

    private void RefreshPointsDisplay()
    {
        if (pointsText == null || BoundComponent == null)
            return;

        lastDisplayedPoints = BoundComponent.Points;
        pointsText.text = lastDisplayedPoints.ToString();
    }

    private void ClearPointsDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = string.Empty;
        }
    }
}
