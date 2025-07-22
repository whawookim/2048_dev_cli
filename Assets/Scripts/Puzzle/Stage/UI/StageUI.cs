using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Puzzle.UI
{
	public class StageUI : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI gameStatus;

		[SerializeField]
		private TextMeshProUGUI gameScore;
		
		private readonly LocalizedString status_start = new("GameStrings", "status_start");
		private readonly LocalizedString status_clear = new("GameStrings", "status_clear");
		private readonly LocalizedString status_pause = new("GameStrings", "status_pause");
		private readonly LocalizedString status_fail  = new("GameStrings", "status_fail");
		private LocalizedString currentStatus;
		
		private LocalizedString localizedScore = new("GameStrings", "score_display");

		public void SubscribeEvent()
		{
			MessageSystem.Instance.Subscribe<ChangeGameStateEvent>(OnChangeGameState);
			MessageSystem.Instance.Subscribe<UpdateGameScoreEvent>(OnUpdateGameScore);
		}

		public void UnsubscribeEvent(bool deleteKey = false)
		{
			MessageSystem.Instance.Unsubscribe<ChangeGameStateEvent>(OnChangeGameState, deleteKey);
			MessageSystem.Instance.Unsubscribe<UpdateGameScoreEvent>(OnUpdateGameScore, deleteKey);
		}

		public void UpdateGameScoreText(int totalScore)
		{
			// 값 전달용 변수 바인딩
			localizedScore.Arguments = new object[] { new { score = totalScore } };

			localizedScore.StringChanged -= UpdateScoreText;
			localizedScore.StringChanged += UpdateScoreText;
			localizedScore.RefreshString();
		}

		private void UpdateScoreText(string localizedText)
		{
			gameScore.text = localizedText;
		}

		public void SetGameState(Stage.StageState state)
		{
			switch (state)
			{
				case Stage.StageState.Start:
					currentStatus = status_start;
					break;
				case Stage.StageState.Clear:
					currentStatus = status_clear;
					break;
				case Stage.StageState.Pause:
					currentStatus = status_pause;
					break;
				case Stage.StageState.Fail:
					currentStatus = status_fail;
					break;
			}

			// 이벤트 연결 제거 → 재연결
			currentStatus.StringChanged -= OnStatusChanged;
			currentStatus.StringChanged += OnStatusChanged;

			// 수동 갱신
			currentStatus.RefreshString(); 
		}
		
		private void OnStatusChanged(string localizedValue)
		{
			gameStatus.text = localizedValue;
		}

		public void OnClickRestart()
		{
			StageManager.Instance.RestartGame();
		}

		public void OnClickLobby()
		{
			StageManager.Instance.GoToLobby();
		}

		private bool OnChangeGameState(Events e)
		{
			if (e is ChangeGameStateEvent cgse)
			{
				SetGameState(cgse.State);

				return true;
			}

			return false;
		}

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
