using System.Threading.Tasks;
using Google;
using UnityEngine;

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

    public async Task<LoginResult> LoginAsync()
    {
        GoogleSignIn.Configuration = _configuration;

        try
        {
            var signInTask = GoogleSignIn.DefaultInstance.SignIn();
            var user = await signInTask;

            _isLoggedIn = true;

            Debug.Log($"Google Login Success: {user.Email}, ID Token: {user.IdToken}");
            return LoginResult.Success(user.UserId, user.IdToken);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Google Login Failed: {ex.Message}");
            _isLoggedIn = false;
            return LoginResult.Failed($"Google Sign-In Failed: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        _isLoggedIn = false;
    }
}
