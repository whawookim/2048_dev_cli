using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.UI.Scene
{
    /// <summary>
    /// 스테이지 씬 진입 시 전달되는 상태 정보 (예: 난이도, 모드 등)
    /// </summary>
	public class StagesState
	{
		public StageMode CurrentStageMode { get; set; }
	}

    /// <summary>
    /// 스테이지 메인 UI 씬. 유저가 퍼즐을 플레이하는 화면에서 UI와 게임 로직을 연결함.
    /// </summary>
	public class Stages : MonoBehaviour, IUIScene
	{
		public static Stages Instance { get; private set; }
 
        public static string AddressableName => nameof(Stages);

        ///<summary> 실제 보드 UI 레이아웃 </summary>
		[SerializeField]
		private BoardUI boardUI;

        /// <summary> 상단 UI (점수 등) </summary>
		[SerializeField]
		private StageUI stageUI;

        /// 씬 진입 시 전달된 정보
		private StagesState _states;

#region MonoBehaviour
		private void Awake()
		{
			Debug.Assert(Instance == null);

			Instance = this;
		}

		private void OnDestroy()
		{
			// 데이터 날리기용
			Dispose();
					
			Debug.Assert(Instance == this);

			Instance = null;
		}
#endregion
		
#region IUIScene
        
		string IUIScene.Name => nameof(Stages);
		public Flow.UISceneManager UISceneManager { get; set; }
        
        /// <summary>
        /// 씬이 로드될 때 실행되는 비동기 초기화 로직 (프리팹 초기화, 모델 구성 등)
        /// </summary>
		Task IUIScene.LoadAsync(object savedState)
		{
			_states = savedState as StagesState;

			StageManager.Instance.StatusController.SetStageMode(_states.CurrentStageMode);
            StageManager.Instance.LoadStage(_states.CurrentStageMode, boardUI.GetBlockParent(),
                boardUI.GetBoardParent());

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 씬이 시작될 때 호출됨 (실제 게임 시작 처리)
        /// </summary>
		void IUIScene.Begin()
		{
            // 상단 UI 버튼 등 연결
			stageUI.SubscribeEvent();
            // 메시지 구독 (이동, 리셋 등)
            StageManager.Instance.BoardController.SubscribeEvents();
			StageManager.Instance.StatusController.StartGame();
		}

		void IUIScene.Resume(object result)
		{
		}

		void IUIScene.Pause()
		{
		}

        /// <summary>
        /// 씬 종료 전 호출되는 정리 함수
        /// </summary>
		void IUIScene.Finish()
		{
			stageUI.UnsubscribeEvent(true);
            StageManager.Instance.BoardController.UnsubscribeEvents(true);
		}

		object IUIScene.GetState()
		{
			return null;
		}

        public void OnClickBackButton()
        {
            stageUI.OnClickLobby();
        }

        #endregion

        /// <summary>
        /// 스테이지 씬이 파괴될 때 호출되는 정리 함수
        /// (Controller가 생성한 오브젝트 및 풀 반환)
        /// </summary>
		public void Dispose()
        {
            StageManager.Instance.Dispose();
        }
	}
}
