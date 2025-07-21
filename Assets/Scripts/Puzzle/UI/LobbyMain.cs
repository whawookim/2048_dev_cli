using System.Collections;
using UnityEngine;

namespace Puzzle.UI
{
	/// <summary>
	/// 게임의 로비 메인을 관리하는 메인 UI
	/// </summary>
	public class LobbyMain : MonoBehaviour, IUIScene
	{
		public static LobbyMain Instance { get; private set; }

		private int stageIndex = 0;

		[SerializeField]
		private StageData[] stages;
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

		// Start is called before the first frame update
		private void Start()
		{
			SetCurrentStage((int)GameManager.Instance.CurrentStage);
		}

		private void Update()
		{
		#if UNITY_EDITOR || UNITY_ANDROID
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnClickEndButton();
			}
		#endif
		}
#endregion
		
#region IUIScene
        
		string IUIScene.Name => nameof(LobbyMain);
		public UISceneManager UISceneManager { get; set; }
		IEnumerator IUIScene.Load(object savedState)
		{
			yield return AdManager.Instance.LoadAndShowBannerProcess();
		}

		void IUIScene.Begin()
		{
			
		}

		void IUIScene.Resume(object result)
		{
			AdManager.Instance.ShowBanner();
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

#endregion

		/// <summary>
		/// 현재 선택된 스테이지에 맞게 UI 업데이트
		/// </summary>
		private void SetCurrentStage(int index)
		{
			stages[stageIndex].gameObject.SetActive(false);
			stages[index].gameObject.SetActive(true);
			stageIndex = index;

			GameManager.Instance.ChangeStage(stages[stageIndex].Mode);
		}

		/// <summary>
		/// 다음 스테이지 선택
		/// </summary>
		public void OnClickRightStage()
		{
			SetCurrentStage(stageIndex + 1 >= stages.Length ? 0 : stageIndex + 1);
		}

		/// <summary>
		/// 이전 스테이지 선택
		/// </summary>
		public void OnClickLeftStage()
		{
			SetCurrentStage(stageIndex - 1 < 0 ? stages.Length - 1 : stageIndex - 1);
		}

		/// <summary>
		/// 현재 선택한 스테이지 시작
		/// </summary>
		public void OnClickStartButton()
		{
			GameManager.Instance.ChangeScene("Stage", nameof(Stages));
		}

		public void OnClickEndButton()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		public void OnClickRankingButton()
		{
			UISceneManager.Instance.PushOverlay(RankingPopup.Instance, new RankingPopupState()
			{
				StageMode = GameManager.Instance.CurrentStage
			}, typeof(RankingPopup));
		}
		
		/// <summary>
		/// IDP 로그인 팝업
		/// </summary>
		public void OnClickIDPButton()
		{
			UISceneManager.Instance.PushOverlay(IDPChoicePopup.Instance, new IDPChoicePopupState()
			{
				IDPList = IDPPlatformSupportUtil.GetSupportedIDPs()
			}, typeof(IDPChoicePopup));
		}
	}
}
