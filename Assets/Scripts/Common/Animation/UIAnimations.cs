using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Puzzle.Animation
{
    /// <summary>
    /// UI 연출 전용 헬퍼 클래스.
    /// 이동, 병합 등의 DOTween 기반 연출을 제공한다.
    /// </summary>
    public static class UIAnimations
    {
        /// <summary>
        /// 지정된 위치로 이동시키는 애니메이션.
        /// </summary>
        public static async Task MoveAsync(RectTransform rect, Vector3 to, float duration = 0.15f,
            Ease ease = Ease.OutCubic, bool waitForCompletion = true)
        {
            await rect.DOAnchorPos(to, duration).SetEase(ease)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// 블록 병합 시 스케일이 커졌다가 줄어드는 연출.
        /// </summary>
        public static async Task MergeAsync(RectTransform rect, float punch = 0.2f, float duration = 0.2f,
            int vibrato = 1, bool waitForCompletion = true)
        {
            await rect.DOPunchScale(Vector3.one * punch, duration, vibrato)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// 블록 Spawn 및 스케일 변경 연출
        /// </summary>
        public static async Task ScaleAsync(RectTransform rect, Vector3 to, float duration = 0.2f,
            Ease ease = Ease.OutBack, bool waitForCompletion = true)
        {
            await rect.DOScale(to, duration).SetEase(ease)
                .AsyncWaitForCompletion();
        }
        
        
	    public static IEnumerator Position(Transform transform, float duration, Vector3 targetPos,
		    Interpolations.InterpolationsAction interpolationsAction, bool isLocal = false)
	    {
		    var startPos = (isLocal) ? transform.localPosition : transform.position;
		    var sumTime = 0.0f;

		    while (sumTime < duration)
		    {
			    var tempPos = Vector3.Lerp(startPos, targetPos,
				    interpolationsAction.Invoke(sumTime, 0, 1, duration));

			    if (isLocal)
			    {
				    transform.localPosition = tempPos;
			    }
			    else
			    {
				    transform.position = tempPos;
			    }

			    yield return null;

			    sumTime += Time.deltaTime;
		    }

		    if (isLocal)
		    {
			    transform.localPosition = targetPos;
		    }
		    else
		    {
			    transform.position = targetPos;
		    }
	    }

	    public static IEnumerator Scale(Transform transform, float duration, Vector3 targetScale,
		    Interpolations.InterpolationsAction interpolationsAction)
	    {
		    var startScale = transform.localScale;
		    var sumTime = 0.0f;

		    while (sumTime < duration)
		    {
			    transform.localScale = Vector3.Lerp(startScale, targetScale,
				    interpolationsAction.Invoke(sumTime, 0, 1, duration));

			    yield return null;

			    sumTime += Time.deltaTime;
		    }

		    transform.localScale = targetScale;
	    }

	    /// <summary>
	    /// 스케일이 특정 scale까지 바뀌었다가 돌아옴
	    /// </summary>
	    public static IEnumerator ElasticScale(Transform transform, float duration, Vector3 centerScale,
		    Interpolations.InterpolationsAction increaseInterpolation, Interpolations.InterpolationsAction decreaseInterpolation)
	    {
		    var originScale = transform.localScale;

		    yield return Scale(transform, duration * 0.5f, centerScale, increaseInterpolation);
		    yield return Scale(transform, duration * 0.5f, originScale, decreaseInterpolation);
	    }
    }   
}
