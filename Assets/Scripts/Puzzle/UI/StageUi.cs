using System;
using UnityEngine;

namespace Puzzle.UI
{
	public class StageUi : MonoBehaviour
	{
		public static StageUi Instance { get; private set; }

		[SerializeField]
		private UILabel gameStatus;

		[SerializeField]
		private UILabel gameScore;

		private int totalScore = 0;

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		private void OnEnable()
		{
			MessageSystem.Instance.Subscribe<ChangeGameStateEvent>(OnChangeGameState);
			MessageSystem.Instance.Subscribe<UpdateGameScoreEvent>(OnUpdateGameScore);
		}

		private void OnDisable()
		{
			MessageSystem.Instance.Unsubscribe<ChangeGameStateEvent>(OnChangeGameState);
			MessageSystem.Instance.Unsubscribe<UpdateGameScoreEvent>(OnUpdateGameScore);
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
			gameScore.text = $"Score : {totalScore}";
		}

		public void SetGameState(Stage.StageState state)
		{
			switch (state)
			{
				case Stage.StageState.Start:
					gameStatus.text = "Game Start !!!";
					break;
				case Stage.StageState.Clear:
					gameStatus.text = "Game Clear !!!";
					break;
				case Stage.StageState.Pause:
					gameStatus.text = "Game Pause !!!";
					break;
				case Stage.StageState.Fail:
					gameStatus.text = "Game Fail !!!";
					break;
			}
		}

		public void OnClickRestart()
		{
			GC.Collect();
			Stages.Instance.RestartGame();
		}

		public void OnClickLobby()
		{
			GC.Collect();
			Game.Instance.ChangeScene("Lobby");
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
