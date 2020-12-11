using System.Collections;
using UnityEngine;

public static class UIAnimations
{
	public static IEnumerator Position(UIRect widget, float duration, Vector3 targetPos, 
		Interpolations.InterpolationsAction interpolationsAction)
	{
		var startPos = widget.cachedTransform.localPosition;
		var sumTime = 0.0f;

		while (sumTime < duration)
		{
			widget.cachedTransform.localPosition =
				Vector3.Lerp(startPos, targetPos,
					interpolationsAction.Invoke(sumTime, startPos.sqrMagnitude, (targetPos - startPos).sqrMagnitude,
						duration));

			yield return null;

			sumTime += Time.deltaTime;
		}

		widget.cachedTransform.localPosition = targetPos;
		
		yield break;
	}
}
