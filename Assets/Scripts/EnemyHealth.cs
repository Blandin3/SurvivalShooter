using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 30f;
    public int scoreValue = 10;
    public string deathTrigger = "Death";
    public float respawnDelay = 3f;

    public AudioClip hitSound;

    public UnityEvent onDamaged;
    public UnityEvent onDeath;
    public UnityEvent onRespawn;

    public bool IsDead { get; private set; }

    float currentHealth;
    Vector3 spawnPosition;
    Quaternion spawnRotation;

    Collider col;
    Animator animator;
    AudioSource audioSource;
    Renderer[] renderers;
    EnemyMelee melee;
    ShooterEnemy shooter;

    void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        currentHealth = maxHealth;

        col = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        renderers = GetComponentsInChildren<Renderer>();
        melee = GetComponent<EnemyMelee>();
        shooter = GetComponent<ShooterEnemy>();
    }

    // Called by EnemySpawner after Instantiate to apply difficulty scaling,
    // since Awake() already runs (and caches currentHealth) before the spawner can touch maxHealth.
    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        currentHealth = newMax;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        onDamaged?.Invoke();

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    void Die()
    {
        IsDead = true;
        onDeath?.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterKill(scoreValue);
        }

        if (animator != null && animator.runtimeAnimatorController != null && !string.IsNullOrEmpty(deathTrigger))
        {
            animator.SetTrigger(deathTrigger);
        }

        SetVisible(false);
        if (col != null) col.enabled = false;
        if (melee != null) melee.enabled = false;
        if (shooter != null) shooter.enabled = false;

        // The GameObject itself stays active (only visuals/collider/AI are toggled) so
        // this Invoke keeps ticking - a deactivated GameObject would never fire it.
        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        currentHealth = maxHealth;
        IsDead = false;

        SetVisible(true);
        if (col != null) col.enabled = true;

        if (melee != null)
        {
            melee.enabled = true;
            melee.ResetAttackTimer();
        }

        if (shooter != null)
        {
            shooter.enabled = true;
            shooter.ResetAttackTimer();
        }

        onRespawn?.Invoke();
    }

    void SetVisible(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = visible;
        }
    }

    // Called by GameManager to instantly clear the field when the game ends,
    // regardless of any pending respawn from a death in progress.
    public void Wipe()
    {
        CancelInvoke();
        Destroy(gameObject);
    }
}
