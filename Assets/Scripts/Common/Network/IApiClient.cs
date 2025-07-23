using System.Collections.Generic;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// API 클라이언트 인터페이스. 공통 SendAsync 메서드 정의.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// 비동기 API 요청 전송
        /// </summary>
        /// <param name="endpoint">요청 endpoint (예: "/login")</param>
        /// <param name="body">JSON 직렬화용 데이터</param>
        /// <param name="method">HTTP 요청 메서드</param>
        /// <returns>ApiResponse 객체</returns>
        Task<ApiResponse> SendAsync(string endpoint, Dictionary<string, object> body = null, HttpMethod method = HttpMethod.POST);
    }
}
