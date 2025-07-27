using UnityEngine;

public static class Interpolations
{
	public delegate float InterpolationsAction(float t, float b, float c, float d);

	public static float Linear(float t, float b, float c, float d)
	{
		return c * t / d + b;
	}

	public static float EaseInQuad(float t, float b, float c, float d)
	{
		t /= d;
		return c * t * t + b;
	}

	public static float EaseOutQuad(float t, float b, float c, float d)
	{
		t /= d;
		return -c * t * (t - 2) + b;
	}

	public static float EaseInOutQuad(float t, float b, float c, float d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t + b;
		t--;
		return -c / 2 * ( t * (t - 2) - 1) + b;
	}

	public static float EaseInCubic(float t, float b, float c, float d)
	{
		t /= d;
		return c * t * t * t + b;
	}

	public static float EaseOutCubic(float t, float b, float c, float d)
	{
		t /= d;
		t--;
		return c * (t * t * t + 1) + b;
	}

	public static float EaseInOutCubic(float t, float b, float c, float d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t * t + b;
		t -= 2;
		return c / 2 * (t * t * t + 2) + b;
	}

	public static float EaseInQuart(float t, float b, float c, float d)
	{
		t /= d;
		return c * t * t * t * t + b;
	}

	public static float EaseOutQuart(float t, float b, float c, float d)
	{
		t /= d;
		t--;
		return -c * (t * t * t * t - 1) + b;
	}

	public static float EaseInOutQuart(float t, float b, float c, float d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t * t * t + b;
		t -= 2;
		return -c / 2 * (t * t * t * t - 2) + b;
	}

	public static float EaseInQuint(float t, float b, float c, float d)
	{
		t /= d;
		return c * t * t * t * t * t + b;
	}

	public static float EaseOutQuint(float t, float b, float c, float d)
	{
		t /= d;
		t--;
		return c * (t * t * t * t * t + 1) + b;
	}

	public static float EaseInOutQuint(float t, float b, float c, float d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t * t * t * t + b;
		t -= 2;
		return c / 2 * (t * t * t * t * t + 2) + b;
	}

	public static float EaseInSine(float t, float b, float c, float d)
	{
		return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
	}

	public static float EaseOutSine(float t, float b, float c, float d)
	{
		return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
	}

	public static float EaseInOutSine(float t, float b, float c, float d)
	{
		return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
	}

	public static float EaseInExpo(float t, float b, float c, float d)
	{
		return c * Mathf.Pow( 2, 10 * (t / d - 1) ) + b;
	}

	public static float EaseOutExpo(float t, float b, float c, float d)
	{
		return c * ( -Mathf.Pow( 2, -10 * t / d ) + 1 ) + b;
	}

	public static float EaseInOutExpo(float t, float b, float c, float d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * Mathf.Pow( 2, 10 * (t - 1) ) + b;
		t--;
		return c/2 * ( -Mathf.Pow( 2, -10 * t) + 2 ) + b;
	}

	public static float EaseInCirc(float t, float b, float c, float d)
	{
		t /= d;
		return -c * (Mathf.Sqrt(1 - t * t) - 1) + b;
	}

	public static float EaseOutCirc(float t, float b, float c, float d)
	{
		t /= d;
		t--;
		return c * Mathf.Sqrt(1 - t * t) + b;
	}

	public static float EaseInOutCirc(float t, float b, float c, float d)
	{
		t /= d / 2;
		if (t < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
		t -= 2;
		return c / 2 * (Mathf.Sqrt(1 - t * t) + 1) + b;
	}
}
