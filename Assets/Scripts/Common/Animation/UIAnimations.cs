using System.Collections;
using UnityEngine;

public static class UIAnimations
{
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
