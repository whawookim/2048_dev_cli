using System.Threading.Tasks;
using GoogleMobileAds.Api;
using Plugins.AdMob;
using UnityEngine;

/// <summary>
/// 광고 관리 매니저
/// TODO: 현재 광고가 최상단으로 올라오니 이거 다른 방법으로 구현하든 방법을 찾을 것.
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

        DestroyBanner();
    }
#endregion
    
    public void Initialize()
    {
        AdMobConfig.InitRemoteConfig();
        
        try
        {
            MobileAds.Initialize(initStatus =>
            {
                MyDebug.Log("[AdManager] Admob initialized");
            });
        }
        catch (System.Exception e)
        {
            MyDebug.LogError($"[AdManager] AdMob initialize EXCEPTION: {e}");
        }
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
                // 일단 꺼둔다
                HideBanner();
                isLoading = false;
                MyDebug.Log("Banner Ad load Success");
            };
        
            _bannerView.OnBannerAdLoadFailed += (error) =>
            {
                // 켜있을리 없지만 일단 혹시 모르니
                HideBanner();
                DestroyBanner();
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

    public void DestroyBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
}
