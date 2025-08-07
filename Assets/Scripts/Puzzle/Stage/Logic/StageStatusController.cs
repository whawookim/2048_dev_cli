using System;
using System.Threading.Tasks;

namespace Puzzle.Stage
{
    /// <summary>
    /// 퍼즐 스테이지의 진행 상태, 점수, 이동 횟수, 타이머 등을 관리하는 컨트롤러.
    /// 게임 로직에서 상태 관리 및 점수 UI 반영 등을 위해 사용됨.
    /// </summary>
    public enum StageState
    {
        Playing,
        Pause,
        Clear,
        Fail
    }
    
    public class StageStatusController
    {
        /// <summary>
        /// 현재 스테이지 모드 (예: Normal, TimeAttack 등)
        /// </summary>
	    public StageMode CurrentStageMode { get; private set; }
	    
        /// <summary>
        /// 현재 스테이지 상태
        /// </summary>
        public StageState CurrentState { get; private set; }

        /// <summary>
        /// 현재 점수
        /// </summary>
        public int CurrentScore { get; private set; }

        /// <summary>
        /// 스테이지 시작 이후 현재까지 경과한 시간 (초)
        /// EndTime이 설정되어 있으면 해당 시간까지, 아니면 현재 시간 기준
        /// </summary>
        public int PlayTime => 
	        (int)((EndTime ?? GameTime.Instance.CurrentTime) - StartTime).TotalSeconds;
        
        /// <summary>
        /// 총 이동 횟수
        /// </summary>
        public int MovedCount { get; private set; }
        
        /// <summary>
        /// 스테이지 시작 시간
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 스테이지 종료 시간 (없을 수 있음)
        /// </summary>
        public DateTime? EndTime { get; private set; }
        
        /// <summary>
        /// 현재 게임 상태가 플레이 중인지
        /// </summary>
        public bool CanPlaying => CurrentState == StageState.Playing;

        /// <summary>
        /// 스테이지 모드 설정
        /// </summary>
        public void SetStageMode(StageMode mode)
        {
	        CurrentStageMode = mode;
        }
        
        /// <summary>
        /// 현재 점수 설정 및 점수 업데이트 이벤트 발행
        /// </summary>
        public void SetScore(int score)
        {
	        CurrentScore = score;
        	MessageSystem.Instance.Publish(UpdateGameScoreEvent.Create(CurrentScore));
        }

        /// <summary>
        /// 점수 누적 및 업데이트 이벤트 발행
        /// </summary>
        public void AddScore(int score)
        {
	        CurrentScore += score;
        	MessageSystem.Instance.Publish(UpdateGameScoreEvent.Create(CurrentScore));
        }

        /// <summary>
        /// 이동 횟수 설정
        /// </summary>
        public void SetMoveCount(int count)
        {
	        MovedCount = count;
        }

        /// <summary>
        /// 이동 횟수 누적
        /// </summary>
        public void AddMoveCount(int count)
        {
	        MovedCount += count;
        }

        /// <summary>
        /// 스테이지 시작 시간 설정
        /// </summary>
        public void SetStartTime(DateTime startTime)
        {
	        StartTime = startTime;
        }

        /// <summary>
        /// 스테이지 종료 시간 설정
        /// </summary>
        public void SetEndTime(DateTime? endTime)
        {
	        EndTime = endTime;
        }

        /// <summary>
        /// 스테이지 상태 설정 및 상태 변경 이벤트 발행
        /// </summary>
        public void ChangeState(StageState changeState)
        {
	        CurrentState = changeState;
	        
	        MessageSystem.Instance.Publish(ChangeGameStateEvent.Create(CurrentState));
        }

        private void InitGame()
        {
	        ChangeState(StageState.Playing);
	        SetScore(0);
	        SetMoveCount(0);
	        SetStartTime(GameTime.Instance.CurrentTime);
	        SetEndTime(null);
        }

        public void StartGame()
        {
        	Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsConstants.StageStart);

	        InitGame();
        }

        public void RestartGame()
        {
	        Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsConstants.StageRestart);
	        // Board는 그대로 두고 블록들만 꺼주고 비우기
	        MessageSystem.Instance.Publish(ResetBoardEvent.Create());

	        InitGame();
        }

        public void GameFail()
        {
	        MyDebug.Log($"Game Fail ! [Mode {CurrentStageMode}. Score - {CurrentScore}, Move Count - {MovedCount}, Play Time - {PlayTime}s]");
	        
	        SetEndTime(GameTime.Instance.CurrentTime);
	        Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsConstants.StageFail);
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
	        Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsConstants.StageClear);
	        
	        MyDebug.Log($"Game Clear ! [Mode {CurrentStageMode}. Score - {CurrentScore}, Move Count - {MovedCount}, Play Time - {PlayTime}s]");

        	var request = StageManager.Instance.ClearGameAsync(CurrentStageMode, PlayTime, MovedCount);
            
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
