using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace Puzzle.UI
{
	/// <summary>
	/// 블록 컴포넌트
	/// </summary>
	public class Block : MonoBehaviour, IPooledObject
	{
        #region constant

        /// <summary>
        ///  블록 배경 색상
        /// </summary>
        private static readonly Color32[] BgColors =
        {
            // 2
            new Color32(238, 228, 218, 255), // #EEE4DA
            // 4
            new Color32(237, 224, 200, 255), // #EDE0C8
            // 8
            new Color32(242, 177, 121, 255), // #F2B179
            // 16
            new Color32(245, 149, 99, 255),  // #F59563
            // 32
            new Color32(246, 124, 95, 255),  // #F67C5F
            // 64
            new Color32(246, 94, 59, 255),   // #F65E3B
            // 128
            new Color32(237, 207, 114, 255), // #EDCF72
            // 256
            new Color32(237, 204, 97, 255),  // #EDCC61
            // 512
            new Color32(237, 200, 80, 255),  // #EDC850
            // 1024
            new Color32(237, 197, 63, 255),  // #EDC53F
            // 2048
            new Color32(237, 194, 46, 255),  // #EDC22E
            // 4096
            new Color32(60, 58, 50, 255),    // #3C3A32 (딥 다크)
        };

        /// <summary>
        /// 블록 숫자 색상
        /// </summary>
        private static readonly Color32[] TextColors =
        {
            // 2~64
            new Color32(119, 110, 101, 255),
            // 128~
            new Color32(249, 246, 242, 255),
        };

        #endregion
        
        [field:SerializeField]
        public RectTransform Rect { get; private set; }

        [SerializeField]
        private TextMeshProUGUI numberText;

        [SerializeField]
        private Image bgSprite;

        public int Number { get; private set; }

        public void Init(int num)
        {
            Number = num;
            numberText.text = num.ToString();
            numberText.color = (num <= 64) ? TextColors[0] : TextColors[1];
            bgSprite.color = BgColors[(int) Mathf.Log(num)];
            Rect.DOKill();
            Rect.localScale = Vector3.one;
        }
        
        public void SetSize(int size)
        {
            Rect.sizeDelta = new Vector2(size, size);
        }

        /// <summary>
        /// 블록의 화면 위치를 설정한다.
        /// 외부에서 보드 좌표를 계산한 후 전달해야 한다.
        /// </summary>
        public void SetPosition(Vector3 worldPos)
        {
            Rect.anchoredPosition = worldPos;
        }

        /// <summary>
        /// 블록을 숨긴다. 병합 등으로 인해 사라질 때 사용.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetScale(Vector3 scale)
        {
            Rect.localScale = scale;
        }
    }
}
