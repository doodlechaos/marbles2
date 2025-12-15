using System.Collections;
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

    public void Init(ulong accountId, int rank, int prizeEarned)
    {
        _rankText.SetText($"{rank}.");
        _prizeEarnedText.SetText($"+{prizeEarned}");

        //TODO: If the accountId is the local player's accountId, highlight the entry with an orange background

        StartCoroutine(FetchAccountCustomizationDataCoroutine(accountId));
    }

    private IEnumerator FetchAccountCustomizationDataCoroutine(ulong accountId)
    {
        _usernameText.SetText("TODO...");
        yield return null;
    }
}
