using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;

public class LaserBeam : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private Transform photonPrefab;

	private List<Transform> photons;
	private List<Vector3> photonVelocities;
	private int maxPhotons = 200;
	private float speedOfLight = 3f;
	private float gravityStrength = 0.005f;
	private float tickRate = 0.05f;
	private Gravity gravity;


	private void Start() 
	{
		photons = new List<Transform>();
		photonVelocities = new List<Vector3>();
		gravity = FindFirstObjectByType<Gravity>();
		speedOfLight *= tickRate; // speed of light becomes distance travelled per tick instead of per second

		StartCoroutine(PropogatePhotons());
	}

	private IEnumerator PropogatePhotons()
	{
		while (true)
		{
			yield return new WaitForSeconds(tickRate);

			Transform newPhoton = Instantiate(photonPrefab);
			newPhoton.transform.position = this.transform.position;
			Vector3 velocity = transform.forward * speedOfLight;
						
			photons.Add(newPhoton);
			photonVelocities.Add(velocity);

			if (photons.Count > maxPhotons)
			{
				Transform deadPhoton = photons[0];
				photons.Remove(photons[0]);
				photonVelocities.Remove(photonVelocities[0]);
				Destroy(deadPhoton.gameObject);
			}

			for (int i = 0; i < photons.Count; i++)
			{
				Vector3 previousPosition = photons[i].position;
				Vector3 newPosition = photons[i].position + photonVelocities[i];

				// adjust for gravity
				newPosition += gravity.SampleField(newPosition) * gravityStrength;
				photonVelocities[i] = (newPosition - previousPosition).normalized * speedOfLight;
				photons[i].position = newPosition;
			}
		}
	}

	private void Update() 
	{
		transform.LookAt(target);
		
	}
}
