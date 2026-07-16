using UnityEngine;

// Poolable projectile. Requires a Rigidbody (kinematic, no gravity) and a
// trigger Collider on the prefab. Player-fired projectiles only damage
// objects tagged "Enemy"; enemy-fired projectiles only damage "Player".
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 10f;
    public float maxLifetime = 4f;
    public bool isPlayerProjectile = true;

    ObjectPool pool;
    float timer;

    public void Init(ObjectPool owningPool, float dmg)
    {
        pool = owningPool;
        damage = dmg;
    }

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= maxLifetime)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPlayerProjectile && !other.CompareTag("Enemy")) return;
        if (!isPlayerProjectile && !other.CompareTag("Player")) return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
        {
            damageable.TakeDamage(damage);
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Release(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
