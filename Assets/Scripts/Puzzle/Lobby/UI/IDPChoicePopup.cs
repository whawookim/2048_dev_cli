using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    public class IDPChoicePopupState
    {
        /// <summary>
        /// 표시하길 원하는 로그인 타입 리스트
        /// </summary>
        public List<LoginType> IDPList { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Action CloseCallback { get; set; }
    }
    
    /// <summary>
    /// 로그인용 IDP 선택 팝업
    /// </summary>
    public class IDPChoicePopup : MonoBehaviour, IUIOverlay
    {
        public static IDPChoicePopup Instance { get; private set; }

        public static string AddressableName => nameof(IDPChoicePopup);

        [SerializeField]
        private List<IDPChoiceButton> allIdpButtons;
        
        [SerializeField]
        private HorizontalLayoutGroup layoutGroup;

        private IDPChoicePopupState states;

        #region Monobehavior

        public void Awake()
        {
            Debug.Assert(Instance == null);
            
            Instance = this;
        }

        public void OnDestroy()
        {
            Debug.Assert(Instance == this);
            
            Instance = null;
        }

        #endregion
        
        #region IUIOverlay
        public string Name => nameof(RankingPopup);

        public UISceneManager UISceneManager { get; set; }

        public void Begin(object state = null)
        {
            states = state as IDPChoicePopupState;
            
            Debug.Assert(states != null);

            RefreshUI();
        }

        public IEnumerator OpenAnimation()
        {
            yield break;
        }

        public IEnumerator CloseAnimation()
        {
            yield break;
        }

        public void OnClickBackButton()
        {
            UISceneManager.Instance.PopOverlay();
        }

        public void Finish()
        {
        }
        #endregion
        
        /// <summary>
        /// 팝업 UI 갱신
        /// </summary>
        private void RefreshUI()
        {
            var boundIdps = User.Me.UserIdpBindings; // 서버 응답 기반 캐싱
            var availableIdps = states.IDPList;

            foreach (var idp in allIdpButtons)
            {
                idp.SetActive(availableIdps.Contains(idp.LoginType));
                idp.Init(LoginOrLogoutProcess);

                bool isBound = boundIdps.Contains(idp.LoginType);
                idp.SetVisualState(isBound);
            }
        }

        /// <summary>
        /// 버튼 콜백으로 LoginType에 따라서 Login인지 LogOut인지 제어
        /// </summary>
        private void LoginOrLogoutProcess(LoginType loginType)
        {
            var idpBindings = User.Me.UserIdpBindings;

            // Logout or Unbound 처리
            if (idpBindings != null && idpBindings.Contains(loginType))
            {
                if (idpBindings.Count == 1)
                {
                    MyDebug.Log("User IDP Binding need count more than 1");
                    return;
                }

                _ = LogoutProcessAsync(loginType);
            }
            // Login or Bound 처리
            else
            {
                _ = LoginProcessAsync(loginType);
            }
        }

        private async Task LogoutProcessAsync(LoginType loginType)
        {
            UIBlocker.Instance.SetEnabled();
            
            await LoginManager.Instance.LogoutAsync(loginType);

            UIBlocker.Instance.SetDisabled();

            // 각 Provider 부분에 상태 변경됨.
            bool isSuccess = !LoginManager.Instance.IsLoggedIn(loginType);
                
            if (isSuccess)
            {
                MyDebug.Log($"Logout Success! : {loginType}");
                RefreshUI();

                var autoLoginType = PlayerPrefs.GetString("auto_login_type");

                if (autoLoginType == loginType.ToString())
                {
                    var idpBindings = User.Me.UserIdpBindings;

                    if (idpBindings != null && idpBindings.Count > 0)
                    {
                        PlayerPrefs.SetString("auto_login_type", idpBindings[0].ToString());
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        MyDebug.LogError("User IDP Binding is empty");
                    }
                }
            }
            else
            {
                MyDebug.LogError($"Logout Failed! : {loginType}");
            }
        }

        private async Task LoginProcessAsync(LoginType loginType)
        {
            UIBlocker.Instance.SetEnabled();
            
            var result = await LoginManager.Instance.LoginAsync(loginType);
            
            UIBlocker.Instance.SetDisabled();
            
            // 각 Provider 부분에 상태 변경됨.
            bool isSuccess = LoginManager.Instance.IsLoggedIn(loginType);
                
            if (isSuccess)
            {
                MyDebug.Log($"Login Success! UserId: {result.UserId}, LoginType: {loginType}");
                RefreshUI();

                PlayerPrefs.SetString("auto_login_type", loginType.ToString());
                PlayerPrefs.Save();
            }
            else
            {
                MyDebug.LogError($"Login Failed! : {loginType}");
            }
        }
    }
}