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

public struct LoginResult
{
    public string UserId;       // UUID or External ID
    public string Token;        // 외부 인증 토큰 (ex. Google OAuth ID Token)
    public LoginType Provider;  // 어떤 로그인 방식인지
    public bool IsLinkedAccount; // 게스트 → 연동 여부

    public LoginResult(string userId, string token, LoginType provider, bool isLinked)
    {
        UserId = userId;
        Token = token;
        Provider = provider;
        IsLinkedAccount = isLinked;
    }
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
    private static LoginManager instance;
    
    public static LoginManager Instance => instance ??= new LoginManager();
    
    private readonly Dictionary<LoginType, ILoginProvider> _providers = new();
    private readonly Dictionary<LoginType, LoginResult> _linkedAccounts = new();

    public void RegisterProvider(ILoginProvider provider)
    {
        _providers[provider.ProviderType] = provider;
    }

    public async Task<LoginResult> LoginAsync(LoginType type)
    {
        if (!_providers.TryGetValue(type, out var provider))
            throw new Exception($"{type} provider not found");

        var result = await provider.LoginAsync();

        if (!string.IsNullOrEmpty(result.UserId))
            _linkedAccounts[type] = result;

        return result;
    }

    public bool IsLinked(LoginType type) => _linkedAccounts.ContainsKey(type);

    public async Task UnlinkAsync(LoginType type)
    {
        if (!_providers.TryGetValue(type, out var provider)) return;

        await provider.LogoutAsync();
        _linkedAccounts.Remove(type);
    }

    public IReadOnlyDictionary<LoginType, LoginResult> LinkedAccounts => _linkedAccounts;
}

