using UnityEngine;

public class OcclusionCheck : MonoBehaviour
{
	public bool useDotProductMethod = true;

	[SerializeField] private Transform point1, point2;

	void Update()
	{
		Debug.DrawLine(Vector3.zero, point2.position, Color.yellow);
		Debug.DrawLine(point1.position, point2.position, Color.grey);
		bool occluded = false;

		if (useDotProductMethod)
		{
			Vector3 lowPoint = lowestPointOnLine(point1.position, point2.position);
			Debug.DrawLine(point2.position, lowPoint, Color.green); // visualize dot product
			occluded = lowPoint.magnitude < transform.localScale.x / 2;
			Color occlusionLineColor = occluded ? Color.red : Color.cyan;
			// check for false positive - which occurs when the 2 points on the same side of the sphere with one roughly above the other.
			// in this case the dot product length will be longer than the distance between the 2 points
			if (occluded)
			{
				if ((point2.position - lowPoint).magnitude > (point1.position - point2.position).magnitude) // false positive
				{
					occluded = false;
					occlusionLineColor = Color.white;
				}
			}

			Debug.DrawLine(Vector3.zero, lowPoint, occlusionLineColor);
		}
		else // use Oscar's triangle method
		{
			float lowPointDistance = distanceToLine(point1.position, point2.position);
			occluded = lowPointDistance < transform.localScale.x / 2;
			Color occlusionLineColor = occluded ? Color.red : Color.cyan;
			Debug.DrawLine(Vector3.zero, point1.position, occlusionLineColor);
		}
	}

	private Vector3 lowestPointOnLine(Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 line = (lineStart - lineEnd).normalized;
		float dotProduct = Vector3.Dot(line, -lineEnd);
		Vector3 lowestPointOnLine = lineEnd + line * dotProduct;
		return (lowestPointOnLine);
	}

	private float distanceToLine(Vector3 lineStart, Vector3 lineEnd)
	{
		float distance = (lineStart - lineEnd).magnitude;
		float adjacent = distance * lineStart.magnitude / (lineStart.magnitude + lineEnd.magnitude);
		float opposite = Mathf.Sqrt(lineStart.magnitude * lineStart.magnitude - adjacent * adjacent);
		return (opposite);
	}
}
