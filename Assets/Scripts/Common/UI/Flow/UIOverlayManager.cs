using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puzzle.UI.Loader;
using Puzzle.UI.Overlay;

namespace Puzzle.UI.Flow
{
    /// <summary>
    /// UI Overlay의 생성, 열기, 닫기, 제거 등을 담당하는 관리자 클래스입니다.
    /// IUIOverlay 인터페이스를 구현한 UI를 Addressables 기반으로 로드 및 재사용합니다.
    /// </summary>
    public class UIOverlayManager : IUIOverlayHandler
    {
        /// <summary>
        /// 현재 화면에 쌓여 있는 Overlay들을 순서대로 저장하는 스택입니다.
        /// </summary>
        private readonly Stack<IUIOverlay> _overlayStack = new();

        public IUIOverlay CurrentOverlay => _overlayStack.Count > 0 ? _overlayStack.Peek() : null;
        
        /// <summary>
        /// Overlay가 닫힌 후에 호출되는 콜백 이벤트입니다.
        /// </summary>
        public event Action OverlayPopped;

        /// <summary>
        /// 지정한 Overlay를 Addressables에서 로드하거나 기존 인스턴스를 재사용하여 화면에 표시합니다.
        /// Begin → SetActive → OpenAsync 순으로 처리되며, 처리 중에는 UI 입력이 차단됩니다.
        /// </summary>
        /// <param name="overlayType">표시할 Overlay의 타입 (IUIOverlay 구현 필수)</param>
        /// <param name="state">초기화에 사용할 상태 데이터 (선택)</param>
        public async Task PushOverlayAsync(Type overlayType, object state = null)
        {
            try
            {
                UIBlocker.Instance.SetEnabled();

                var overlay = await UIAssetLoader.LoadOverlayAsync(overlayType);
                
                if (_overlayStack.Contains(overlay))
                {
                    MyDebug.LogWarning($"Overlay already exists in stack: {overlayType.Name}");
                    UIBlocker.Instance.SetDisabled();
                    return;
                }

                overlay.UIOverlayManager = this;
                
                // 스택에 추가
                _overlayStack.Push(overlay);

                // SetActive 전에 초기화 (state 전달)
                overlay.Begin(state);

                // UI 활성화
                (overlay as UnityEngine.MonoBehaviour)?.gameObject.SetActive(true);

                // 열리는 애니메이션 대기
                await overlay.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                UIBlocker.Instance.SetDisabled();
            }
        }

        /// <summary>
        /// 가장 최근에 열린 Overlay를 닫고 비활성화합니다.
        /// CloseAsync → Finish → SetActive(false) 순으로 처리되며, 처리 중에는 UI 입력이 차단됩니다.
        /// </summary>
        public async Task PopOverlayAsync()
        {
            if (CurrentOverlay == null || _overlayStack.Count == 0)
            {
                MyDebug.LogWarning("No stacked overlay to pop");
                return;
            }

            try
            {
                UIBlocker.Instance.SetEnabled();

                var popOverlay = _overlayStack.Pop();

                // 닫는 애니메이션
                await popOverlay.CloseAsync();

                // 종료 처리
                popOverlay.Finish();

                // 비활성화
                (popOverlay as UnityEngine.MonoBehaviour)?.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                UIBlocker.Instance.SetDisabled();

                // 팝업 종료 이벤트
                OverlayPopped?.Invoke();
            }
        }

        /// <summary>
        /// 모든 Overlay를 강제로 닫고 스택을 초기화합니다.
        /// 애니메이션 없이 즉시 닫히며, 일반적으로 씬 전환 등에 사용됩니다.
        /// </summary>
        public void ClearAllOverlays()
        {
            while (_overlayStack.Count > 0)
            {
                var overlay = _overlayStack.Pop();
                overlay.Finish();
                (overlay as UnityEngine.MonoBehaviour)?.gameObject.SetActive(false);
            }
        }
    }
}
