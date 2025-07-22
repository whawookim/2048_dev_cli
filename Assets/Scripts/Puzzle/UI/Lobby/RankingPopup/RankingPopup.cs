using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Puzzle.UI
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

        private RankingPopupState states;

        public static string AddressableName => nameof(RankingPopup);
        
        [SerializeField]
        private TextMeshProUGUI title;
        
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private UGUIReusableScrollList scrollList;

        [SerializeField]
        private GameObject emptyObj;
       
        private LocalizedString localizedTitle = new("GameStrings", "ranking_popup_title");
        
        private List<RankingData> rankingDataList = new ();
        
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

        public UISceneManager UISceneManager { get; set; }

        public void Begin(object state = null)
        {
            states = state as RankingPopupState;
            
            Debug.Assert(states != null);
            
            MyDebug.Log(states.StageMode.ToString());
            
            localizedTitle.Arguments = new object[] { new { stage = states.StageMode.ToString() }};
            localizedTitle.StringChanged -= OnTitleChanged;
            localizedTitle.StringChanged += OnTitleChanged;
            localizedTitle.RefreshString();
            
            // TODO: 캐시된 거 있으면 특정 시간동안 그대로 사용하기 기능
        }

        public IEnumerator OpenAnimation()
        {
            InitUI();

            _ = LoadDataAsync();
            
            yield break;
        }

        public IEnumerator CloseAnimation()
        {
            yield break;
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
            
            var request = RankingManager.Instance.GetRankingData(states.StageMode, states.RankingMode);
            
            await request;

            rankingDataList = request.Result ?? new List<RankingData>();

            SetUI();
            
            UIBlocker.Instance.SetDisabled();
        }

        private void OnItemUpdated(GameObject go, int index)
        {
            var item = go.GetComponent<RankingPopupItem>();
            item.SetData(rankingDataList[index]);
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
            scrollList.SetItemCount(rankingDataList.Count);

            if (resetScroll)
            {
                scrollList.ResetScroll();
            }
            
            emptyObj.SetActive(rankingDataList.Count <= 0);
        }

        public void OnTitleChanged(string localizedValue)
        {
            title.text = localizedValue;
        }

        public void OnClickExitButton()
        {
            UISceneManager.PopOverlay();
        }
    }
}
