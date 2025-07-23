using System.Threading.Tasks;

namespace Login
{
    /// <summary>
    /// 로그인 제공자 인터페이스. Guest, Google 등에서 구현.
    /// </summary>
    public interface ILoginProvider
    {
        LoginType ProviderType { get; }
        bool IsLoggedIn { get; }

        Task<LoginResult> LoginAsync();
        Task LogoutAsync();
    }
}
