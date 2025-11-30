using UnityEngine;

public class MathUtils : MonoBehaviour
{
   	/// <summary>
	/// Remap an input value from range A to range B
	/// </summary>
	/// <param name="value"></param>
	/// <param name="minA">Input range minimum</param>
	/// <param name="maxA">Input range maximum</param>
	/// <param name="minB">Output range minimum</param>
	/// <param name="maxB">Output range maximum</param>
	/// <returns></returns>
	public static float Remap(float value, float minA, float maxA, float minB, float maxB)
	{
		float t = Mathf.InverseLerp(minA, maxA, value);
		return (Mathf.Lerp(minB, maxB, t));
	}
}
