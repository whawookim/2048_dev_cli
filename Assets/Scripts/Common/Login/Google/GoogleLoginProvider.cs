using System.Threading.Tasks;
using Google;

namespace Login
{
    /// <summary>
    /// Google Sign-In SDK와 연동된 로그인 제공자.
    /// ID Token을 기반으로 서버 인증 및 Unbind 기능 제공.
    /// </summary>
    public class GoogleLoginProvider : ILoginProvider
    {
        public LoginType ProviderType => LoginType.Google;
        public bool IsLoggedIn => _isLoggedIn;
        private bool _isLoggedIn = false;

        private GoogleSignInConfiguration _configuration;

        public GoogleLoginProvider()
        {
            _configuration = new GoogleSignInConfiguration
            {
                WebClientId = "583449144791-gb1vajk5ne68bil5fdq4g27r2d2hv56g.apps.googleusercontent.com", // google-service.json에서 확인
                RequestIdToken = true,
                RequestEmail = true
            };
        }

        /// <summary>
        /// Google Sign-In SDK를 통해 로그인 시도 후,
        /// 서버에 ID Token 기반 인증 요청을 보냅니다.
        /// </summary>
        public async Task<LoginResult> LoginAsync()
        {
            GoogleSignIn.Configuration = _configuration;

            try
            {
                var user = await GoogleSignIn.DefaultInstance.SignIn();
                MyDebug.Log($"Google Login Success: {user.Email}, ID Token: {user.IdToken}");

                // 서버에 로그인 요청
                var request = ApiConnection.Login(nameof(LoginType.Google), user.UserId, user.IdToken);
                while (!request.IsDone)
                    await Task.Yield();

                if (request.Ok)
                {
                    _isLoggedIn = true;
                    return LoginResult.Success(User.Me.UserId, user.IdToken); // User.Me 사용
                }
                else
                {
                    MyDebug.LogError(
                        $"Google login failed: {request.Response?.error?.code}, message {request.Response?.error?.message}");
                    return LoginResult.Failed(request.Response?.error?.message ?? "Google Login Server Error");
                }
            }
            catch (System.Exception ex)
            {
                MyDebug.LogError($"Google Login Failed: {ex.Message}");
                _isLoggedIn = false;
                return LoginResult.Failed($"Google Sign-In Failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 서버에 언바인드 요청 후 Google SDK에서 로그아웃.
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                // 서버에 로그인 요청
                var request = ApiConnection.Unbind(User.Me.UserId, LoginType.Google);
                while (!request.IsDone)
                    await Task.Yield();
                
                if (request.Ok)
                {
                    MyDebug.Log("Unbind 성공");
                }
                else
                {
                    MyDebug.LogWarning(
                        $"Unbind 실패: {request.Response?.error?.code}, message {request.Response?.error?.message}");
                }
            }
            catch (System.Exception ex)
            {
                MyDebug.LogError($"Google Logout Failed: {ex.Message}");
                _isLoggedIn = false;
            }

            // 2. 로컬 상태 해제
            GoogleSignIn.DefaultInstance.SignOut();// Google IDP인 경우만
            _isLoggedIn = false;
        }
    }
}
