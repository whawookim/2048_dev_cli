using UnityEngine;
using System.Collections;

namespace Puzzle.UI
{
    public class NoticePopupState
    {
        public string NoticeText { get; set; }
    }
    
    public class NoticePopup : MonoBehaviour, IUIOverlay
    {
        public static NoticePopup Instance { get; private set; }

        public static string AddressableName => nameof(NoticePopup);

        private NoticePopupState states;
        
        #region Monobehavior

        public void Awake()
        {
            Debug.Assert(Instance == null);
            
            Instance = this;
        }

        public void OnDestroy()
        {
            Debug.Assert(Instance == this);
            
            Instance = null;
        }

        #endregion
        
        #region IUIOverlay
        public string Name => nameof(RankingPopup);

        public UISceneManager UISceneManager { get; set; }

        public void Begin(object state = null)
        {
            states = state as NoticePopupState;
            
            Debug.Assert(states != null);

        }

        public IEnumerator OpenAnimation()
        {
            yield break;
        }

        public IEnumerator CloseAnimation()
        {
            yield break;
        }

        public void OnClickBackButton()
        {
        }

        public void Finish()
        {
        }
        #endregion
    }
}
