using UnityEngine;

public class FieldPoint : MonoBehaviour
{
	public Vector3 fieldVector;

	[SerializeField] private LineRenderer myLineRenderer;

	private const float lineThicknessMin = 0.02f, lineThicknessMax = 0.04f;

	private void FixedUpdate() 
	{
		Vector3 lineVector = fieldVector * Gravity.fieldLineMaxLength;
		myLineRenderer.startWidth = lineThicknessMin;

		// clamp the line length so we don't get super long lines, instead make the line thicker if it's a very long vector
		if (lineVector.magnitude > Gravity.fieldLineMaxLength)
		{
			float lineThickness = Mathf.Clamp(Mathf.Lerp(Gravity.fieldLineMaxLength, Gravity.fieldLineMaxLength * 3, lineVector.magnitude), 0, 1);
			myLineRenderer.startWidth = Mathf.Lerp(lineThicknessMin, lineThicknessMax, lineThickness);
			lineVector = Vector3.ClampMagnitude(lineVector, Gravity.fieldLineMaxLength);
		}

		myLineRenderer.SetPosition(1, lineVector);
	}
}
