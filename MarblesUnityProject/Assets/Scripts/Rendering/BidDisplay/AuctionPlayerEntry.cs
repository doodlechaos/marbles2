using TMPro;
using UnityEngine;

public class AuctionPlayerEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _rankText;

    [SerializeField]
    private TextMeshPro _playerNameText;

    [SerializeField]
    private TextMeshPro _bidAmountText;

    [SerializeField]
    private Vector3 _targetPosition;

    public void Init(ushort rank, string playerName, uint bidAmount)
    {
        _rankText.SetText($"{rank})");
        _playerNameText.SetText(playerName);
        _bidAmountText.SetText($"{bidAmount}x");
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _targetPosition,
            Time.deltaTime * 10.0f
        );
    }
}
