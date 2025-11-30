using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GravityFlowVisualization : MonoBehaviour
{
	public bool showTimeDimension;

	[SerializeField] private Gravity gravityField;
	[SerializeField] private FlowPoint flowPointPrefab;
	[SerializeField] private float flowMultiplierSpace, flowMultiplierTime;
	
	private FlowPoint[,] flowPoints;
	private bool updating = false;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);
		flowPoints = new FlowPoint[Gravity.fieldResolution, Gravity.fieldResolution];
		initialize();
	}

	private void initialize()
	{
		// create one flow point on each field point of the parent gravity field
		for (int i = 0; i < Gravity.fieldResolution; i++)
		{
			for (int j = 0; j < Gravity.fieldResolution; j++)
			{
				flowPoints[i,j] = Instantiate(flowPointPrefab, gravityField.field[i, j].transform.position, quaternion.identity);
				flowPoints[i, j].startPosition = flowPoints[i, j].transform.position;
			}
		}
		updating = true;
	}

	private void FixedUpdate()
	{
		if (!updating) return;

		for (int i = 0; i < Gravity.fieldResolution; i++)
		{
			for (int j = 0; j < Gravity.fieldResolution; j++)
			{
				Vector3 localGravity = gravityField.SampleField(flowPoints[i, j].transform.position);
				flowPoints[i, j].myRigidbody.linearVelocity = localGravity * flowMultiplierSpace;

				// if any flow points are inside a planet (ignoring y offset), reset them
				foreach (MassiveObject m in gravityField.objects)
				{
					Vector3 horizontalPosition = new Vector3(flowPoints[i, j].transform.position.x, 0, flowPoints[i, j].transform.position.z);
					if ((m.transform.position - horizontalPosition).magnitude < m.transform.localScale.x / 2)
						flowPoints[i, j].Reset();
				}

				if (showTimeDimension)
				{
					// add a downward force to show falling behind in time
					flowPoints[i, j].myRigidbody.linearVelocity += Vector3.down * (1 - Mathf.Cos(localGravity.magnitude)) * flowMultiplierTime;
				}
			}
		}
	}
}
