using System.Threading.Tasks;
using Puzzle.UI.Flow;

namespace Puzzle.UI.Scene
{
    /// <summary>
    /// 스택 기반 UI 씬 인터페이스입니다. 씬 전환과 상태 관리를 담당합니다.
    /// </summary>
    public interface IUIScene
    {
        /// <summary>
        /// 씬 이름
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 이 씬을 관리 중인 매니저
        /// </summary>
        UISceneManager UISceneManager { get; set; }

        /// <summary>
        /// 씬에 필요한 리소스를 로드합니다. Addressables 또는 외부 데이터 포함 가능.
        /// </summary>
        /// <param name="savedState">저장된 상태 정보 또는 초기화에 필요한 파라미터</param>
        Task LoadAsync(object savedState);

        /// <summary>
        /// 씬 활성화 직후 호출되는 진입 지점입니다.
        /// </summary>
        void Begin();

        /// <summary>
        /// 다른 씬에서 되돌아올 때 호출되는 메서드입니다.
        /// </summary>
        /// <param name="result">되돌아올 때 전달되는 상태 값</param>
        void Resume(object result);

        /// <summary>
        /// 이 씬에서 다른 씬으로 이동할 때 정지 처리를 합니다.
        /// </summary>
        void Pause();

        /// <summary>
        /// 씬이 종료되었을 때 정리 처리를 수행합니다.
        /// </summary>
        void Finish();

        /// <summary>
        /// 이 씬의 현재 상태를 반환합니다. 저장 필요 시 사용됩니다.
        /// </summary>
        /// <returns>상태 객체</returns>
        object GetState();

        /// <summary>
        /// 안드로이드 BackButton, Scene 닫기 콜백 연결 등으로 사용
        /// </summary>
        void OnClickBackButton();
    }
}
