using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Login
{
    /// <summary>
    /// 게스트 로그인 제공자. 고유 UUID를 기반으로 서버에 로그인 요청을 보냅니다.
    /// PlayerPrefs를 통해 UUID를 저장/재사용하며, 서버 연동은 ApiConnection을 통해 수행됩니다.
    /// </summary>
     public class GuestLoginProvider : ILoginProvider
     {
         public LoginType ProviderType => LoginType.Guest;
         public bool IsLoggedIn => _isLoggedIn;
     
         private bool _isLoggedIn;
         private string _guestId;
     
         private const string PlayerPrefsKey = "guest_uuid";
     
         /// <summary>
         /// 비동기 로그인 수행. UUID를 생성 또는 불러오고 서버에 로그인 요청을 보냅니다.
         /// </summary>
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
     
         /// <summary>
         /// 로컬 상태만 초기화. 서버에는 별도 요청하지 않음.
         /// </summary>
         public Task LogoutAsync()
         {
             _isLoggedIn = false;
             _guestId = string.Empty;
             return Task.CompletedTask;
         }
     }
}
