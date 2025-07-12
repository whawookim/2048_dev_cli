using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Puzzle.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Puzzle
{
    /// <summary>
    /// 타이틀 스크린
    /// </summary>
    public class TitleScreen : MonoBehaviour, IUIScene
    {
        private static TitleScreen instance;

        public static TitleScreen Instance => instance;

#region MonoBehaviour
        public void Awake()
        {
            Debug.Assert(Instance == null);

            instance = this;
        }

        public void OnDestroy()
        {
            Debug.Assert(Instance == this);
            
            instance = null;
        }
#endregion
        
#region IUIScene
        
        string IUIScene.Name => nameof(TitleScreen);
        public UISceneManager UISceneManager { get; set; }
        IEnumerator IUIScene.Load(object savedState)
        {
            yield return LoginProcess();
        }

        void IUIScene.Begin()
        {
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

#endregion

        /// <summary>
        /// 로그인 프로세스
        /// </summary>
        public async Task LoginProcess()
        {
            if (!PlayerPrefs.HasKey("guest_uuid"))
            {
                // UUID 없음 → 팝업: 어떤 방식으로 로그인할지 선택
                ShowLoginChoicePopup(); // Google 로그인 or 새로 시작
            }
            else
            {
                // 게스트 로그인 시도
                var result = await LoginManager.Instance.LoginAsync(LoginType.Guest);
            }
        }

        /// <summary>
        /// IDP 로그인 팝업
        /// </summary>
        public void ShowLoginChoicePopup()
        {
            
        }

        /// <summary>
        /// 게임 시작 버튼
        /// </summary>
        public void OnClickStartButton()
        {
            UISceneManager.Instance.SetTransition(new UITransition()
            {
                NextScene = LobbyMain.Instance,
                NextSceneType = typeof(LobbyMain),
                TransitionType = UITransitionType.Push,
            });
        }

        /// <summary>
        /// 게임 종료 버튼
        /// </summary>
        public void OnCLickEndButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }
    }
}
