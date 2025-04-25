using UnityEngine;
using System.Collections;

public class BossEnemy : Enemy
{
    [Header("Boss Attack Settings")]
    public float telegraphDuration = 2f;
    public float attackRadius = 3f;
    public float attackDamage = 50f;

    [Header("Boss Effects")]
    public GameObject telegraphEffectPrefab;
    public GameObject attackEffectPrefab;

    [HideInInspector] public GameObject telegraphEffectInstance;
    [HideInInspector] public Vector2 attackDirection;
    [HideInInspector] public float attackAngle;
    private GameObject attackEffect;

    protected override void Start()
    {
        base.Start();
        ChangeState(new StateChase());
    }

    public override bool IsWithinAttackRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) <= attackRadius;
    }

    public void StartTelegraph()
    {
        if (IsDead || player == null) return;
        StartCoroutine(TelegraphAndAttack());
    }

    private IEnumerator TelegraphAndAttack()
    {
        attackDirection = (player.position - transform.position).normalized;
        attackAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        if (telegraphEffectPrefab != null)
        {
            telegraphEffectInstance = Instantiate(
                telegraphEffectPrefab,
                transform.position,
                Quaternion.Euler(0, 0, attackAngle)
            );
        }

        yield return new WaitForSeconds(telegraphDuration);

        if (telegraphEffectInstance != null)
        {
            Destroy(telegraphEffectInstance, telegraphDuration); // Let it linger
        }

        PerformAttack();
    }

    public void PerformAttack()
    {
        if (IsDead || player == null) return;
        StartCoroutine(DelayedAttackDamage(telegraphEffectInstance));
    }

private IEnumerator DelayedAttackDamage(GameObject attackEffect)
{
    if (attackEffect == null)
    {
        Debug.LogWarning("AttackEffect is null.");
        yield break;
    }

    Collider2D[] childColliders = attackEffect.GetComponentsInChildren<Collider2D>();

    foreach (Collider2D col in childColliders)
    {
        if (col == null) continue;

        bool isInside = col.OverlapPoint(player.position);

        Debug.Log($"Collider: {col.name}, OverlapPoint: {isInside}, Player Pos: {player.position}");

        if (isInside)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                Debug.Log("✅ Player was overlapping collider! Applying damage.");
                playerScript.TakeDamage(attackDamage);
                break;
            }
        }
    }
    Destroy(attackEffect);

}


    public override void Die()
    {
        base.Die();
        Destroy(telegraphEffectInstance);
        Destroy(gameObject);
        // Do NOT destroy telegraphEffectInstance here — let it finish lingering
    }
}
