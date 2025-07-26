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
 
        public string AddressableName => nameof(Stages);

        ///<summary> 실제 보드 UI 레이아웃 </summary>
		[SerializeField]
		private BoardUI boardUI;

        /// <summary> 상단 UI (점수 등) </summary>
		[SerializeField]
		private StageUI stageUI;

        /// 씬 진입 시 전달된 정보
		private StagesState _states;
        
        /// 게임 제어 책임 객체 (로직)
        private Stage.StageBoardController _controller;

        /// 게임 상태 정보 (2차원 배열 등)
        private Stage.StageBoardModel _model;

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

            return LoadStage();
		}
        
        /// <summary>
        /// 씬이 시작될 때 호출됨 (실제 게임 시작 처리)
        /// </summary>
		void IUIScene.Begin()
		{
            // 상단 UI 버튼 등 연결
			stageUI.SubscribeEvent();
            // 메시지 구독 (이동, 리셋 등)
            _controller.SubscribeEvents();
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
            _controller.UnsubscribeEvents(true);
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
            _controller?.Dispose();
            _controller = null;
        }
		
		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(Vector2Int pos)
		{
			return _controller.GetBoardPosition(pos);
		}
        
        /// <summary>
        /// 현재 StageMode에 맞춰 보드와 블록을 세팅하고 게임판을 초기화
        /// </summary>
        public async Task LoadStage()
        {
            var mode = _states.CurrentStageMode;
            
            var board = StageManager.Instance.OriginBoardObj;
            var block = StageManager.Instance.OriginBlockObj;
            
            var maxSize = mode.GetBoardSize();
            var maxNum = mode.GetBlockMaxNum();

            _model = new Stage.StageBoardModel(maxSize, maxNum);

            var blockFactory = new ObjectPoolFactory<Block>(
                block,
                boardUI.GetBlockParent(),
                maxSize * maxSize,
                b => {
                    b.gameObject.SetActive(true);
                    b.transform.localScale = Vector3.one;
                });

            var boardFactory = new ObjectPoolFactory<Board>(
                board,
                boardUI.GetBoardParent(),
                maxSize * maxSize,
                b => {
                    b.gameObject.SetActive(true);
                    b.transform.localScale = Vector3.one;
                });

            _controller = new Stage.StageBoardController(mode, _model, boardUI, blockFactory, boardFactory);
            _controller.Initialize();
        }
	}
}
