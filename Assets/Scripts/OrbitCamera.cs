/// Orbit camera - left-click drags orbit the camera around its origin
/// right-click drags look around FPS style

using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform orbitX, orbitY, lookX, lookY, zoomPivot;
    [SerializeField] private float orbitSpeed, lookSpeed, zoomSpeed;
    [SerializeField] private Vector2 orbitClampX, lookClampX, zoomClamp;

    void Update()
    {
        Vector2 orbitDrag = Vector2.zero;
        Vector2 lookDrag = Vector2.zero;

        // read mouse drags
        if (Input.GetMouseButton(0))
        {
            orbitDrag = new Vector2(-Input.mousePositionDelta.x, Input.mousePositionDelta.y);
        }
        else if (Input.GetMouseButton(1))
        {
            lookDrag = new Vector2(Input.mousePositionDelta.x, -Input.mousePositionDelta.y);
        }

        // apply rotations
        orbitX.Rotate(orbitSpeed * Time.deltaTime * Vector3.right * orbitDrag.y);
        orbitY.Rotate(orbitSpeed * Time.deltaTime * Vector3.up * orbitDrag.x);
        lookX.Rotate(lookSpeed * Time.deltaTime * Vector3.right * lookDrag.y);
        lookY.Rotate(lookSpeed * Time.deltaTime * Vector3.up * lookDrag.x);

        // clamp rotation
        float orbitXRotation = MathUtils.ConstrainAngle(orbitX.localEulerAngles.x);
        float lookXRotation = MathUtils.ConstrainAngle(lookX.localEulerAngles.x);

        orbitX.localEulerAngles = new Vector3(Mathf.Clamp(orbitXRotation, orbitClampX.x, orbitClampX.y), 0, 0);
        lookX.localEulerAngles = new Vector3(Mathf.Clamp(lookXRotation, lookClampX.x, lookClampX.y), 0, 0);

        //zoom
        zoomPivot.Translate(zoomSpeed * Time.deltaTime * Vector3.forward * Input.mouseScrollDelta.y * Mathf.Abs(zoomPivot.localPosition.z));
        zoomPivot.localPosition = new Vector3(0, 0, Mathf.Clamp(zoomPivot.localPosition.z, zoomClamp.x, zoomClamp.y));
    }
}
