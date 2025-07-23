using System;

namespace Puzzle.UI.Scene
{
    public enum UITransitionType
    {
        /// <summary>
        /// prevScene.Pause();
        /// yield return nextScene.Load(savedState);
        /// nextScene.Begin();
        /// nextScene.Resume();
        /// // 나중에 prevScene이 제거되거나 다른 곳에서 다시 사용될 경우 스테이트 저장하고 종료될 수 있음
        /// stack.Add(prevScene.GetState());
        /// prevScene.Finish();
        /// </summary>
        Push,
        /// <summary>
        /// currentScene.Pause();
        /// currentScene.Finish();
        /// if (이전 씬이 아직 Finish 되지 않은 경우)
        /// {
        /// 	prevScene.Resume();
        /// }
        /// else
        /// {
        /// 	prevScene.Load(savedState);
        /// 	prevScene.Begin();
        /// 	prevScene.Resume();
        /// }
        /// </summary>
        Pop,
        // TODO:
        Replace
    }
    
    /// <summary>
    /// 트랜지션 애니메이션 종류 (슬라이드, 페이드 등)
    /// </summary>
    public enum TransitionAnimationType
    {
        None,
        Fade,
        SlideLeft,
        SlideRight
        // 추가 가능
    }
    
    /// <summary>
	/// 트랜지션
	/// </summary>
	public class UITransition
	{
		/// <summary>
		/// 트랜지션 타입(바꾸기, 푸시, 팝)
		/// </summary>
		public UITransitionType TransitionType { get; set; }
        
        /// <summary>
        /// 연출 타입
        /// </summary>
        public TransitionAnimationType AnimationType { get; set; } = TransitionAnimationType.None;

		/// <summary>
		/// 다음에 나타낼 씬(팝에서는 사용할 수 없음, 다른 경우에도 null 가능)
		/// </summary>
		public IUIScene NextScene { get; set; }

		public Type NextSceneType { get; set; }

		/// <summary>
		/// IScene.Load()에 넣을 정보(팝에서는 사용할 수 없음, 다른 경우에도 null 가능)
		/// </summary>
		public object SavedState { get; set; }

		/// <summary>
		/// IScene.Resume()에 넣을 결과 정보(팝에서만 사용 가능, 결과 없으면 null)
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// 트랜지션 끝날 때 실행할 이벤트
		/// </summary>
		public event Action TransitionEnded;

		/// <summary>
		/// 트랜지션 끝날 때 실행할 이벤트 지금 실행
		/// </summary>
		/// <remarks>ISceneManager에서만 사용할 것</remarks>
		internal void RaiseTransitionEnded()
		{
			TransitionEnded?.Invoke();
		}
		
		/// <summary>
		/// FadeOut 되는 시간. null 이라면 기본값을 사용
		/// </summary>
		public float? FadeOutDuration { get; set; } = null;

		/// <summary>
		/// FadeIn 되는 시간. null 이라면 기본값을 사용
		/// </summary>
		public float? FadeInDuration { get; set; } = null;

		/// <summary>
		/// 페이드 시에 적용될 색상, null 이라면 기본값 (Color.black 을 사용)
		/// </summary>
		public UnityEngine.Color? FadeColor { get; set; } = null;
	}
}
