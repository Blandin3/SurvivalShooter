using UnityEngine;

// Moves the AR rig itself rather than the camera - the camera's local pose is driven every
// frame by the device's AR tracking, so writing to its transform would just get overwritten.
// Translating the rig's parent transform shifts the whole tracked space instead, which is the
// correct way to add virtual locomotion on top of AR Foundation.
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("The on-screen joystick (e.g. FixedJoystick from the Joystick Pack) placed on the HUD canvas.")]
    public Joystick joystick;

    [Tooltip("The 'XR Origin (AR Rig)' transform to move. Do NOT assign the Main Camera itself.")]
    public Transform xrOrigin;

    [Tooltip("Used only to read facing direction so movement is relative to where the player is looking.")]
    public Camera viewCamera;

    public float moveSpeed = 1.5f;

    void Awake()
    {
        if (viewCamera == null) viewCamera = Camera.main;
    }

    void Update()
    {
        if (joystick == null || xrOrigin == null) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing) return;

        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f) return;

        Transform facing = viewCamera != null ? viewCamera.transform : xrOrigin;
        Vector3 forward = facing.forward;
        Vector3 right = facing.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * v + right * h) * moveSpeed * Time.deltaTime;
        xrOrigin.position += move;
    }
}
