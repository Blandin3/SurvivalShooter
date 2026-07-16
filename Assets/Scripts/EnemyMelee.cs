using UnityEngine;

public class EnemyMelee : EnemyBase
{
    public float attackRange = 1.8f;

    [Tooltip("Determines this enemy's turn in the attack rotation. Leave at -1 to auto-assign from hierarchy order (first child attacks first).")]
    public int attackOrderIndex = -1;

    protected override float EngageRange => attackRange;

    protected override void Awake()
    {
        base.Awake();

        if (attackOrderIndex < 0)
        {
            attackOrderIndex = transform.parent != null ? transform.GetSiblingIndex() : 0;
        }

        ResetAttackTimer();
    }

    // Staggers this enemy's first eligible attack time based on its fixed order index,
    // so enemies don't all swing at once, and re-applying it after a respawn reproduces
    // the same attack order every time.
    public void ResetAttackTimer()
    {
        int siblingCount = transform.parent != null ? Mathf.Max(1, transform.parent.childCount) : 1;
        float offset = attackOrderIndex * (attackCooldown / siblingCount);
        lastAttackTime = Time.time - attackCooldown + offset;
    }

    protected override void TryAttack(float distanceToPlayer)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        PlayAttackFeedback();
        playerHealth.TakeDamage(damage);
    }
}
