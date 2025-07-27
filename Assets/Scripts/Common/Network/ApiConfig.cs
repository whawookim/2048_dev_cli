using Firebase.RemoteConfig;

namespace Network
{
    /// <summary>
    /// Firebase RemoteConfig에서 API 설정값을 불러오는 클래스.
    /// 현재는 API BaseUrl만 관리하고 있음.
    /// </summary>
    public static class ApiConfig
    {
        private const string RemoteKeyApiBaseUrl = "api_base_url";
        
        /// <summary>
        /// Firebase RemoteConfig에서 불러온 API 서버 주소
        /// </summary>
        public static string ApiBaseUrl { get; private set; }

        /// <summary>
        /// RemoteConfig 초기화 후 API BaseUrl을 설정
        /// </summary>
        public static void InitDebugRemoteConfig()
        {
            ApiBaseUrl = FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteKeyApiBaseUrl).StringValue;
        }
    }
}
