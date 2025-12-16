using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PodiumEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _rankText;

    [SerializeField]
    private Image _playerPfp;

    [SerializeField]
    private TextMeshProUGUI _usernameText;

    [SerializeField]
    private TextMeshProUGUI _prizeEarnedText;

    [SerializeField]
    private bool _isLocalPlayer = false;

    [SerializeField]
    private Image _backgroundImage;

    public void Init(ulong accountId, int rank, int prizeEarned, bool isLocalPlayer)
    {
        _rankText.SetText($"{rank}.");
        _prizeEarnedText.SetText($"+{prizeEarned}");
        _isLocalPlayer = isLocalPlayer;
        _backgroundImage.enabled = isLocalPlayer;

        StartCoroutine(FetchAccountCustomizationDataCoroutine(accountId));
    }

    private IEnumerator FetchAccountCustomizationDataCoroutine(ulong accountId)
    {
        _usernameText.SetText("TODO...");
        yield return null;
    }
}
