using System;
using System.Collections;

namespace Puzzle.UI
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
        Pop
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

    public interface IUIScene
    {
        /// <summary>
        /// 씬 이름
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 이 씬을 관리 중인 매니저 얻기/설정
        /// </summary>
        UISceneManager UISceneManager { get; set; }

        /// <summary>
        /// 씬에 필요한 리소스 로드
        /// </summary>
        /// <param name="savedState">씬 생성에 필요한 정보(null 가능)</param>
        IEnumerator Load(object savedState);

        /// <summary>
        /// 씬 시작
        /// </summary>
        void Begin();

        /// <summary>
        /// 씬 재개
        /// </summary>
        /// <param name="result">Pop에서 되돌아 왔을 때 결과 값(없으면 null)</param>
        void Resume(object result);

        /// <summary>
        /// 씬 정지
        /// </summary>
        void Pause();

        /// <summary>
        /// 씬 종료
        /// </summary>
        void Finish();

        /// <summary>
        /// 씬의 현재 상태를 얻음
        /// </summary>
        /// <returns>씬의 현재 상태(null 가능)</returns>
        object GetState();
    }
}
