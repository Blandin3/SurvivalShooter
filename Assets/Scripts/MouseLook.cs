using UnityEngine;

// Editor/desktop testing convenience: lets the mouse directly control look direction instead
// of needing to hold the XR Simulation environment's right-click-drag navigation. Has no effect
// on a real device (no mouse there) - actual look direction on-device comes from physically
// rotating the phone, which AR tracking already handles.
//
// Yaw (left/right) rotates the XR Origin rather than the camera, so it adds on top of AR
// tracking instead of fighting it every frame - same reasoning PlayerMovement.cs uses for
// translating the rig instead of the camera. Pitch (up/down) is applied to the camera directly
// and clamped, since AR tracking doesn't drive pitch on its own in the Simulation's idle pose.
public class MouseLook : MonoBehaviour
{
    [Tooltip("The 'XR Origin (AR Rig)' transform. Do NOT assign the Main Camera itself.")]
    public Transform xrOrigin;

    [Tooltip("The Main Camera transform.")]
    public Transform cameraTransform;

    public float sensitivity = 3f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    float pitch;

    void Start()
    {
        if (cameraTransform != null)
        {
            float startAngle = cameraTransform.localEulerAngles.x;
            pitch = startAngle > 180f ? startAngle - 360f : startAngle;
        }
    }

    void Update()
    {
        if (xrOrigin == null && cameraTransform == null) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        if (xrOrigin != null && mouseX != 0f)
        {
            xrOrigin.Rotate(0f, mouseX, 0f, Space.World);
        }

        if (cameraTransform != null && mouseY != 0f)
        {
            pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
            Vector3 euler = cameraTransform.localEulerAngles;
            cameraTransform.localRotation = Quaternion.Euler(pitch, euler.y, euler.z);
        }
    }
}
