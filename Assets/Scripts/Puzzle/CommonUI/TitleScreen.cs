using UnityEngine;
using System.Threading.Tasks;

namespace Puzzle.UI.Scene
{
    /// <summary>
    /// 타이틀 스크린
    /// </summary>
    public class TitleScreen : MonoBehaviour, IUIScene
    {
        public static string AddressableName => nameof(TitleScreen);

        public static TitleScreen Instance { get; private set; }

#region MonoBehaviour
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
        
#region IUIScene
        
        string IUIScene.Name => nameof(TitleScreen);
        public Flow.UISceneManager UISceneManager { get; set; }
        Task IUIScene.LoadAsync(object savedState)
        {
            return Task.CompletedTask;
        }

        void IUIScene.Begin()
        {
            _ = AutoLoginProcess(); // 비동기로 자동 로그인 시도
        }

        void IUIScene.Resume(object result)
        {
        }

        void IUIScene.Pause()
        {
        }

        void IUIScene.Finish()
        {
        }

        object IUIScene.GetState()
        {
            return null;
        }

        public void OnClickBackButton()
        {
            OnCLickEndButton();
        }

        #endregion

        /// <summary>
        /// IDP 로그인 팝업
        /// </summary>
        public void ShowLoginChoicePopup()
        {
            Flow.UIFlowManager.Instance.PushOverlay(typeof(Overlay.IDPChoicePopup), new Overlay.IDPChoicePopupState()
            {
                IDPList = Login.IDPPlatformSupportUtil.GetSupportedIDPs(),
                CloseCallback = RefreshUI
            });
        }

        /// <summary>
        /// 게임 시작 버튼
        /// </summary>
        public void OnClickStartButton()
        {
            if (User.Me.UserIdpBindings.Count <= 0)
            {
                ShowLoginChoicePopup();
                return;
            }
            
            Flow.UIFlowManager.Instance.SetTransition(new UITransition()
            {
                NextScene = LobbyMain.Instance,
                NextSceneType = typeof(LobbyMain),
                TransitionType = UITransitionType.Push,
                SavedState = new LobbyMainState()
            });
        }

        /// <summary>
        /// 게임 종료 버튼
        /// </summary>
        public void OnCLickEndButton()
        {
            GameManager.Instance.QuitGame();
        }
        
        /// <summary>
        /// 자동 로그인 프로세스
        /// </summary>
        private async Task AutoLoginProcess()
        {
            if (!PlayerPrefs.HasKey("auto_login_type"))
            {
                ShowLoginChoicePopup(); // 로그인 방식 선택 팝업 표시
                return;
            }

            string loginTypeStr = PlayerPrefs.GetString("auto_login_type");
            if (!System.Enum.TryParse<Login.LoginType>(loginTypeStr, out var loginType))
            {
                ShowLoginChoicePopup();
                return;
            }

            MyDebug.Log($"Auto Login Try: {loginType}");

            UIBlocker.Instance.SetEnabled();
            
            var result = await Login.LoginManager.Instance.LoginAsync(loginType);
            
            UIBlocker.Instance.SetDisabled();

            if (result.IsSuccess)
            {
                MyDebug.Log($"Auto Login Success! UserId: {result.UserId}");
                // 바로 다음 씬으로 전환하거나 로그인 완료 UI 갱신 등 처리
                PlayerPrefs.SetString("auto_login_type", loginType.ToString());
                PlayerPrefs.Save();
            }
            else
            {
                // TODO: 에러가 어떤 식으로 날지 모르니 생각좀 해볼 것 DeleteKey는
                MyDebug.LogError($"Auto Login Fail: {result.ErrorMessage}");
                PlayerPrefs.DeleteKey("auto_login_type");
                PlayerPrefs.Save();// 실패 시 저장 삭제
                ShowLoginChoicePopup();
            }
        }

        /// <summary>
        /// 로그인 상태에 따라서 UI 갱신용
        /// </summary>
        private void RefreshUI()
        {
            
        }
    }
}
