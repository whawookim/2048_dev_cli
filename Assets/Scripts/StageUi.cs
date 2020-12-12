using System;
using UnityEngine;

public class StageUi : MonoBehaviour
{
	public static StageUi Instance { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public enum GameState
	{
		Start,
		Pause,
		Clear,
		Fail
	}
	
	[SerializeField]
	private UILabel gameStatus;

	[SerializeField]
	private UILabel gameScore;

	private int totalScore = 0;

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

	public void SetGameStatus(GameState state)
	{
		switch (state)
		{
			case GameState.Start:
				gameStatus.text = "Game Start !!!";
				break;
			case GameState.Clear:
				gameStatus.text = "Game Clear !!!";
				break;
			case GameState.Pause:
				gameStatus.text = "Game Pause !!!";
				break;
			case GameState.Fail:
				gameStatus.text = "Game Fail !!!";
				break;
		}
	}

	public void OnClickRestart()
	{
		Board.Instance.RestartGame();
	}
}
