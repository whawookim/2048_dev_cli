using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Puzzle.UI
{
    /// <summary>
    /// 스테이지 UI (상단 상태 텍스트 및 점수 출력, 재시작/로비 버튼 포함)
    /// 게임 진행 상태 및 점수를 표시하며, 관련 이벤트를 수신함
    /// </summary>
	public class StageUI : MonoBehaviour
	{
        /// <summary> 상태 텍스트 출력 (시작, 클리어 등) </summary>
		[SerializeField]
		private TextMeshProUGUI gameStatus;

        /// <summary> 현재 점수 출력 </summary>
		[SerializeField]
		private TextMeshProUGUI gameScore;
		
        /// 다국어 상태 문자열 정의
		private readonly LocalizedString _statusStart = new(GameStringsManager.DefaultTable, "status_start");
		private readonly LocalizedString _statusClear = new(GameStringsManager.DefaultTable, "status_clear");
		private readonly LocalizedString _statusPause = new(GameStringsManager.DefaultTable, "status_pause");
		private readonly LocalizedString _statusFail  = new(GameStringsManager.DefaultTable, "status_fail");
		
        /// 현재 표시 중인 상태 문자열
        private LocalizedString _currentStatus;
		
        /// 점수 문자열 형식 지정용 (예: Score: {score})
		private readonly LocalizedString _localizedScore = new(GameStringsManager.DefaultTable, "score_display");

        /// <summary>
        /// 메시지 시스템 이벤트 구독 시작 (게임 상태 및 점수 변경)
        /// </summary>
		public void SubscribeEvent()
		{
			MessageSystem.Instance.Subscribe<ChangeGameStateEvent>(OnChangeGameState);
			MessageSystem.Instance.Subscribe<UpdateGameScoreEvent>(OnUpdateGameScore);
		}
        
        /// <summary>
        /// 메시지 시스템 이벤트 구독 해제
        /// </summary>
		public void UnsubscribeEvent(bool deleteKey = false)
		{
			MessageSystem.Instance.Unsubscribe<ChangeGameStateEvent>(OnChangeGameState, deleteKey);
			MessageSystem.Instance.Unsubscribe<UpdateGameScoreEvent>(OnUpdateGameScore, deleteKey);
		}

        /// <summary>
        /// 외부에서 전달된 점수 값을 출력용으로 갱신
        /// </summary>
		public void UpdateGameScoreText(int totalScore)
		{
			// 값 전달용 변수 바인딩
			_localizedScore.Arguments = new object[] { new { score = totalScore } };

			_localizedScore.StringChanged -= UpdateScoreText;
			_localizedScore.StringChanged += UpdateScoreText;
			_localizedScore.RefreshString();
		}

        /// <summary>
        /// 점수 문자열이 실제로 갱신되었을 때 텍스트에 반영
        /// </summary>
		private void UpdateScoreText(string localizedText)
		{
			gameScore.text = localizedText;
		}

        /// <summary>
        /// 게임 상태가 변경되었을 때 호출됨
        /// -> 로컬라이징 상태 텍스트 변경
        /// </summary>
		public void SetGameState(Stage.StageState state)
		{
			switch (state)
			{
				case Stage.StageState.Playing:
					_currentStatus = _statusStart;
					break;
				case Stage.StageState.Clear:
					_currentStatus = _statusClear;
					break;
				case Stage.StageState.Pause:
					_currentStatus = _statusPause;
					break;
				case Stage.StageState.Fail:
					_currentStatus = _statusFail;
					break;
			}

			// 이벤트 연결 제거 → 재연결
			_currentStatus.StringChanged -= OnStatusChanged;
			_currentStatus.StringChanged += OnStatusChanged;

			// 수동 갱신
			_currentStatus.RefreshString(); 
		}
		
        /// <summary>
        /// 상태 문자열이 실제로 갱신되었을 때 텍스트에 반영
        /// </summary>
		private void OnStatusChanged(string localizedValue)
		{
			gameStatus.text = localizedValue;
		}
        
        /// <summary>
        /// 재시작 버튼 클릭 시 게임 재시작
        /// </summary>
		public void OnClickRestart()
		{
			StageManager.Instance.RestartGame();
		}

        /// <summary>
        /// 로비 버튼 클릭 시 로비로 이동
        /// </summary>
		public void OnClickLobby()
		{
			StageManager.Instance.GoToLobby();
		}

        /// <summary>
        /// 행동 되돌리기 클릭
        /// </summary>
        public void OnClickUndo()
        {
            if (!Stage.UndoHistory.CanUndo)
            {
                Debug.Log("No move to undo.");
                return;
            }
            
            StageManager.Instance.UndoLastCommand();
        }
        
        /// <summary>
        /// 게임 상태 변경 이벤트 처리
        /// </summary>
		private bool OnChangeGameState(Events e)
		{
			if (e is ChangeGameStateEvent cgse)
			{
				SetGameState(cgse.State);
				return true;
			}

			return false;
		}

        /// <summary>
        /// 점수 갱신 이벤트 처리
        /// </summary>
		private bool OnUpdateGameScore(Events e)
		{
			if (e is UpdateGameScoreEvent ugse)
			{
				UpdateGameScoreText(ugse.Value);
				return true;
			}

			return false;
		}
	}
}
