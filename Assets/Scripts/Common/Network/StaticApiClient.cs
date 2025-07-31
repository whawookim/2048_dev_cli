using System.Collections.Generic;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// 기존 static 방식 호출을 유지하면서 내부적으로 구조화된 ApiClient 인스턴스를 사용하는 Wrapper.
    /// 초기화 시 Firebase RemoteConfig를 통해 BaseUrl을 설정함.
    /// </summary>
    public static class StaticApiClient
    {
        private static IApiClient _client;

        /// <summary>
        /// Firebase RemoteConfig에서 BaseUrl을 불러와 초기화합니다.
        /// 앱 시작 시 한 번만 호출되어야 합니다.
        /// </summary>
        public static void Initialize()
        {
            ApiConfig.InitDebugRemoteConfig();
            MyDebug.Log($"ApiConfig.ApiBaseUrl : {ApiConfig.ApiBaseUrl}");
            // RemoteConfig 적용된 값
            _client = new ApiClient(ApiConfig.ApiBaseUrl);
        }

        /// <summary>
        /// 공통 API 호출 메서드 (기존 ApiClient.SendAsync 대체용)
        /// </summary>
        public static Task<ApiResponse> SendAsync(string endpoint, Dictionary<string, object> body = null, HttpMethod method = HttpMethod.POST)
        {
            if (_client == null)
            {
                MyDebug.LogError("StaticApiClient 사용 전 InitializeAsync()가 호출되지 않았습니다.");
                return Task.FromResult<ApiResponse>(null);
            }

            return _client.SendAsync(endpoint, body, method);
        }
    }
}
