using System;

public static class Interpolations
{
	public delegate float InterpolationsAction(float t, float b, float c, float d);
	
	public static float Linear(float t, float b, float c, float d)
	{
		return c * t / d + b;
	}
}
