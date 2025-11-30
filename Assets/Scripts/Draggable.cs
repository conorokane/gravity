using UnityEngine;

public class Draggable : MonoBehaviour
{
	private Vector3 mouseOffset = Vector3.zero;

	public bool dragging;
    
	private void OnMouseDown() 
	{
		mouseOffset = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y)) - transform.position;
		dragging = true;
	}

	private void OnMouseDrag() 
	{
		//Vector2 mousePosition = new Vector2(Input.mousePosition.x, Camera.main.pixelHeight - Input.mousePosition.y);
		transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y)) 
			- mouseOffset;
	}

	private void Update() 
	{
		if (Input.GetMouseButtonUp(0))
			dragging = false;	
	}
}
