using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// API 호출 실패 시 반환되는 에러 정보.
    /// 서버에서 정의한 에러 코드, 메시지, 필드명을 포함.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// 에러 코드 (예: "USER_NOT_FOUND", "INVALID_PASSWORD")
        /// </summary>
        public string code { get; set; }
        
        /// <summary>
        /// 사용자에게 표시 가능한 에러 메시지
        /// </summary>
        public string message { get; set; }
        
        /// <summary>
        /// 에러가 발생한 입력 필드명 (옵션)
        /// </summary>
        public string field { get; set; }
    }
    
    /// <summary>
    /// 서버에서 공통적으로 반환하는 API 응답 포맷.
    /// 성공 여부, 에러 정보, 결과 데이터를 포함.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// JSON 루트 키 (ex: { "res": { ok, error, data } })
        /// </summary>
        public const string ResponseKey = "res";
        
        /// <summary>
        /// 요청이 성공했는지 여부 (true = 성공)
        /// </summary>
        public bool ok;

        /// <summary>
        /// 실패 시 포함되는 에러 정보
        /// </summary>
        public ApiError error { get; set; }

        /// <summary>
        /// 서버에서 내려주는 데이터 (JSON 전체)
        /// </summary>
        public Dictionary<string, object> data;
        
        /// <summary>
        /// 요청 성공 여부 (읽기 전용 프로퍼티)
        /// </summary>
        public bool IsSuccess => ok;

        /// <summary>
        /// 요청 실패 여부 (읽기 전용 프로퍼티)
        /// </summary>
        public bool IsFailure => !ok || error != null;
    }
}
