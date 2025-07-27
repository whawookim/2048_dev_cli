using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    /// <summary>
    /// Unity uGUI 기반의 재사용 가능한 스크롤 리스트.
    /// 제한된 수의 아이템 GameObject를 풀링하여 스크롤 시 동적으로 재사용하며,
    /// ScrollRect의 Viewport 영역 안에서 필요한 만큼만 활성화하여 퍼포먼스를 최적화함.
    /// </summary>
    [DisallowMultipleComponent]
    public class UGUIReusableScrollList : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        [Header("Layout")]
        [SerializeField] private int itemCountPerRow = 1;
        [SerializeField] private Vector2 spacing = new Vector2(0, 10);
        [SerializeField] private bool adjustItemWidth = false;
        [SerializeField] private bool topPivot = true;

        /// <summary>
        /// 아이템이 보여질 때마다 호출되는 콜백. GameObject와 Index를 제공함
        /// </summary>
        public Action<GameObject, int> OnUpdateItem;

        private readonly List<RectTransform> items = new();

        private int itemCount;
        private Vector2 itemSize;
        private int visibleRowCount;
        private float spacingY;

        /// <summary>
        /// 초기화 함수: 프리팹 기반으로 스크롤 아이템 풀을 구성하고 Scroll 이벤트를 바인딩합니다.
        /// </summary>
        public void Init()
        {
            Clear();
            
            // 원본 게임오브젝트는 꺼져도 됨.
            itemPrefab.SetActive(false);

            if (itemPrefab.TryGetComponent(out LayoutElement layout))
            {
                itemSize = new Vector2(layout.preferredWidth, layout.preferredHeight);
            }
            else
            {
                var rt = itemPrefab.GetComponent<RectTransform>();
                itemSize = rt.sizeDelta;
            }

            spacingY = spacing.y;

            float viewportHeight = ((RectTransform)scrollRect.viewport).rect.height;
            visibleRowCount = Mathf.CeilToInt(viewportHeight / (itemSize.y + spacingY)) + 2;

            int needCount = visibleRowCount * itemCountPerRow;
            for (int i = 0; i < needCount; i++)
            {
                var item = Instantiate(itemPrefab, content).GetComponent<RectTransform>();
                item.name = $"Item_{i}";
                item.gameObject.SetActive(true);
                items.Add(item);
            }

            scrollRect.onValueChanged.AddListener(_ => UpdateVisibleItems());
        }

        /// <summary>
        /// 아이템 개수 설정. 전체 리스트 길이에 따라 Content 높이 조정.
        /// </summary>
        public void SetItemCount(int count)
        {
            itemCount = count;

            int rowCount = Mathf.CeilToInt(count / (float)itemCountPerRow);
            float height = rowCount * (itemSize.y + spacingY) - spacingY;
            content.sizeDelta = new Vector2(content.sizeDelta.x, height);

            UpdateVisibleItems();
        }

        private void UpdateVisibleItems()
        {
            float scrollY = content.anchoredPosition.y;
            float viewHeight = ((RectTransform)scrollRect.viewport).rect.height;

            // 스크롤 불필요한 경우 고정 배치
            if (content.rect.height <= viewHeight)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i >= itemCount)
                    {
                        items[i].gameObject.SetActive(false);
                        continue;
                    }

                    var item = items[i];
                    item.gameObject.SetActive(true);

                    float x = (i % itemCountPerRow) * (itemSize.x + spacing.x);
                    float y = -(i / itemCountPerRow) * (itemSize.y + spacingY);
                    item.anchoredPosition = new Vector2(x, y);

                    OnUpdateItem?.Invoke(item.gameObject, i);
                }
                return;
            }

            int firstVisibleRow = Mathf.FloorToInt(scrollY / (itemSize.y + spacingY));
            int startIndex = firstVisibleRow * itemCountPerRow;

            for (int i = 0; i < items.Count; i++)
            {
                int itemIndex = startIndex + i;

                if (itemIndex >= 0 && itemIndex < itemCount)
                {
                    var item = items[i];
                    item.gameObject.SetActive(true);

                    float x = (itemIndex % itemCountPerRow) * (itemSize.x + spacing.x);
                    float y = -(itemIndex / itemCountPerRow) * (itemSize.y + spacingY);
                    item.anchoredPosition = new Vector2(x, y);

                    OnUpdateItem?.Invoke(item.gameObject, itemIndex);
                }
                else
                {
                    items[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetScroll()
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
            UpdateVisibleItems();
        }

        public void Clear()
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
        }

        private void OnDestroy()
        {
            scrollRect.onValueChanged.RemoveListener(_ => UpdateVisibleItems());
        }
    }   
}
