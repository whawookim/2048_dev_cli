using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum LoginType
{
    Guest,
    Google,
    Apple,
    // 추가 가능
}

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

    public static LoginResult Success(string userId, string token = null)
        => new LoginResult(true, userId, token);

    public static LoginResult Failed(string errorMessage)
        => new LoginResult(false, null, null, errorMessage);
}

public interface ILoginProvider
{
    LoginType ProviderType { get; }
    bool IsLoggedIn { get; }

    Task<LoginResult> LoginAsync();
    Task LogoutAsync();
}

/// <summary>
/// 로그인 관련 매니저 싱글턴 클래스
/// </summary>
public class LoginManager
{
    public static LoginManager Instance { get; } = new LoginManager();

    private readonly Dictionary<LoginType, ILoginProvider> _providers = new();

    private LoginManager()
    {
        RegisterProviders();
    }

    public void RegisterProviders()
    {
        var availableIdps = IDPPlatformSupportUtil.GetSupportedIDPs();

        foreach (var idp in availableIdps)
        {
            switch (idp)
            {
                case LoginType.Guest:
                    RegisterProvider(new GuestLoginProvider());
                    break;
                case LoginType.Google:
                    RegisterProvider(new GoogleLoginProvider());
                    break;
            }
        }
    }

    /// <summary>
    /// 로그인 제공자 등록
    /// </summary>
    public void RegisterProvider(ILoginProvider provider)
    {
        if (!_providers.ContainsKey(provider.ProviderType))
        {
            _providers.Add(provider.ProviderType, provider);
        }
    }

    /// <summary>
    /// 특정 로그인 제공자에 대해 로그인 시도
    /// </summary>
    public async Task<LoginResult> LoginAsync(LoginType loginType)
    {
        if (!_providers.TryGetValue(loginType, out var provider))
        {
            MyDebug.LogError($"Login provider not registered: {loginType}");
            return LoginResult.Failed($"Provider {loginType} not registered.");
        }

        return await provider.LoginAsync();
    }

    /// <summary>
    /// 특정 provider의 로그인 여부
    /// </summary>
    public bool IsLoggedIn(LoginType loginType)
    {
        return _providers.TryGetValue(loginType, out var provider) && provider.IsLoggedIn;
    }

    /// <summary>
    /// 전체 provider 중 하나라도 로그인되어 있는지 확인
    /// </summary>
    public bool IsAnyLoggedIn()
    {
        foreach (var provider in _providers.Values)
        {
            if (provider.IsLoggedIn)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 모든 provider 로그아웃 (강제 로그아웃)
    /// </summary>
    public async Task LogoutAllAsync()
    {
        foreach (var provider in _providers.Values)
        {
            if (provider.IsLoggedIn)
            {
                await provider.LogoutAsync();
            }
        }
    }

    /// <summary>
    /// 특정 provider 로그아웃
    /// </summary>
    public async Task LogoutAsync(LoginType loginType)
    {
        if (_providers.TryGetValue(loginType, out var provider) && provider.IsLoggedIn)
        {
            await provider.LogoutAsync();
        }
    }
}
