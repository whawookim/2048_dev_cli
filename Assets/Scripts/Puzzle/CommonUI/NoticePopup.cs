using UnityEngine;
using System.Threading.Tasks;

namespace Puzzle.UI.Overlay
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

        public Flow.UIOverlayManager UIOverlayManager { get; set; }

        public void Begin(object state = null)
        {
            states = state as NoticePopupState;
            
            Debug.Assert(states != null);

        }

        public Task OpenAsync()
        {
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            return Task.CompletedTask;
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
