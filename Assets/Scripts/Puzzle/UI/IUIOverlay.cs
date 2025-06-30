using System.Collections;

namespace Puzzle.UI
{
    /// <summary>
    /// 현재 UIScene 위에 띄울 UI 인터페이스
    /// ex) 공지사항, 메세지, 팝업, 채팅창 등..
    /// </summary>
    public interface IUIOverlay
    {
        /// <summary>
        /// 오버레이 이름.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 이 오버레이를 관리 중인 매니저 얻기/설정
        /// </summary>
        UISceneManager UISceneManager { get; set; }

        /// <summary>
        /// 오버레이 시작시 불리는 함수
        /// </summary>
        void Begin(object state = null);

        /// <summary>
        /// 열릴 때 애니메이션 (따로 없다면 구현 안해도 됨. => 트랜지션 없이 열림)
        /// </summary>
        IEnumerator OpenAnimation();

        /// <summary>
        /// 닫힐 때 애니메이션 (따로 없다면 구현 안해도 됨. => 트랜지션 없이 닫힘)
        /// </summary>
        IEnumerator CloseAnimation();

        void OnClickBackButton();

        /// <summary>
        /// 오버레이 팝할 때 불리는 함수.
        /// </summary>
        void Finish();
    }
}