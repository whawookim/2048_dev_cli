using UnityEngine;
using DG.Tweening;

namespace DOTween
{
    /// <summary>
    /// DOTween을 활용한 좌우 루프 이동 연출용
    /// </summary>
    public class SimpleTweenPosition : MonoBehaviour
    {
        [Header("움직임 설정")]
        public float moveAmount = 50f; // 이동 거리
        public float duration = 0.5f;  // 이동 시간

        private void Start()
        {
            transform.DOLocalMoveX(transform.localPosition.x + moveAmount, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject); // 오브젝트가 파괴되면 트윈도 함께 정리!
        }
    }
}
