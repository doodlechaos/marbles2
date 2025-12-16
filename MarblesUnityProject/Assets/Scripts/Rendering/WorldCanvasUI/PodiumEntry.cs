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
    private bool _isLocalPlayer = false; //Just for my own view for debugging reference

    [SerializeField]
    private Image _backgroundImage;

    public void Init(ulong accountId, int rank, int prizeEarned, bool isLocalPlayer)
    {
        _rankText.SetText($"{rank}.");
        _prizeEarnedText.SetText($"+{prizeEarned}");
        _isLocalPlayer = isLocalPlayer;

        StartCoroutine(FetchAccountCustomizationDataCoroutine(accountId));
    }

    private IEnumerator FetchAccountCustomizationDataCoroutine(ulong accountId)
    {
        _usernameText.SetText("TODO...");
        yield return null;
    }

    //Have to do this instead of activating and deactivating the gameobject because that will stop the coroutine or not allow it to start
    public void Hide()
    {
        _backgroundImage.enabled = false;
        _rankText.enabled = false;
        _playerPfp.enabled = false;
        _usernameText.enabled = false;
        _prizeEarnedText.enabled = false;
        _backgroundImage.enabled = false;
    }

    public void Show()
    {
        _backgroundImage.enabled = true;
        _rankText.enabled = true;
        _playerPfp.enabled = true;
        _usernameText.enabled = true;
        _prizeEarnedText.enabled = true;
        _backgroundImage.enabled = _isLocalPlayer;
    }
}
