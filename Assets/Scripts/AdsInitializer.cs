using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine.Networking;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;
    private static AdsInitializer AdsInitializerInstance;
    void Awake()
    {
        StartCoroutine(Initialization());
        if (AdsInitializerInstance == null)
        {
            AdsInitializerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (AdsInitializerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    public async void InitializeAds()
    {
        _gameId = _androidGameId;
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
            await System.Threading.Tasks.Task.Yield();
        }
    }

    public void OnInitializationComplete()
    {
        GetComponent<InterstitialAd>().LoadAd();
        GetComponent<RewardedAd>().LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {

    }

    private IEnumerator Initialization()
    {
        while (Advertisement.isInitialized == false)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                InitializeAds();
            }
        }
        yield return null;
    }
}