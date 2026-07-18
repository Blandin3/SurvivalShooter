using UnityEngine;

// Shared chase/detect/face-player behavior for all enemy types. Direct kinematic
// movement, deliberately not NavMeshAgent-based: AR planes are only known at
// runtime, so there's no baked NavMesh to path on.
//
// Derived classes (EnemyMelee, ShooterEnemy) only decide two things:
//   - EngageRange: the distance at which this enemy stops closing in
//   - TryAttack: what "attacking" means once in range and off cooldown
public abstract class EnemyBase : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;
    public float detectionRange = 20f;

    public Animator animator;
    public string speedParam = "Speed";
    public string attackTrigger = "Attack";
    public AudioClip attackSound;

    [Tooltip("Corrects for models whose mesh doesn't face the same way as their Transform's forward axis (common with raw/unprocessed FBX exports). 0 = no correction. If the model faces sideways or backwards relative to its movement/target, adjust this in 90-degree steps while in Play mode until it looks right.")]
    public float visualYawOffset = 0f;

    protected Transform player;
    protected PlayerHealth playerHealth;
    protected Transform playerCamera;
    protected AudioSource audioSource;
    protected float lastAttackTime;
    float nextDebugLogTime;

    protected bool HasAnimatorController => animator != null && animator.runtimeAnimatorController != null;

    // Distance at which this enemy type stops closing in and switches to its attack behavior.
    protected abstract float EngageRange { get; }

    // Distance below which this enemy backs away from the player instead of standing still.
    // 0 (the default) disables retreating entirely - only ranged enemies should override this.
    protected virtual float RetreatRange => 0f;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogWarning(GetType().Name + ": found GameObject '" + playerObj.name + "' tagged 'Player', but it has no PlayerHealth component.", this);
            }
            else
            {
                Debug.Log(GetType().Name + ": targeting player '" + playerObj.name + "' (PlayerHealth instance ID " + playerHealth.GetInstanceID() + ").", this);
            }
        }
        else
        {
            Debug.LogWarning(GetType().Name + ": no GameObject tagged 'Player' found in the scene.", this);
        }

        playerCamera = Camera.main != null ? Camera.main.transform : null;
    }

    protected virtual void Update()
    {
        if (player == null || playerHealth == null || playerHealth.IsDead) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        // Face toward the camera (where the player is actually looking from) rather than the
        // "player" transform, which is often a hand/weapon prop sitting at an odd offset from
        // the camera - facing player.position can make the enemy visibly look past the player.
        Vector3 toFaceTarget = playerCamera != null ? playerCamera.position - transform.position : toPlayer;
        toFaceTarget.y = 0f;

        if (Time.time >= nextDebugLogTime)
        {
            nextDebugLogTime = Time.time + 1.5f;
            Debug.Log(gameObject.name + " (" + GetType().Name + "): distance=" + distance.ToString("F2") + " detectionRange=" + detectionRange + " engageRange=" + EngageRange + " retreatRange=" + RetreatRange, this);
        }

        if (distance > detectionRange)
        {
            SetSpeedAnim(0f);
            return;
        }

        if (distance > EngageRange)
        {
            Vector3 dir = toPlayer.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, visualYawOffset, 0f);
            SetSpeedAnim(moveSpeed);
        }
        else if (RetreatRange > 0f && distance < RetreatRange)
        {
            Vector3 dir = toPlayer.normalized;
            transform.position -= dir * moveSpeed * Time.deltaTime;
            FacePlayer(toFaceTarget);
            SetSpeedAnim(moveSpeed);
            TryAttack(distance);
        }
        else
        {
            FacePlayer(toFaceTarget);
            SetSpeedAnim(0f);
            TryAttack(distance);
        }
    }

    protected void SetSpeedAnim(float speed)
    {
        if (HasAnimatorController && !string.IsNullOrEmpty(speedParam))
        {
            animator.SetFloat(speedParam, speed);
        }
    }

    protected void FacePlayer(Vector3 toPlayer)
    {
        if (toPlayer.sqrMagnitude < 0.0001f) return;
        Quaternion lookRotation = Quaternion.LookRotation(toPlayer.normalized) * Quaternion.Euler(0f, visualYawOffset, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }

    // Snaps to face the camera rather than the abstract "player" transform - used right
    // before firing, since the player transform (e.g. a hand/weapon prop) can sit at an
    // odd offset from the camera, while the camera is where the player actually is looking
    // from and where a shot should visibly be aimed toward.
    protected void FaceCamera()
    {
        if (playerCamera == null) return;

        Vector3 toCamera = playerCamera.position - transform.position;
        toCamera.y = 0f;
        if (toCamera.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(toCamera.normalized) * Quaternion.Euler(0f, visualYawOffset, 0f);
    }

    protected void PlayAttackFeedback()
    {
        if (HasAnimatorController && !string.IsNullOrEmpty(attackTrigger))
        {
            animator.SetTrigger(attackTrigger);
        }

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    protected abstract void TryAttack(float distanceToPlayer);
}
