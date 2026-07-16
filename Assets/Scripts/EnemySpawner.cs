using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject meleeEnemyPrefab;
    public GameObject shooterEnemyPrefab;
    public float spawnInterval = 4f;
    public float spawnRadius = 3f;
    public AudioClip spawnSound;

    Transform anchor;
    AudioSource audioSource;
    Coroutine spawnRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void BeginSpawning(Transform placementAnchor)
    {
        anchor = placementAnchor;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval * DifficultySettings.SpawnIntervalMultiplier);
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        if (anchor == null) return;

        GameObject prefab = Random.value < 0.5f ? meleeEnemyPrefab : shooterEnemyPrefab;
        if (prefab == null) return;

        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = anchor.position + new Vector3(offset.x, 0f, offset.y);

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, anchor);

        var health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.SetMaxHealth(health.maxHealth * DifficultySettings.EnemyHealthMultiplier);
        }

        var melee = enemy.GetComponent<EnemyMelee>();
        if (melee != null) melee.damage *= DifficultySettings.EnemyDamageMultiplier;

        var shooter = enemy.GetComponent<ShooterEnemy>();
        if (shooter != null) shooter.damage *= DifficultySettings.EnemyDamageMultiplier;

        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
}
