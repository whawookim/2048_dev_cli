using System.Threading.Tasks;
using Puzzle.UI.Flow;

namespace Puzzle.UI.Overlay
{
    public interface IUIOverlay
    {
        /// <summary>
        /// IUIOverlay 이름
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 이 씬을 관리 중인 매니저
        /// </summary>
        UIOverlayManager UIOverlayManager { get; set; }
    
        /// <summary>
        /// 시작시 불리는 함수
        /// </summary>
        void Begin(object state);
    
        /// <summary>
        /// 열릴 때 연출
        /// </summary>
        Task OpenAsync();
    
        /// <summary>
        /// 닫힐 때 연출
        /// </summary>
        Task CloseAsync();
    
        /// <summary>
        /// 오버레이 닫히고 마지막에 호출
        /// </summary>
        void Finish();
    
        /// <summary>
        /// 안드로이드 BackButton, Overlay 닫기 콜백 연결 등으로 사용
        /// </summary>
        void OnClickBackButton();
    }   
}
