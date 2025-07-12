using System.Threading.Tasks;
using UnityEngine;
using Google;

public class GoogleLoginUI : MonoBehaviour
{
    private GoogleSignInConfiguration configuration;

    /// <summary>
    /// Google Cloud OAuth 2.0 클라이언트 ID
    /// </summary>
    [SerializeField]
    private string webClientId = "583449144791-gb1vajk5ne68bil5fdq4g27r2d2hv56g.apps.googleusercontent.com";

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };

        GoogleSignIn.Configuration = configuration;
    }

    public void OnClickLogin()
    {
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError("Google Sign-In Failed");
        }
        else
        {
            var user = task.Result;
            Debug.Log($"Google Login Success: {user.DisplayName}, {user.IdToken}");
        }
    }
}
