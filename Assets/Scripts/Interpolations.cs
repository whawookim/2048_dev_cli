using System;

public static class Interpolations
{
	public delegate double InterpolationsAction(double t, double b, double c, double d);
	
	public static double Linear(double t, double b, double c, double d)
	{
		return c * t / d + b;
	}

	public static double EaseInQuad(double t, double b, double c, double d)
	{
		t /= d;
		return c * t * t + b;
	}

	public static double EaseOutQuad(double t, double b, double c, double d)
	{
		t /= d;
		return -c * t * (t - 2) + b;
	}

	public static double EaseInOutQuad(double t, double b, double c, double d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t + b;
		t--;
		return -c / 2 * ( t * (t - 2) - 1) + b;
	}

	public static double EaseInCubic(double t, double b, double c, double d)
	{
		t /= d;
		return c * t * t * t + b;
	}

	public static double EaseOutCubic(double t, double b, double c, double d)
	{
		t /= d;
		t--;
		return c * (t * t * t + 1) + b;
	}

	public static double EaseInOutCubic(double t, double b, double c, double d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t * t + b;
		t -= 2;
		return c / 2 * (t * t * t + 2) + b;
	}

	public static double EaseInQuart(double t, double b, double c, double d)
	{
		t /= d;
		return c * t * t * t * t + b;
	}

	public static double EaseOutQuart(double t, double b, double c, double d)
	{
		t /= d;
		t--;
		return -c * (t * t * t * t - 1) + b;
	}

	public static double EaseInOutQuart(double t, double b, double c, double d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t * t * t + b;
		t -= 2;
		return -c / 2 * (t * t * t * t - 2) + b;
	}

	public static double EaseInQuint(double t, double b, double c, double d)
	{
		t /= d;
		return c * t * t * t * t * t + b;
	}

	public static double EaseOutQuint(double t, double b, double c, double d)
	{
		t /= d;
		t--;
		return c * (t * t * t * t * t + 1) + b;
	}

	public static double EaseInOutQuint(double t, double b, double c, double d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * t * t * t * t * t + b;
		t -= 2;
		return c / 2 * (t * t * t * t * t + 2) + b;
	}

	public static double EaseInSine(double t, double b, double c, double d)
	{
		return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b;
	}

	public static double EaseOutSine(double t, double b, double c, double d)
	{
		return c * Math.Sin(t / d * (Math.PI / 2)) + b;
	}

	public static double EaseInOutSine(double t, double b, double c, double d)
	{
		return -c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b;
	}

	public static double EaseInExpo(double t, double b, double c, double d)
	{
		return c * Math.Pow( 2, 10 * (t / d - 1) ) + b;
	}

	public static double EaseOutExpo(double t, double b, double c, double d)
	{
		return c * ( -Math.Pow( 2, -10 * t / d ) + 1 ) + b;
	}

	public static double EaseInOutExpo(double t, double b, double c, double d)
	{
		t /= d / 2;
		if (t < 1) return c / 2 * Math.Pow( 2, 10 * (t - 1) ) + b;
		t--;
		return c/2 * ( -Math.Pow( 2, -10 * t) + 2 ) + b;
	}

	public static double EaseInCirc(double t, double b, double c, double d)
	{
		t /= d;
		return -c * (Math.Sqrt(1 - t * t) - 1) + b;
	}

	public static double EaseOutCirc(double t, double b, double c, double d)
	{
		t /= d;
		t--;
		return c * Math.Sqrt(1 - t * t) + b;
	}

	public static double EaseInOutCirc(double t, double b, double c, double d)
	{
		t /= d / 2;
		if (t < 1) return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;
		t -= 2;
		return c / 2 * (Math.Sqrt(1 - t * t) + 1) + b;
	}
}
