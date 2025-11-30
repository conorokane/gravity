using UnityEngine;

public class StraightLine : MonoBehaviour
{
	[SerializeField] private Transform lineEnd;
	[SerializeField] private LineRenderer myLine;

	private Vector3[] linePoints;
	private const int lineResolution = 100;
	private const float lineBendStrength = 0.1f;
	private Gravity gravity;

	private void Start() 
	{
		linePoints = new Vector3[lineResolution];
		gravity = FindFirstObjectByType<Gravity>();
		myLine.positionCount = lineResolution;
		lineEnd.SetParent(null);
	}

	private void FixedUpdate() 
	{
		// calculate line points
		for (int i = 0; i < lineResolution; i++)
		{
			float t = (float)i / lineResolution;
			linePoints[i] = Vector3.Lerp(transform.position, lineEnd.position, t);

			// lookup field vector at each points location
			Vector3 fieldVector = gravity.SampleField(linePoints[i]);

			// offset line based on field
			linePoints[i] += fieldVector * lineBendStrength;
		}

		myLine.SetPositions(linePoints);
	}	
}
