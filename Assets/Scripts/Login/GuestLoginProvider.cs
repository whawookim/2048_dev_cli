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

    public Task<LoginResult> LoginAsync()
    {
        if (_isLoggedIn)
            return Task.FromResult(LoginResult.Success(_guestId));

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

        _isLoggedIn = true;
        Debug.Log($"Guest login success: {_guestId}");
        return Task.FromResult(LoginResult.Success(_guestId));
    }

    public Task LogoutAsync()
    {
        _isLoggedIn = false;
        _guestId = string.Empty;
        return Task.CompletedTask;
    }
}
