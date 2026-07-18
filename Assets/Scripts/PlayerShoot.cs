using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera shootCamera;
    public ObjectPool bulletPool;
    public Transform muzzle;
    public float damage = 20f;
    public float aimRange = 100f;
    public AudioClip shootSound;

    AudioSource audioSource;

    void Awake()
    {
        if (shootCamera == null) shootCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Fire from the gun model's actual barrel node instead of a hand-placed guess -
        // BUTTSTOCK_BARREL is a named bone/node inside the FP_ScarH model hierarchy.
        if (muzzle == null)
        {
            muzzle = FindDeepChild(transform, "BUTTSTOCK_BARREL");
            if (muzzle == null)
            {
                Debug.LogWarning("PlayerShoot.Awake(): no child named 'BUTTSTOCK_BARREL' found under '" + gameObject.name + "', falling back to the camera as the fire origin.", this);
            }
        }
    }

    static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform found = FindDeepChild(child, name);
            if (found != null) return found;
        }

        return null;
    }

    void Update()
    {
        // Editor/desktop testing convenience. On device, wire a UI "Fire" button to Fire() instead,
        // since a bare screen tap is normally reserved for AR plane placement / camera look.
        if (Input.GetMouseButtonDown(1))
        {
            Fire();
        }
    }

    [ContextMenu("Test Fire")]
    public void Fire()
    {
        Debug.Log("PlayerShoot.Fire() called on " + gameObject.name + ".", this);

        if (bulletPool == null)
        {
            Debug.LogWarning("PlayerShoot.Fire(): bulletPool is not assigned.", this);
            return;
        }

        if (shootCamera == null)
        {
            Debug.LogWarning("PlayerShoot.Fire(): shootCamera is not assigned and Camera.main was null at Awake().", this);
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
        {
            Debug.LogWarning("PlayerShoot.Fire(): blocked because GameManager.State is " + GameManager.Instance.State + ", not Playing.", this);
            return;
        }

        Transform origin = muzzle != null ? muzzle : shootCamera.transform;

        // Find what's actually under the crosshair (screen center), then aim the bullet from the
        // muzzle toward that exact point - otherwise a muzzle offset from the camera makes the bullet
        // travel parallel to the aim direction instead of converging on what you're looking at.
        Ray aimRay = new Ray(shootCamera.transform.position, shootCamera.transform.forward);
        Vector3 aimPoint = Physics.Raycast(aimRay, out RaycastHit hit, aimRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
            ? hit.point
            : aimRay.GetPoint(aimRange);

        Vector3 direction = (aimPoint - origin.position).normalized;

        // If the muzzle sits far enough forward of the camera that the aim point ends up
        // behind it (e.g. a close target), converging toward that point would aim the bullet
        // backward instead of where the player is actually looking - fall back to the camera's
        // forward direction instead of ever firing behind the player.
        if (Vector3.Dot(direction, shootCamera.transform.forward) < 0f)
        {
            direction = shootCamera.transform.forward;
        }

        GameObject bulletObj = bulletPool.Get(origin.position, Quaternion.LookRotation(direction));

        var projectile = bulletObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(bulletPool, damage);
        }
        else
        {
            Debug.LogWarning("PlayerShoot.Fire(): spawned bullet '" + bulletObj.name + "' has no Projectile component - it will just sit there.", this);
        }

        Debug.Log("PlayerShoot.Fire(): bullet spawned at " + origin.position + " active=" + bulletObj.activeInHierarchy + ".", this);

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
