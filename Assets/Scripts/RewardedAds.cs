using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections;

public class RewardedAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitIdReward;
    public GameObject _showAdButton;
    public Player player;

    void Awake()
    {
        _adUnitIdReward = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSAdUnitId
            : _androidAdUnitId;
    }

    public void LoadAd()
    {
        Advertisement.Load(_adUnitIdReward, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId.Equals(_adUnitIdReward))
        {
            ReattachListener();
        }
    }

    public void ShowAd()
    {
        Advertisement.Show(_adUnitIdReward, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        LoadAd();
        player.ResumeGame();
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        if (_showAdButton != null)
        {
            _showAdButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private async void ReattachListener()
    {
        while (_showAdButton == null)
        {
            await System.Threading.Tasks.Task.Delay(5000);
        }

        _showAdButton.GetComponent<Button>().onClick.AddListener(ShowAd);
        await System.Threading.Tasks.Task.Yield();
    }
}