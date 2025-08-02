using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.UI.Scene
{
	public class LobbyMainState
	{
		public StageMode CurrentStageMode { get; set; } = StageMode.Stage3x3;
	}
	
	/// <summary>
	/// 게임의 로비 메인을 관리하는 메인 UI
	/// </summary>
	public class LobbyMain : MonoBehaviour, IUIScene
	{
		public static string AddressableName => nameof(LobbyMain);

		public static LobbyMain Instance { get; private set; }

		private int _stageIndex = 0;

		[SerializeField]
		private StageCardUI[] stages;
		
		private LobbyMainState _states;

#region MonoBehaviour
		void Awake()
		{
			Debug.Assert(Instance == null);

			Instance = this;
		}

		private void OnDestroy()
		{
			Debug.Assert(Instance == this);

			Instance = null;
		}
#endregion
		
#region IUIScene
        
		string IUIScene.Name => nameof(LobbyMain);
		public Flow.UISceneManager UISceneManager { get; set; }
		Task IUIScene.LoadAsync(object savedState)
		{
			_states = savedState as LobbyMainState;

            return Task.CompletedTask;
        }

        void IUIScene.Begin()
		{
			SetCurrentStage((int)_states.CurrentStageMode);
            AdManager.Instance.ShowBanner();
        }

		void IUIScene.Resume(object result)
		{
		}

		void IUIScene.Pause()
		{
            AdManager.Instance.HideBanner();
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
            OnClickEndButton();
        }

        #endregion

		/// <summary>
		/// 현재 선택된 스테이지에 맞게 UI 업데이트
		/// </summary>
		private void SetCurrentStage(int index)
		{
			stages[_stageIndex].gameObject.SetActive(false);
			stages[index].gameObject.SetActive(true);
			_stageIndex = index;

			_states.CurrentStageMode = stages[_stageIndex].Mode;
		}

		/// <summary>
		/// 다음 스테이지 선택
		/// </summary>
		public void OnClickRightStage()
		{
			SetCurrentStage(_stageIndex + 1 >= stages.Length ? 0 : _stageIndex + 1);
		}

		/// <summary>
		/// 이전 스테이지 선택
		/// </summary>
		public void OnClickLeftStage()
		{
			SetCurrentStage(_stageIndex - 1 < 0 ? stages.Length - 1 : _stageIndex - 1);
		}

		/// <summary>
		/// 현재 선택한 스테이지 시작
		/// </summary>
		public void OnClickStartButton()
		{
			GameManager.Instance.ChangeScene(UnityScene.Stage, new UITransition()
			{
				NextScene = Stages.Instance,
				NextSceneType = typeof(Stages),
				TransitionType = UITransitionType.Push,
				SavedState = new StagesState()
				{
					CurrentStageMode = _states.CurrentStageMode,
				}
			});
		}

		public void OnClickEndButton()
		{
			GameManager.Instance.QuitGame();
		}

		public void OnClickRankingButton()
        {
            Flow.UIFlowManager.Instance.PushOverlay(typeof(Overlay.RankingPopup), new Overlay.RankingPopupState()
            {
                StageMode = _states.CurrentStageMode
            });
        }
		
		/// <summary>
		/// IDP 로그인 팝업
		/// </summary>
		public void OnClickIDPButton()
		{
            Flow.UIFlowManager.Instance.PushOverlay(typeof(Overlay.IDPChoicePopup), new Overlay.IDPChoicePopupState()
			{
				IDPList = Login.IDPPlatformSupportUtil.GetSupportedIDPs()
			});
		}
	}
}
