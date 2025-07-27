using System.Threading.Tasks;
using GoogleMobileAds.Api;
using Plugins.AdMob;
using UnityEngine;

/// <summary>
/// 광고 관리 매니저
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    /// <summary>
    /// Editor 배너 광고 Id
    /// </summary>
    [SerializeField]
    private string bannerUnitId = "ca-app-pub-3940256099942544/6300978111";

#region MonoBehaviour
    private void Awake()
    {
        Debug.Assert(Instance == null);
        
        Instance = this;
    }

    private void OnDestroy()
    {
        Debug.Assert(Instance == this);
        
        Instance = null;
    }
#endregion
    
    public void Initialize()
    {
        AdMobConfig.InitRemoteConfig();

        MobileAds.Initialize(initStatus =>
        {
            MyDebug.Log("Admob initialized");
        });
    }

    public string GetBannerAdUnitId()
    {
#if UNITY_EDITOR
        return bannerUnitId;
#else
        return AdMobConfig.BannerUnitId;
#endif
    }
    
    private BannerView _bannerView;

    public async Task LoadAndShowBannerAsync(bool showBanner = false)
    {
        if (_bannerView == null)
        {
            bool isLoading = true;
            
            // 배너 위치 설정 (예: 하단 중앙)
            _bannerView = new BannerView(GetBannerAdUnitId(), AdSize.Banner, AdPosition.Bottom);
            
            _bannerView.OnBannerAdLoaded += () =>
            {
                isLoading = false;
                MyDebug.Log("Banner Ad load Success");
            };
        
            _bannerView.OnBannerAdLoadFailed += (error) =>
            {
                isLoading = false;
                MyDebug.LogError($"{error} Banner Ad load Failed");
            };
            
            // 광고 요청 객체 생성 (v10 방식: 매개변수 없이 기본 생성자 사용)
            AdRequest request = new AdRequest();
        
            // 광고 로드
            _bannerView.LoadAd(request);

            while (isLoading)
                await Task.Yield();
        }

        if (showBanner)
        {
            ShowBanner();
        }
    }

    public void ShowBanner()
    {
        if (_bannerView == null) return;
        
        _bannerView.Show();
        MyDebug.LogWarning("Show Banner");
    }

    public void HideBanner()
    {
        if (_bannerView == null) return;
        
        _bannerView.Hide();
        MyDebug.LogWarning("Hide Banner");
    }
}
