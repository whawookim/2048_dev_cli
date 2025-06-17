using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;


public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    /// <summary>
    /// 배너 광고 Id
    /// "ca-app-pub-3194408665340443/8344250604"
    /// </summary>
    [SerializeField]
    private string bannerUnitId
#if UNITY_EDITOR
        = "ca-app-pub-3940256099942544/6300978111";
#else
        = "ca-app-pub-3194408665340443/8344250604";
#endif

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    public void Init()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Admob initialized");
        });
    }
    
    private BannerView bannerView;

    public IEnumerator LoadAndShowBannerProcess()
    {
        if (bannerView == null)
        {
            bool isLoading = true;
            
            // 배너 위치 설정 (예: 하단 중앙)
            bannerView = new BannerView(bannerUnitId, AdSize.Banner, AdPosition.Bottom);
            
            bannerView.OnBannerAdLoaded += () =>
            {
                isLoading = false;
                Debug.Log("Banner Ad load Success");
            };
        
            bannerView.OnBannerAdLoadFailed += (error) =>
            {
                isLoading = false;
                Debug.LogError($"{error} Banner Ad load Failed");
            };
            
            // 광고 요청 객체 생성 (v10 방식: 매개변수 없이 기본 생성자 사용)
            AdRequest request = new AdRequest();
        
            // 광고 로드
            bannerView.LoadAd(request);

            while (isLoading)
            {
                yield return null;
            }
        }

        ShowBanner();
    }

    public void ShowBanner()
    {
        if (bannerView == null) return;
        
        bannerView.Show();
    }

    public void HideBanner()
    {
        if (bannerView == null) return;
        
        bannerView.Hide();
    }
}
