using System.Collections;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class PodiumManager : MonoBehaviour
{
    [SerializeField]
    private float _panelSlideInDuration = 1.0f;

    [SerializeField]
    private float _entriesScrollDownSpeed = 2.0f;

    [SerializeField]
    private float _entriesScrollDownMaxDuration = 2.0f;

    [SerializeField]
    private float _finalDisplayHoldDuration = 2.0f;

    [SerializeField]
    private float _panelSlideOutDuration = 2.0f;

    [SerializeField]
    private Transform _gameTile1Origin;

    [SerializeField]
    private Transform _gameTile2Origin;

    [SerializeField]
    private PodiumEntry _podiumEntryPrefab;

    [SerializeField]
    private float _spacingBetweenEntries = 100.0f;

    [SerializeField]
    private Transform _entriesParent;

    private Coroutine _currPodiumCoroutine = null;

    [Header("Test Inputs")]
    [SerializeField]
    private uint _testTotalPrize = 1000;

    [SerializeField]
    private ulong[] _testAccountIdsInRankOrder = new ulong[] { 1, 2, 3, 4, 5 };

    [SerializeField]
    private byte _testWorldId = 1;

    [ProButton]
    public void TestRunPodiumAnimation()
    {
        RunPodiumAnimation(_testTotalPrize, _testAccountIdsInRankOrder, _testWorldId);
    }

    public void RunPodiumAnimation(uint prize, ulong[] accountIdsInRankOrder, byte worldId)
    {
        if (_currPodiumCoroutine != null)
        {
            StopCoroutine(_currPodiumCoroutine);
        }
        _currPodiumCoroutine = StartCoroutine(
            RunPodiumAnimationCoroutine(prize, accountIdsInRankOrder, worldId)
        );
    }

    private IEnumerator RunPodiumAnimationCoroutine(
        uint totalPrize,
        ulong[] accountIdsInRankOrder,
        byte worldId
    )
    {
        for (int i = _entriesParent.childCount - 1; i >= 0; i--)
            Destroy(_entriesParent.GetChild(i).gameObject);

        float timer = 0.0f;
        Transform target = worldId == 1 ? _gameTile1Origin : _gameTile2Origin;
        float xOffsetFromTarget = worldId == 1 ? -10.0f : 10.0f;
        //Animate the panel moving to the correct worldId position
        while (timer < _panelSlideInDuration)
        {
            float t = timer / _panelSlideInDuration;
            transform.position = Vector3.Lerp(
                target.position + new Vector3(xOffsetFromTarget, 0.0f, 0.0f),
                target.position,
                t
            );
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;

        //Spawn a podium entry for each account id
        for (int i = 0; i < accountIdsInRankOrder.Length; i++)
        {
            ulong accountId = accountIdsInRankOrder[i];
            int rank = i + 1;
            int prizeEarned = (int)totalPrize / rank;
            PodiumEntry podiumEntry = Instantiate(_podiumEntryPrefab, _entriesParent);
            podiumEntry.Init(accountId, rank, prizeEarned);
            podiumEntry.transform.localPosition = new Vector3(0, -i * _spacingBetweenEntries, 0);
        }

        //TODO:
        //The entries should all start above the top of the masked area of the viewport.
        //Then they all scroll down at a constant speed until the rank 1 player reaches the top edge (just like mario kart at the end of a race)
        //I want it to travel at a constant speed regardless of how many players there are, but if there are too many players such that
        //It will take longer than the maxDuration, then we need to increase the speed so that it finishes sliding in the max duration amount of time.
        Vector3 entriesRootEndLocalPos = new Vector3(0, 350, 0);
        Vector3 entriesRootStartLocalPos =
            entriesRootEndLocalPos
            + new Vector3(0, _entriesParent.childCount * _spacingBetweenEntries, 0);
        timer = 0;
        while (timer < _entriesScrollDownMaxDuration)
        {
            //If the local player entry scrolls down below the BottomLockEntry, enable the BottomLockEntry to prevent the local player from being unable to see their entry
            float t = timer / _entriesScrollDownMaxDuration;
            _entriesParent.localPosition = Vector3.Lerp(
                entriesRootStartLocalPos,
                entriesRootEndLocalPos,
                t
            );
            timer += Time.deltaTime;
            yield return null;
        }

        //Hold for viewing
        timer = 0;
        while (timer < _finalDisplayHoldDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        //After a 2 second delay, explode the local player's points earned into particles which fly into the user's points display icon (we can color it yellow until it matches the server value)
        timer = 0;
        while (timer < _panelSlideOutDuration)
        {
            float t = timer / _panelSlideOutDuration;
            transform.position = Vector3.Lerp(
                target.position,
                target.position + new Vector3(xOffsetFromTarget, 0.0f, 0.0f),
                t
            );
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Podium animation completed");
        yield return null;
    }
}
