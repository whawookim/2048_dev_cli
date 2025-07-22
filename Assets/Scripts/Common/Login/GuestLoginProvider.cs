using System;
using System.Threading.Tasks;
using UnityEngine;

public class GuestLoginProvider : ILoginProvider
{
    public LoginType ProviderType => LoginType.Guest;
    public bool IsLoggedIn => _isLoggedIn;

    private bool _isLoggedIn;
    private string _guestId;

    private const string PlayerPrefsKey = "guest_uuid";

    public async Task<LoginResult> LoginAsync()
    {
        if (_isLoggedIn)
            return LoginResult.Success(_guestId);

        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            _guestId = PlayerPrefs.GetString(PlayerPrefsKey);
        }
        else
        {
            _guestId = SystemInfo.deviceUniqueIdentifier + "_" + Guid.NewGuid().ToString("N");
            PlayerPrefs.SetString(PlayerPrefsKey, _guestId);
            PlayerPrefs.Save();
        }
        
        // 서버로 로그인 요청
        var request = ApiConnection.Login(nameof(LoginType.Guest), _guestId, null);
        while (!request.IsDone)
            await Task.Yield();

        if (request.Ok)
        { 
            _isLoggedIn = true;
            return LoginResult.Success(User.Me.UserId); // User.Me 사용
        }
        else
        {
            MyDebug.LogError(
                $"Guest login failed: {request.Response?.error?.code}, message {request.Response?.error?.message}");
            return LoginResult.Failed(request.Response?.error?.message ?? "Unknown Error");
        }
    }

    public Task LogoutAsync()
    {
        _isLoggedIn = false;
        _guestId = string.Empty;
        return Task.CompletedTask;
    }
}
