using System.Collections.Generic;
using System.Threading.Tasks;

namespace Login
{
    /// <summary>
    /// 지원되는 로그인 방식 열거형. 각 로그인 제공자에 대응.
    /// </summary>
    public enum LoginType
    {
        Guest,
        Google,
        Apple,
        // 추가 가능
    }
    
    /// <summary>
    /// 로그인 전반을 관리하는 매니저 클래스 (싱글턴)
    /// 다양한 로그인 제공자 등록/실행/상태 확인/로그아웃을 지원.
    /// </summary>
    public class LoginManager
    {
        public static LoginManager Instance { get; } = new LoginManager();

        private readonly Dictionary<LoginType, ILoginProvider> _providers = new();

        private LoginManager()
        {
            RegisterProviders();
        }

        /// <summary>
        /// 플랫폼에서 지원하는 IDP를 등록
        /// </summary>
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
}
