using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassiveObject : MonoBehaviour
{
	[SerializeField] Transform myVelocityIndicator;

	public float mass;
	public Vector3 velocity;

	[HideInInspector] public List<HistoricPosition> previousPositions;

	private const float 
		velocityIndicatorScale = 100f,
		previousPositionsTimeStep = 0.05f;	// time between snapshots in seconds
		
	public static int numberOfPreviousPositions = 100;
	private Gravity gravityField;

	// stores the past position of an object, with a timestamp
	public class HistoricPosition
	{
		public Vector3 position;
		public float time;
	}

	private void Start() 
	{
		gravityField = FindFirstObjectByType<Gravity>();

		previousPositions = new List<HistoricPosition>();
		HistoricPosition initialPosition = new HistoricPosition
		{
			position = transform.position,
			time = Time.time
		};
		previousPositions.Add(initialPosition);
		velocity = (myVelocityIndicator.position - transform.position) / velocityIndicatorScale;
		myVelocityIndicator.SetParent(null);

		StartCoroutine(RecordPositions());
	}

	private void Update() 
	{
		myVelocityIndicator.position = transform.position + (velocity * velocityIndicatorScale);	
	}

	private void FixedUpdate() 
	{
		if (gravityField.simulationRunning)
		{
			transform.Translate(velocity);

			// wrap around if we leave the world bounds
			if (transform.position.x < -Gravity.fieldScale / 2)
				transform.Translate(Vector3.right * Gravity.fieldScale);
			if (transform.position.x > Gravity.fieldScale / 2)
				transform.Translate(Vector3.left * Gravity.fieldScale);
			if (transform.position.z < -Gravity.fieldScale / 2)
				transform.Translate(Vector3.forward * Gravity.fieldScale);
			if (transform.position.z > Gravity.fieldScale / 2)
				transform.Translate(Vector3.back * Gravity.fieldScale);

			transform.Translate(Vector3.down * transform.position.y); // remove any Y axis drift
		}
	}

	private IEnumerator RecordPositions()
	{
		while (true)
		{
			HistoricPosition currentPosition = new HistoricPosition
			{
				position = transform.position,
				time = Time.time
			};
			previousPositions.Add(currentPosition);
			if (previousPositions.Count > numberOfPreviousPositions)
				previousPositions.Remove(previousPositions[0]);
			yield return new WaitForSeconds(previousPositionsTimeStep);
		}
	}
}
