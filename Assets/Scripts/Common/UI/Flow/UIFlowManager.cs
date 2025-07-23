using System;
using System.Threading.Tasks;
using Puzzle.UI.Scene;
using Puzzle.UI.Overlay;

namespace Puzzle.UI.Flow
{
    /// <summary>
    /// UIOverlayManager 및 UISceneManager를 통합 관리하는 UI 흐름 진입점 클래스
    /// </summary>
    public class UIFlowManager : IUIOverlayHandler, IUISceneHandler
    {
        public static UIFlowManager Instance { get; } = new ();

        private readonly UIOverlayManager _overlayManager;
        private readonly UISceneManager _sceneManager;

        private UIFlowManager()
        {
            _overlayManager = new UIOverlayManager();
            _sceneManager = new UISceneManager();
        }

        #region Overlay

        public void PushOverlay(Type overlayType, object state = null)
        {
            _ = PushOverlayAsync(overlayType, state);
        }

        public async Task PushOverlayAsync(Type overlayType, object state = null)
        {
            await _overlayManager.PushOverlayAsync(overlayType, state);
        }

        public void PopOverlay()
        {
            _ = PopOverlayAsync();
        }

        public async Task PopOverlayAsync()
        {
            await _overlayManager.PopOverlayAsync();
        }

        public void ClearAllOverlays()
        {
            _overlayManager.ClearAllOverlays();
        }

        public IUIOverlay CurrentOverlay => _overlayManager.CurrentOverlay;

        #endregion

        #region Scene

        public void SetTransition(UITransition transition)
        {
            _ = SetTransitionAsync(transition);
        }

        public async Task SetTransitionAsync(UITransition transition)
        {
            await _sceneManager.SetTransitionAsync(transition);
        }

        public void ClearStackScenes()
        {
            _sceneManager.ClearStackScenes();
        }

        public IUIScene CurrentScene => _sceneManager.CurrentScene;

        public UITransition CurrentTransition => _sceneManager.CurrentTransition;

        #endregion
    }
}
