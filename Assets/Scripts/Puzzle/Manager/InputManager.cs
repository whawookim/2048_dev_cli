using UnityEngine;
using System;

namespace Puzzle
{
    public class InputManager : IUpdatable
    {
        public Action EscapeCallback { get; set; }
        
        /// <summary>
        /// 안드로이드 뒤로가기 동작
        /// </summary>
        void IUpdatable.UpdateFrame()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var curOverlay = UISceneManager.Instance.CurrentOverlay;

                if (curOverlay != null)
                {
                    curOverlay.OnClickBackButton();
                    return;
                }

                if (EscapeCallback != null)
                {
                    EscapeCallback.Invoke();
                }
            }
#endif
        }
    }
}
