using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;

public class TimeDilationViewer : MonoBehaviour
{
	[SerializeField] private Transform clock, clockHandPivot;
	[SerializeField] private LineRenderer myLine;
	[SerializeField] private float handTurnRate;
	[SerializeField] private Image greenFill, redFill;
	[SerializeField] private float observerVerticalSpeed, observerHorizontalAcceleration, clockVerticalSpeed, clockHorizontalAcceleration, velocityDifferenceMultiplier;
	[SerializeField] private Draggable observerDrag, clockDrag;

	private Gravity gravityField;
	private Vector3 previousObserverPosition, previousClockPosition;

	private void Start() 
	{
		clock.SetParent(null);
		gravityField = FindFirstObjectByType<Gravity>();
		previousObserverPosition = transform.position;
		previousClockPosition = clock.position;
	}

	private void FixedUpdate() 
	{
		float observerFieldStrength = 0, clockFieldStrength = 0;

		// sample the field to determine the local field strength
		observerFieldStrength = gravityField.SampleField(transform.position).magnitude;
		clockFieldStrength = gravityField.SampleField(clock.position).magnitude;

		float fieldDifference = (observerFieldStrength - clockFieldStrength);

		// vertical (linear) motion
		transform.Translate(Vector3.forward * observerVerticalSpeed);
		clock.Translate(Vector3.forward * clockVerticalSpeed);

		// horizontal (accelerating) motion
		transform.Translate(Vector3.right * observerHorizontalAcceleration * Mathf.Sin(Time.time * 5f));
		clock.Translate(Vector3.right * clockHorizontalAcceleration * Mathf.Sin(Time.time * 5f));

		// wrap around the top/bottom
		if (transform.position.z < -Gravity.fieldScale / 2)
				transform.Translate(Vector3.forward * Gravity.fieldScale);
		if (transform.position.z > Gravity.fieldScale / 2)
			transform.Translate(Vector3.back * Gravity.fieldScale);
		if (clock.position.z < -Gravity.fieldScale / 2)
				clock.Translate(Vector3.forward * Gravity.fieldScale);
		if (clock.position.z > Gravity.fieldScale / 2)
			clock.Translate(Vector3.back * Gravity.fieldScale);

		// dilation due to velocity difference
		Vector3 observerVelocity = transform.position - previousObserverPosition;
		Vector3 clockVelocity = clock.position - previousClockPosition;

		float velocityDifference = 0;
		// prevent dragging from creating a false velocity reading
		if (!observerDrag.dragging && !clockDrag.dragging)
		{
			velocityDifference = (clockVelocity - observerVelocity).magnitude * velocityDifferenceMultiplier;
		}

		// Debug.Log("Observer: " + observerVelocity.ToString() + " clock: " + clockVelocity.ToString() + " difference: " + velocityDifference.ToString());

		float cumulativeTimeDilation = (velocityDifference + fieldDifference) * handTurnRate;

		if (cumulativeTimeDilation > 0)
		{
			redFill.fillAmount = 0;
			greenFill.fillAmount = cumulativeTimeDilation / 360;
		}
		else
		{
			greenFill.fillAmount = 0;
			redFill.fillAmount = cumulativeTimeDilation / -360;
		}

		// set the clock hand and fill to the correct angle
		clockHandPivot.eulerAngles = Vector3.up * cumulativeTimeDilation;

		previousObserverPosition = transform.position;
		previousClockPosition = clock.position;
	}

	private void Update() 
	{
		myLine.SetPosition(0, transform.position);	
		myLine.SetPosition(1, clock.position);	
	}
}
