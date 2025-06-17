using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Puzzle.UI
{
	public class StageUi : MonoBehaviour
	{
		public static StageUi Instance { get; private set; }

		[SerializeField]
		private TextMeshProUGUI gameStatus;

		[SerializeField]
		private TextMeshProUGUI gameScore;

		private int totalScore = 0;
		
		private readonly LocalizedString status_start = new("GameStrings", "status_start");
		private readonly LocalizedString status_clear = new("GameStrings", "status_clear");
		private readonly LocalizedString status_pause = new("GameStrings", "status_pause");
		private readonly LocalizedString status_fail  = new("GameStrings", "status_fail");
		private LocalizedString currentStatus;
		
		private LocalizedString localizedScore = new("GameStrings", "score_display");
		
		private void Awake()
		{
			Instance = this;

			SubscribeEvent();
		}

		private void OnDestroy()
		{
			Instance = null;
			
			UnsubscribeEvent(true);
		}

		private void SubscribeEvent()
		{
			MessageSystem.Instance.Subscribe<ChangeGameStateEvent>(OnChangeGameState);
			MessageSystem.Instance.Subscribe<UpdateGameScoreEvent>(OnUpdateGameScore);
		}

		private void UnsubscribeEvent(bool deleteKey = false)
		{
			MessageSystem.Instance.Unsubscribe<ChangeGameStateEvent>(OnChangeGameState, deleteKey);
			MessageSystem.Instance.Unsubscribe<UpdateGameScoreEvent>(OnUpdateGameScore, deleteKey);
		}

		private void Update()
		{
#if UNITY_EDITOR || UNITY_ANDROID
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnClickLobby();
			}
#endif
		}

		public void SetGameScore(int score)
		{
			totalScore = score;
			UpdateGameScore();
		}

		public void AddGameScore(int score)
		{
			totalScore += score;
			UpdateGameScore();
		}

		public void UpdateGameScore()
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
			GC.Collect();
			Stages.Instance.RestartGame();
		}

		public void OnClickLobby()
		{
			GC.Collect();
			GameManager.Instance.ChangeScene("Lobby");
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
				switch (ugse.Type)
				{
					case UpdateGameScoreType.Add:
						AddGameScore(ugse.Value);
						break;
					case UpdateGameScoreType.Set:
						SetGameScore(ugse.Value);
						break;
				}
				return true;
			}

			return false;
		}
	}
}
