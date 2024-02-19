using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;
    private float currentTime;
    [SerializeField] private float adsInterval = 40f;
    public bool wasPlayedOnGameOver = false;
    public bool wasShowed = false;

    void Awake()
    {
        currentTime = Time.realtimeSinceStartup;
        
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;
    }

    public void LoadAd()
    {
        PlayerPrefs.SetString("AdUnitId", _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if(Time.realtimeSinceStartup - currentTime > adsInterval && !wasPlayedOnGameOver) {
            currentTime = Time.realtimeSinceStartup;
            Advertisement.Show(_adUnitId, this);
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {        
        wasPlayedOnGameOver = false;
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        wasPlayedOnGameOver = false;     
    }

    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) {
        LoadAd();
        wasShowed = true;
    }
}