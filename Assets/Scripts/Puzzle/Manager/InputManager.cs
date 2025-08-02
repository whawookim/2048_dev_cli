using Puzzle.UI;
using UnityEngine;
using Puzzle.UI.Flow;

namespace Puzzle
{
    public class InputManager : IUpdatable
    {
        /// <summary>
        /// 안드로이드 뒤로가기 동작
        /// </summary>
        void IUpdatable.UpdateFrame()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            if (UIBlocker.Instance.IsBlocked) return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var curOverlay = UIFlowManager.Instance.CurrentOverlay;

                if (curOverlay != null)
                {
                    curOverlay.OnClickBackButton();
                    return;
                }
                
                var curScene = UIFlowManager.Instance.CurrentScene;

                if (curScene != null)
                {
                    curScene.OnClickBackButton();
                    return;
                }
            }
#endif
        }
    }
}
