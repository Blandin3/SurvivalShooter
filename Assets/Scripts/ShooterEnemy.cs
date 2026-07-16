using UnityEngine;

public class ShooterEnemy : EnemyBase
{
    public float stopDistance = 8f;
    public float shootRange = 10f;

    [Tooltip("If the player gets closer than this, the shooter backs away while continuing to fire instead of standing still.")]
    public float retreatDistance = 4f;

    public ObjectPool bulletPool;
    public Transform firePoint;

    protected override float EngageRange => stopDistance;
    protected override float RetreatRange => retreatDistance;

    // Called by EnemyHealth after a respawn so this enemy is immediately eligible to attack again.
    public void ResetAttackTimer()
    {
        lastAttackTime = Time.time - attackCooldown;
    }

    protected override void TryAttack(float distanceToPlayer)
    {
        if (distanceToPlayer > shootRange)
        {
            Debug.Log(gameObject.name + " ShooterEnemy.TryAttack(): blocked, distance " + distanceToPlayer.ToString("F2") + " > shootRange " + shootRange, this);
            return;
        }
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        FaceCamera();
        PlayAttackFeedback();

        if (bulletPool == null)
        {
            Debug.LogWarning(gameObject.name + " ShooterEnemy.TryAttack(): bulletPool is not assigned, can't fire.", this);
            return;
        }

        Transform origin = firePoint != null ? firePoint : transform;
        Vector3 targetPoint = playerCamera != null ? playerCamera.position : player.position + Vector3.up * 1.2f;
        Vector3 dir = (targetPoint - origin.position).normalized;

        GameObject bulletObj = bulletPool.Get(origin.position, Quaternion.LookRotation(dir));
        var projectile = bulletObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(bulletPool, damage);
        }

        Debug.Log(gameObject.name + " ShooterEnemy.TryAttack(): fired bullet from " + origin.position + " toward " + targetPoint + ".", this);
    }
}
