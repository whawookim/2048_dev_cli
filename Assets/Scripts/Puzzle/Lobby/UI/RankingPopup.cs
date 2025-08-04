using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Puzzle.UI.Overlay
{
    public class RankingPopupState
    {
        /// <summary>
        /// 현재 스테이지 모드
        /// TODO: StageSpec 변환
        /// </summary>
        public StageMode StageMode { get; set; }

        /// <summary>
        /// 랭킹을 표시할 모드 선택
        /// </summary>
        /// <remarks>default로 Score</remarks>
        public RankingMode RankingMode { get; set; } = RankingMode.Score;
    }
    
    /// <summary>
    /// 스테이지 랭킹 팝업
    /// </summary>
    public class RankingPopup : MonoBehaviour, IUIOverlay
    {
        public static RankingPopup Instance { get; private set; }

        private RankingPopupState _states;

        public static string AddressableName => nameof(RankingPopup);
        
        [SerializeField]
        private TextMeshProUGUI title;
        
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private UGUIReusableScrollList scrollList;

        [SerializeField]
        private GameObject emptyObj;
       
        private readonly LocalizedString _localizedTitle = new(GameStringsManager.DefaultTable, "ranking_popup_title");
        
        private List<RankingData> _rankingDataList = new ();
        
        #region Monobehavior

        public void Awake()
        {
            Debug.Assert(Instance == null);
            
            Instance = this;

            scrollList.OnUpdateItem = OnItemUpdated;
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
            _states = state as RankingPopupState;
            
            Debug.Assert(_states != null);
            
            MyDebug.Log(_states.StageMode.ToString());
            
            _localizedTitle.Arguments = new object[] { new { stage = _states.StageMode.ToString() }};
            _localizedTitle.StringChanged -= OnTitleChanged;
            _localizedTitle.StringChanged += OnTitleChanged;
            _localizedTitle.RefreshString();
            
            // TODO: 캐시된 거 있으면 특정 시간동안 그대로 사용하기 기능
        }

        public Task OpenAsync()
        {
            InitUI();

            return LoadDataAsync();
        }

        public Task CloseAsync()
        {
            return Task.CompletedTask;
        }

        public void OnClickBackButton()
        {
            OnClickExitButton();
        }

        public void Finish()
        {
        }
        #endregion

        private async Task LoadDataAsync()
        {
            UIBlocker.Instance.SetEnabled();
            
            var request = RankingManager.Instance.GetRankingData(_states.StageMode, _states.RankingMode);
            
            await request;

            _rankingDataList = request.Result ?? new List<RankingData>();

            SetUI();
            
            UIBlocker.Instance.SetDisabled();
        }

        private void OnItemUpdated(GameObject go, int index)
        {
            var item = go.GetComponent<RankingPopupItem>();
            item.SetData(_rankingDataList[index]);
        }

        public void InitUI()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.transform);
            scrollList.Init();
            scrollList.SetItemCount(0);
            scrollList.ResetScroll();
        }

        public void SetUI(bool resetScroll = true)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.transform);
            scrollList.Init();
            scrollList.SetItemCount(_rankingDataList.Count);

            if (resetScroll)
            {
                scrollList.ResetScroll();
            }
            
            emptyObj.SetActive(_rankingDataList.Count <= 0);
        }

        public void OnTitleChanged(string localizedValue)
        {
            title.text = localizedValue;
        }

        public void OnClickExitButton()
        {
            Flow.UIFlowManager.Instance.PopOverlay();
        }
    }
}
