using UnityEngine;

public class FlowPoint : MonoBehaviour
{
	[SerializeField] private AnimationCurve opacityCurve;

	public Vector3 startPosition;
	public Rigidbody myRigidbody;
	
	private float distanceTravelled = 0, maxDistance = 1;
	private Material myMaterial;

	private void Start() 
	{
		myRigidbody = GetComponent<Rigidbody>();
		myMaterial = GetComponent<Renderer>().material;
	}

	private void FixedUpdate() 
	{
		distanceTravelled = Vector3.SqrMagnitude(new Vector3(transform.position.x, 0, transform.position.z) - startPosition); // ignore Y motion for distance calculation
		if (distanceTravelled >= maxDistance)
		{
			Reset();
		}

		float newAlpha = opacityCurve.Evaluate(MathUtils.Remap(0, maxDistance, 0, 1, distanceTravelled));
		myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, newAlpha);
	}

	public void Reset()
	{
		transform.position = startPosition;
		distanceTravelled = 0;
	}
}
