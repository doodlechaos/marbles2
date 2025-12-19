using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuctionPlayerEntry : MonoBehaviour, IAccountCustomizationConsumer
{
    [SerializeField]
    private TextMeshProUGUI _rankText;

    [SerializeField]
    private Image _playerPfp;

    [SerializeField]
    private TextMeshProUGUI _playerNameText;

    [SerializeField]
    private TextMeshProUGUI _bidAmountText;

    [SerializeField]
    private Vector3 _targetLocalPosition;

    [SerializeField]
    private float _movementSpeedScalar = 7.0f;

    public ulong AccountId { get; private set; }

    public void Init(ulong accountId, ushort rank, string playerName, uint bidAmount)
    {
        AccountId = accountId;
        _rankText.SetText($"{rank})");
        _playerNameText.SetText(playerName);
        _bidAmountText.SetText($"{bidAmount}x");

        AccountCustomizationCache.Inst.RegisterConsumer(accountId, this);
    }

    void OnDisable()
    {
        AccountCustomizationCache.Inst.UnregisterConsumer(AccountId, this);
    }

    public void UpdateDisplay(ushort rank, uint bidAmount)
    {
        _rankText.SetText($"{rank})");
        _bidAmountText.SetText($"{bidAmount}x");
    }

    public void ApplyAccountCustomization(AccountVisual visual)
    {
        _playerNameText.SetText(visual.Username);
        _playerPfp.sprite = visual.PfpSprite;
    }

    public void SetTargetLocalPos(Vector3 targetPosition)
    {
        _targetLocalPosition = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            _targetLocalPosition,
            Time.deltaTime * _movementSpeedScalar
        );
    }
}
