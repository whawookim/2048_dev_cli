using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Network
{
    /// <summary>
    /// API 서버와 통신을 담당하는 정적 클라이언트.
    /// RESTful 엔드포인트에 대해 JSON 요청/응답을 처리하며,
    /// GET 쿼리스트링 변환 및 POST 바디 직렬화도 포함.
    /// 응답 데이터는 ApiResponse 형태로 역직렬화되며,
    /// 내부적으로 AutoPatch 시스템이 함께 적용된다.
    /// </summary>
    public class ApiClient : IApiClient
    {
        /// <summary>
        /// API 호출 base url 주소
        /// </summary>
        /// <remarks>생성자에서 세팅되어야 하며 Firebase </remarks>
        private readonly string _baseUrl;

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// 지정된 endpoint로 비동기 HTTP 요청을 전송하고 응답을 ApiResponse 형태로 반환합니다.
        /// </summary>
        /// <param name="endpoint">API 엔드포인트 (예: "/login")</param>
        /// <param name="body">요청 본문에 포함될 Dictionary 형태의 JSON 데이터</param>
        /// <param name="method">HTTP 메서드 (POST, GET 등)</param>
        /// <returns>역직렬화된 ApiResponse 객체</returns>
        public async Task<ApiResponse> SendAsync(string endpoint, Dictionary<string, object> body = null,
            HttpMethod method = HttpMethod.POST)
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                MyDebug.LogError("BaseUrl is null or empty.");
                return null;
            }

            try
            {
                string url = $"{_baseUrl}{endpoint}";

                if (method == HttpMethod.GET && body != null && body.Count > 0)
                {
                    var query = string.Join("&", body.Select(kvp =>
                        $"{UnityWebRequest.EscapeURL(kvp.Key)}={UnityWebRequest.EscapeURL(kvp.Value.ToString())}"));
                    url += "?" + query;
                }

                using var request = new UnityWebRequest(url, method.ToString());
                request.downloadHandler = new DownloadHandlerBuffer();

                if (method != HttpMethod.GET)
                {
                    string json = JsonConvert.SerializeObject(body ?? new Dictionary<string, object>());
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    MyDebug.LogError($"API 요청 실패: {request.error}");
                    return null;
                }

                var response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);

                // 서버에서 받은 데이터 자동 패치 처리
                ApiDeltaDispatcher.ApplyAuto(response.data);

                return response;
            }
            catch (Exception ex)
            {
                MyDebug.LogException(ex);
                return null;
            }
        }
    }
}
