using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Assertions.Must;

public class Gravity : MonoBehaviour
{
	public MassiveObject[] objects;
	public bool simulationRunning = false;
	[SerializeField] private FieldPoint fieldPointPrefab;

	public static float fieldLineMaxLength = 0;			// is calculated in Start()

	public bool infiniteSpeedOfLight = true;
	public float speedOfLight; // actually the distance a field propogation travels in 1 second
	
	public static float 
		fieldScale = 10, // size of the field area (one side of the square)
		gravitationalConstant = 0.5f;

	[HideInInspector] public FieldPoint[,] field;

	public static int fieldResolution = 30; // number of points on each side of the field

	public class PointInSpaceTime
	{
		public Vector3 position;
		public Vector3 gravitation;
	}

    void Start()
    {
		field = new FieldPoint[fieldResolution, fieldResolution];
		for (int i = 0; i < fieldResolution; i++)
		{
			for (int j = 0; j < fieldResolution; j++)
			{
				FieldPoint newPoint = Instantiate(fieldPointPrefab);
				newPoint.transform.position = new Vector3((-fieldScale / 2) + i * fieldScale / fieldResolution, transform.position.y, (-fieldScale / 2) + j * fieldScale / fieldResolution);
				newPoint.fieldVector = Vector3.zero;
				field[i, j] = newPoint;
			}
		}

		fieldLineMaxLength = (fieldScale / fieldResolution) * 0.9f; // lines always reach 90% to the next field poi
	}

    void FixedUpdate()
    {
		// reset all gravitation vectors
		for (int i = 0; i < fieldResolution; i++)
		{
			for (int j = 0; j < fieldResolution; j++)
			{
				field[i, j].fieldVector = Vector3.zero;
			}
		}

		foreach (MassiveObject m in objects)
		{
			// add gravity from this object to the field
			for (int i = 0; i < fieldResolution; i++)
			{
				for (int j = 0; j < fieldResolution; j++)
				{
					Vector3 pointToObject = m.transform.position - field[i, j].transform.position;
					bool objectIsVisibleFromHere = false;

					if (!infiniteSpeedOfLight) 
					{
						// check the distance to the object - if enough time has passed for this point to see its current position, use that
						// otherwise check back down the stack of previous positions until we find a previous position that is visible

						for (int k = m.previousPositions.Count-1; k >= 0 ; k--)
						{
							float timeSinceObjectGotThere = Time.time - m.previousPositions[k].time;
							pointToObject = m.previousPositions[k].position - field[i, j].transform.position;
							if (pointToObject.magnitude < timeSinceObjectGotThere * speedOfLight)
							{
								// it is close enough for us to see, break out of the for loop and use this point
								objectIsVisibleFromHere = true;
								break;
							}
						}
					}

					// if the point is inside the object we need to reduce the strength of the field, falling off to zero when the point and object are coincident
					// we can reduce the mass by the proportion of the distance inside cubed for a spherical object
					// https://en.wikipedia.org/wiki/Shell_theorem

					if (objectIsVisibleFromHere || infiniteSpeedOfLight)
					{
						float proportionOfMass = 1;
						if (pointToObject.magnitude < m.transform.localScale.x / 2)
						{
							proportionOfMass = Mathf.Pow(Mathf.InverseLerp(0, m.transform.localScale.x / 2, pointToObject.magnitude), 3);
						}

						Vector3 fieldContribution = NewtonianGravity(m.mass * proportionOfMass, pointToObject);
						field[i, j].fieldVector += fieldContribution;
					}
				}
			}
		}

		foreach (MassiveObject m in objects)
		{
			// add the gravitation vector for each other massive object
			if (simulationRunning)
			{
				foreach (MassiveObject otherObject in objects)
				{
					if (!GameObject.Equals(m, otherObject))
					{
						Vector3 directionToOtherObject = otherObject.gameObject.transform.position - m.transform.position;
						m.velocity += NewtonianGravity(otherObject.mass, directionToOtherObject) / 10000f;
					}
				}
			}
		}
    }

	public static Vector3 NewtonianGravity(float mass, Vector3 direction)
	{
		if (direction.magnitude < float.Epsilon) // very small distance
			return (Vector3.zero);
		else
			return (direction.normalized * (gravitationalConstant * (mass / (Mathf.Pow(direction.magnitude, 2)))));
	}

	public Vector3 SampleField(Vector3 samplePosition)
	{
		// return zero if the sample point is outside the bounds of the field
		if (samplePosition.x < field[0, 0].transform.position.x ||
			samplePosition.x >= field[fieldResolution - 1, 0].transform.position.x ||
			samplePosition.z < field[0, 0].transform.position.z ||
			samplePosition.z >= field[0, fieldResolution - 1].transform.position.z)
			{
				return Vector3.zero;
			}

		// find the samplePoint as a position in 0 to 1 range of the entire field
		Vector2 samplePoint = new Vector2(
			MathUtils.Remap(samplePosition.x, field[0, 0].transform.position.x, field[fieldResolution - 1, 0].transform.position.x, 0, 1),
			MathUtils.Remap(samplePosition.z, field[0, 0].transform.position.z, field[0, fieldResolution - 1].transform.position.z, 0, 1));

		// find 4 field points surrounding this sample point
		FieldPoint pointA = field[Mathf.FloorToInt((fieldResolution - 1) * samplePoint.x), Mathf.FloorToInt((fieldResolution - 1) * samplePoint.y)];
		FieldPoint pointB = field[Mathf.CeilToInt((fieldResolution - 1) * samplePoint.x), Mathf.FloorToInt((fieldResolution - 1) * samplePoint.y)];
		FieldPoint pointC = field[Mathf.FloorToInt((fieldResolution - 1) * samplePoint.x), Mathf.CeilToInt((fieldResolution - 1) * samplePoint.y)];
		FieldPoint pointD = field[Mathf.CeilToInt((fieldResolution - 1) * samplePoint.x), Mathf.CeilToInt((fieldResolution - 1) * samplePoint.y)];

		// find the local sample point - the sample position within the 4 selected field points, expressed as 0 to 1 range
		Vector2 localSamplePoint = new Vector2(
			MathUtils.Remap(samplePosition.x, pointA.transform.position.x, pointB.transform.position.x, 0, 1),
			MathUtils.Remap(samplePosition.z, pointA.transform.position.z, pointC.transform.position.z, 0, 1));

		// perform bi-linear interpolation to find the field value at any given position (within the field bounds)
		// We have 4 known field points forming a rectangle ABCD, in order: lower-left, lower-right, top-left top-right
		// Given a sample point somewhere inside this rectangle, we derive the interpolated Vector3 from its position
		// The sample point has coordinates from 0 to 1 with 0,0 starting at corner C
		Vector3 bottomEdgeInterpolation = Vector3.Lerp(pointA.fieldVector, pointB.fieldVector, localSamplePoint.x);
		Vector3 topEdgeInterpolation = Vector3.Lerp(pointC.fieldVector, pointD.fieldVector, localSamplePoint.x);
		Vector3 finalInterpolation = Vector3.Lerp(bottomEdgeInterpolation, topEdgeInterpolation, localSamplePoint.y);

		return finalInterpolation;
	}

	
}
