using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Stage
{
    public enum StageState
    {
        Start,
        Pause,
        Clear,
        Fail
    }
    
    public class StageStatusController
    {
        /// <summary>
        /// 현재 스테이지 상태
        /// </summary>
        public StageState CurrentState { get; private set; }
        
        public int CurrentScore { get; private set; }

        public int PlayTime => 
	        (int)((EndTime ?? GameTime.Instance.CurrentTime) - StartTime).TotalSeconds;
        
        public int MovedCount { get; private set; }
        
        public DateTime StartTime { get; private set; }
        
        public DateTime? EndTime { get; private set; }
        
        public void SetScore(int score)
        {
	        CurrentScore = score;
	        
        	MessageSystem.Instance.Publish(UpdateGameScoreEvent.Create(CurrentScore));
        }

        public void AddScore(int score)
        {
	        CurrentScore += score;
	        
        	MessageSystem.Instance.Publish(UpdateGameScoreEvent.Create(CurrentScore));
        }

        public void SetMoveCount(int count)
        {
	        MovedCount = count;
        }

        public void AddMoveCount(int count)
        {
	        MovedCount += count;
        }

        public void SetStartTime(DateTime startTime)
        {
	        StartTime = startTime;
        }

        public void SetEndTime(DateTime? endTime)
        {
	        EndTime = endTime;
        }

        public void ChangeState(StageState changeState)
        {
	        CurrentState = changeState;
	        
	        MessageSystem.Instance.Publish(ChangeGameStateEvent.Create(CurrentState));
        }

        private void InitGame()
        {
	        ChangeState(StageState.Start);
	        SetScore(0);
	        SetMoveCount(0);
	        SetStartTime(GameTime.Instance.CurrentTime);
	        SetEndTime(null);
        }

        public void StartGame()
        {
        	Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsData.StageStart);

	        InitGame();
        }

        public void RestartGame()
        {
	        Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsData.StageRestart);
	        // Board는 그대로 두고 블록들만 꺼주고 비우기
	        MessageSystem.Instance.Publish(ResetBoardEvent.Create());

	        InitGame();
        }

        public void GameFail()
        {
	        Debug.Log($"Game Fail ! [Mode {GameManager.Instance.CurrentStage}. Score - {CurrentScore}, Move Count - {MovedCount}, Play Time - {PlayTime}s]");
	        
	        SetEndTime(GameTime.Instance.CurrentTime);
	        Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsData.StageFail);
	        ChangeState(StageState.Fail);
        }

        public void ClearGame()
        {
        	_ = ClearGameAsync();
        }

        private async Task ClearGameAsync()
        {
        	UI.UIBlocker.Instance.SetEnabled();
	        
	        SetEndTime(GameTime.Instance.CurrentTime);
	        Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsData.StageClear);
	        
	        Debug.Log($"Game Clear ! [Mode {GameManager.Instance.CurrentStage}. Score - {CurrentScore}, Move Count - {MovedCount}, Play Time - {PlayTime}s]");

        	var request = StageManager.Instance.ClearGameAsync(CurrentScore, PlayTime, MovedCount);
            
        	await request;

	        UI.UIBlocker.Instance.SetDisabled();
        	
        	if (request.Result)
        	{
		        ChangeState(StageState.Clear);
        		// TODO: 이펙트를 하든 결과 팝업을 띄워주든 뭔가 해주기?	
        	}
        }
    }
}