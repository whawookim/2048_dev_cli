using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle.Data;
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
        public StageMode CurrentStage { get; set; }
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
       
        private LocalizedString localizedTitle = new("GameStrings", "ranking_popup_title");
        
        private List<RankingData> rankingDataList;
        
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
            
            Debug.Log(states.CurrentStage);
            
            localizedTitle.Arguments = new object[] { new { stage = states.CurrentStage.ToString() }};
            localizedTitle.StringChanged -= OnTitleChanged;
            localizedTitle.StringChanged += OnTitleChanged;
            localizedTitle.RefreshString();
        }

        public IEnumerator OpenAnimation()
        {
            // TODO: 서버에서 랭킹 데이터 받아오기 작업 해서 고치기
            rankingDataList = new List<RankingData>();
            rankingDataList.Add(new RankingData(1, "abc", 1000));
            rankingDataList.Add(new RankingData(2, "def", 920));
            rankingDataList.Add(new RankingData(3, "dfdf", 800));
            rankingDataList.Add(new RankingData(4, "gdfd", 60));
            rankingDataList.Add(new RankingData(5, "fdfdf", 19));
            rankingDataList.Add(new RankingData(6, "fdfdf", 17));
            rankingDataList.Add(new RankingData(7, "fdfdf", 15));
            rankingDataList.Add(new RankingData(8, "fdfdf", 12));
            rankingDataList.Add(new RankingData(9, "fdfdf", 11));
            rankingDataList.Add(new RankingData(10, "fdfdf", 9));
            rankingDataList.Add(new RankingData(11, "fdfdf", 5));
            
            SetUI();
            
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

        private void OnItemUpdated(GameObject go, int index)
        {
            var item = go.GetComponent<RankingPopupItem>();
            item.SetData(rankingDataList[index]);
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
