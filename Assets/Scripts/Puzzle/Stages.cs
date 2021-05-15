using Puzzle.Stage;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Puzzle
{
	public enum StageMode
	{
		Stage3x3 = 0,
		Stage4x4,
		Staeg5x5
	}

	public static class StageModeExtension
	{
		/// <summary>
		/// 보드의 가로 세로 사이즈
		/// </summary>
		public static int GetBoardSize(this StageMode mode)
		{
			switch (mode)
			{
				case StageMode.Stage3x3:
					return 3;
				case StageMode.Stage4x4:
					return 4;
				case StageMode.Staeg5x5:
					return 5;
			}

			return 4;
		}

		public static int GetBlockSize(this StageMode mode)
		{
			var modeIndex = (int) mode;
			var blockSizes = Constants.BoardSizes;

			if (blockSizes.Length <= modeIndex || modeIndex < 0) return 0;

			return blockSizes[modeIndex];
		}

		public static int GetGridSize(this StageMode mode)
		{
			var modeIndex = (int) mode;
			var gridSizes = Constants.GridSizes;

			if (gridSizes.Length <= modeIndex || modeIndex < 0) return 0;

			return gridSizes[modeIndex];
		}
	}

	public class Stages : MonoBehaviour
	{
		public static Stages Instance { get; private set; }

		[FormerlySerializedAs("board")] [SerializeField]
		private BoardManager boardManager;

		private void Awake()
		{
			Debug.Assert(Instance == null);

			Instance = this;
		}

		private void OnDestroy()
		{
			Debug.Assert(Instance != null);

			Instance = null;
		}

		public void Start()
		{
			StartGame();
		}

		public void SetScore(int score)
		{
			MessageSystem.Instance.Publish(UpdateGameScoreEvent.Create(UpdateGameScoreType.Set, score));
		}

		public void AddScore(int score)
		{
			MessageSystem.Instance.Publish(UpdateGameScoreEvent.Create(UpdateGameScoreType.Add, score));
		}

		public void StartGame()
		{
			MessageSystem.Instance.Publish(ChangeGameStateEvent.Create(StageState.Start));
			SetScore(0);

			boardManager.SetBoard();
			boardManager.CreateBlock();
		}

		public void RestartGame()
		{
			StartGame();
		}

		public void EndGame()
		{
			MessageSystem.Instance.Publish(ChangeGameStateEvent.Create(StageState.Fail));
		}

		public void ClearGame()
		{
			MessageSystem.Instance.Publish(ChangeGameStateEvent.Create(StageState.Clear));
		}

		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(int index)
		{
			return boardManager.GetBoardPosition(index);
		}
	}
}
