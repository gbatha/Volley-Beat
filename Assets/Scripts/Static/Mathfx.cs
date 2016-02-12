using UnityEngine;
using System;

public class Mathfx
{

	public static float map (float value, float istart, float istop, float ostart, float ostop) {
		return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
	}
	public static float Hermite(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
	}
	
	public static float Sinerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
	}

//	public static float easeOutExpo (float b, float c, float t) {
//		return c * ( -Mathf.Pow( 2f, -10f * t ) + 1f ) + b;
//	}
//
//	public static float easeOutCirc (float b, float c, float t) {
//		t--;
//		return c * Mathf.Sqrt(1f - t*t) + b;
//	}
	
	public static float Coserp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	}
	
	public static float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static Vector3 Berp(Vector3 start, Vector3 end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}
	
	public static float SmoothStep (float x, float min, float max) 
	{
		x = Mathf.Clamp (x, min, max);
		float v1 = (x-min)/(max-min);
		float v2 = (x-min)/(max-min);
		return -2*v1 * v1 *v1 + 3*v2 * v2;
	}
	
	public static float Lerp(float start, float end, float value)
	{
		return ((1.0f - value) * start) + (value * end);
	}
	
	public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 lineDirection = Vector3.Normalize(lineEnd-lineStart);
		float closestPoint = Vector3.Dot((point-lineStart),lineDirection)/Vector3.Dot(lineDirection,lineDirection);
		return lineStart+(closestPoint*lineDirection);
	}
	
	public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 fullDirection = lineEnd-lineStart;
		Vector3 lineDirection = Vector3.Normalize(fullDirection);
		float closestPoint = Vector3.Dot((point-lineStart),lineDirection)/Vector3.Dot(lineDirection,lineDirection);
		return lineStart+(Mathf.Clamp(closestPoint,0.0f,Vector3.Magnitude(fullDirection))*lineDirection);
	}
	public static float Bounce(float x) {
		return Mathf.Abs(Mathf.Sin(6.28f*(x+1f)*(x+1f)) * (1f-x));
	}
	
	// test for value that is near specified float (due to floating point inprecision)
	// all thanks to Opless for this!
	public static bool Approx(float val, float about, float range) {
		return ( ( Mathf.Abs(val - about) < range) );
	}
	
	// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
	// compares the square of the distance to the square of the range as this 
	// avoids calculating a square root which is much slower than squaring the range
	public static bool Approx(Vector3 val, Vector3 about, float range) {
		return ( (val - about).sqrMagnitude < range*range);
	}
	
	/*
     * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
     * This is useful when interpolating eulerAngles and the object
     * crosses the 0/360 boundary.  The standard Lerp function causes the object
     * to rotate in the wrong direction and looks stupid. Clerp fixes that.
     */
	public static float Clerp(float start , float end, float value){
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min)/2.0f);//half the distance between min and max
		float retval = 0.0f;
		float diff = 0.0f;
		
		if((end - start) < -half){
			diff = ((max - start)+end)*value;
			retval =  start+diff;
		}
		else if((end - start) > half){
			diff = -((max - end)+start)*value;
			retval =  start+diff;
		}
		else retval =  start+(end-start)*value;
		
		// Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
		return retval;
	}

	// quadratic equation is a second order of polynomial       equation in       a single variable 
	// x = [ -b +/- sqrt(b^2 - 4ac) ] / 2a
//	public static void SolveQuadratic(float a, float b, float c)  {
//		float sqrtpart = b * b - 4 * a * c;
//		float x, x1, x2, img;
//		if (sqrtpart > 0)
//		{
//			x1 = (-b + System.Math.Sqrt(sqrtpart)) / (2 * a);
//			x2 = (-b - System.Math.Sqrt(sqrtpart)) / (2 * a);
//			Debug.Log("Two Real Solutions: "+x1+" or  "+x2);
//		}
//		else if (sqrtpart < 0)
//		{
//			sqrtpart = -sqrtpart;
//			x = -b / (2 * a);
//			img = System.Math.Sqrt(sqrtpart) / (2 * a);
//			Debug.Log("Two Imaginary Solutions: "+(x + img));
//		}
//		else
//		{
//			x = (-b + System.Math.Sqrt(sqrtpart)) / (2 * a);
//			Debug.Log("One Real Solution: "+ x);
//		}
//	}
	public static void SolveQuadratic(float a, float b, float c, out float x1, out float x2){
		//Calculate the inside of the square root
//		float insideSquareRoot = (b * b) - 4 * a * c;
//		
//		if (insideSquareRoot < 0)
//		{
//			//There is no solution
//			x1 = float.NaN;
//			x2 = float.NaN;
//		}
//		else
//		{
//			//Compute the value of each x
//			//if there is only one solution, both x's will be the same
//			float t = (float)(-0.5f * (b + Math.Sign(b) * Math.Sqrt(insideSquareRoot)));
//			x1 = c / t;
//			x2 = t / a;
//		}
		x1=-b/2/a+Mathf.Pow(Mathf.Pow(b,2)-4*a*c,0.5f)/2/a;
		
		x2=-b/2/a-Mathf.Pow(Mathf.Pow(b,2)-4*a*c,0.5f)/2/a;
	}
	
}