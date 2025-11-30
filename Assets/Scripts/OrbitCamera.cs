/// Orbit camera - left-click drags orbit the camera around its origin
/// right-click drags look around FPS style

using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform orbitX, orbitY, lookX, lookY, zoomPivot;
    [SerializeField] private float orbitSpeed, lookSpeed, zoomSpeed;
    [SerializeField] private Vector2 orbitClampX, lookClampX, zoomClamp;

    private Vector2 orbitDrag = Vector2.zero, lookDrag = Vector2.zero;
    private float dt = 0;

    void LateUpdate()
    {
        orbitDrag = Vector2.zero;
        lookDrag = Vector2.zero;
        dt = Time.deltaTime;

        // read mouse drags
        if (Input.GetMouseButton(0))
        {
            orbitDrag = new Vector2(Input.mousePositionDelta.x, -Input.mousePositionDelta.y);
            orbitX.Rotate(orbitSpeed * dt * Vector3.right * orbitDrag.y);
            orbitY.Rotate(orbitSpeed * dt * Vector3.up * orbitDrag.x);
            float orbitXRotation = MathUtils.ConstrainAngle(orbitX.localEulerAngles.x);
            orbitX.localEulerAngles = new Vector3(Mathf.Clamp(orbitXRotation, orbitClampX.x, orbitClampX.y), 0, 0);
        }
        else if (Input.GetMouseButton(1))
        {
            lookDrag = new Vector2(Input.mousePositionDelta.x, -Input.mousePositionDelta.y);
            lookX.Rotate(lookSpeed * dt * Vector3.right * lookDrag.y);
            lookY.Rotate(lookSpeed * dt * Vector3.up * lookDrag.x);
            float lookXRotation = MathUtils.ConstrainAngle(lookX.localEulerAngles.x);
            lookX.localEulerAngles = new Vector3(Mathf.Clamp(lookXRotation, lookClampX.x, lookClampX.y), 0, 0);
        }

        //zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            zoomPivot.Translate(zoomSpeed * dt * Vector3.forward * Input.mouseScrollDelta.y * Mathf.Abs(zoomPivot.localPosition.z));
            zoomPivot.localPosition = new Vector3(0, 0, Mathf.Clamp(zoomPivot.localPosition.z, zoomClamp.x, zoomClamp.y));
        }
    }
}
