using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public AudioClip damageSound;
    public AudioClip deathSound;

    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        Debug.Log("PlayerHealth.TakeDamage(): " + gameObject.name + " (instance ID " + GetInstanceID() + ") took " + amount + " damage, now at " + currentHealth + "/" + maxHealth + ".", this);
        onDamaged?.Invoke();

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
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

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
    }
}
