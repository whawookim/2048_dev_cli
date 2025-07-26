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

        private static readonly Color32[] Colors =
        {
            // 2
            new Color32(208, 211, 254, 204),
            // 4
            new Color32(133, 141, 250, 204),
            // 8
            new Color32(62, 74, 246, 204),
            // 16
            new Color32(253, 252, 197, 204),
            // 32
            new Color32(250, 248, 133, 204),
            // 64
            new Color32(253, 250, 77, 204),
            // 128
            new Color32(255, 206, 160, 204),
            // 256
            new Color32(253, 172, 97, 204),
            // 512
            new Color32(245, 131, 24, 204),
            // 1024
            new Color32(245, 131, 24, 204),
            // 2048
            new Color32(245, 24, 235, 204),
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
            bgSprite.color = Colors[(int) Mathf.Log(num)];
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
