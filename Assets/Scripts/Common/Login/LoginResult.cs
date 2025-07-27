namespace Login
{
    /// <summary>
    /// 로그인 시 결과를 담는 객체.
    /// 성공 여부, 유저 ID, 토큰, 에러 메시지 포함.
    /// </summary>
    public class LoginResult
    {
        public bool IsSuccess { get; }
        public string UserId { get; }
        public string Token { get; }
        public string ErrorMessage { get; }

        public LoginResult(bool isSuccess, string userId, string token = null, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            UserId = userId;
            Token = token;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 성공 결과 생성
        /// </summary>
        public static LoginResult Success(string userId, string token = null)
            => new LoginResult(true, userId, token);

        /// <summary>
        /// 실패 결과 생성
        /// </summary>
        public static LoginResult Failed(string errorMessage)
            => new LoginResult(false, null, null, errorMessage);
    }
}
