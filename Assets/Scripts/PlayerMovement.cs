using UnityEngine;

// Moves the AR rig itself rather than the camera - the camera's local pose is driven every
// frame by the device's AR tracking, so writing to its transform would just get overwritten.
// Translating the rig's parent transform shifts the whole tracked space instead, which is the
// correct way to add virtual locomotion on top of AR Foundation.
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("On-screen button held to walk forward.")]
    public TouchButton forwardButton;

    [Tooltip("On-screen button held to walk backward.")]
    public TouchButton backButton;

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
        if (xrOrigin == null) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing) return;

        float v = 0f;
        if (forwardButton != null && forwardButton.IsHeld) v += 1f;
        if (backButton != null && backButton.IsHeld) v -= 1f;
        if (v == 0f) return;

        Transform facing = viewCamera != null ? viewCamera.transform : xrOrigin;
        Vector3 forward = facing.forward;
        forward.y = 0f;
        forward.Normalize();

        xrOrigin.position += forward * v * moveSpeed * Time.deltaTime;
    }
}
