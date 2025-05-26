using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace DOTween
{
    /// <summary>
    /// UGUI 버튼 스케일 효과용
    /// </summary>
    public class ButtonScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private Vector3 originalScale;

        void Start()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOScale(originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject); // 오브젝트가 파괴되면 트윈도 함께 정리!
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOScale(originalScale, 0.1f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject); // 오브젝트가 파괴되면 트윈도 함께 정리!
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(originalScale, 0.1f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject); // 오브젝트가 파괴되면 트윈도 함께 정리!
        }
    }
}
