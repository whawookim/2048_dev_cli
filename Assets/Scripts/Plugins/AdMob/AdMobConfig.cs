using Firebase.RemoteConfig;

namespace Plugins.AdMob
{
    public static class AdMobConfig
    {
        private const string RemoteKey_BannerUnitId = "banner_unit_id";
        
        /// <summary>
        /// Firebase RemoteConfig에서 불러온 Banner Unit Id
        /// </summary>
        public static string BannerUnitId { get; private set; }

        /// <summary>
        /// RemoteConfig 초기화 후 Banner Unit Id을 설정
        /// </summary>
        public static void InitRemoteConfig()
        {
            BannerUnitId = FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteKey_BannerUnitId).StringValue;
        }
    }
}
