using UnityEngine;

public class TemporalPotential : MonoBehaviour
{
	[SerializeField] private Transform point1, point2, planet;
	private Material myMaterial;

	private void Start() 
	{
		myMaterial = GetComponent<Renderer>().material;	
	}

    void Update()
    {
		myMaterial.SetVector("_Point1", point1.position);
		myMaterial.SetVector("_Point2", point2.position);
		myMaterial.SetVector("_MassiveBody", planet.position);
    }
}
