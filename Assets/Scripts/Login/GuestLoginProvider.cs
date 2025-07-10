using System;
using System.Threading.Tasks;
using UnityEngine;

public class GuestLoginProvider : ILoginProvider
{
    private const string GuestUUIDKey = "guest_uuid";
    private string _cachedUUID;

    public LoginType ProviderType => LoginType.Guest;

    public bool IsLoggedIn => !string.IsNullOrEmpty(_cachedUUID);

    public async Task<LoginResult> LoginAsync()
    {
        // 이미 저장된 UUID가 있는지 확인
        if (PlayerPrefs.HasKey(GuestUUIDKey))
        {
            _cachedUUID = PlayerPrefs.GetString(GuestUUIDKey);
        }
        else
        {
            _cachedUUID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(GuestUUIDKey, _cachedUUID);
            PlayerPrefs.Save();
        }

        Debug.Log($"[GuestLogin] UUID: {_cachedUUID}");

        return await Task.FromResult(new LoginResult(
            userId: _cachedUUID,
            token: string.Empty, // 게스트는 외부 토큰 없음
            provider: LoginType.Guest,
            isLinked: false
        ));
    }

    public async Task LogoutAsync()
    {
        // 로그아웃해도 UUID는 유지 (앱 삭제 시만 초기화됨)
        await Task.CompletedTask;
    }
}
