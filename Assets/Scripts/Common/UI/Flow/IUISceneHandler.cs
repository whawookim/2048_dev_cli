using System.Threading.Tasks;
using Puzzle.UI.Scene;

namespace Puzzle.UI.Flow
{
    /// <summary>
    /// UI 씬 전환 및 관리 기능을 정의한 인터페이스입니다.
    /// </summary>
    public interface IUISceneHandler
    {
        /// <summary>
        /// 현재 표시 중인 씬입니다.
        /// </summary>
        IUIScene CurrentScene { get; }

        /// <summary>
        /// 현재 적용된 씬 전환 효과입니다.
        /// </summary>
        UITransition CurrentTransition { get; }

        /// <summary>
        /// 씬 전환 효과를 적용하며 새로운 씬으로 전환합니다.
        /// 기존 씬은 Close 후 비활성화되고, 새로운 씬은 Begin 후 Open 됩니다.
        /// </summary>
        /// <param name="transition">전환 효과 정보</param>
        Task SetTransitionAsync(UITransition transition);

        /// <summary>
        /// 모든 씬을 비활성화하고 스택을 초기화합니다.
        /// </summary>
        void ClearStackScenes();
    }
}
