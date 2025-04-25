using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    public float aimingRange = 5f;

    protected override void Start()
    {
        base.Start();
        currentState = new StateChase();
    }

    public bool IsWithinAimingRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) <= aimingRange;
    }

    public override void AttackPlayer()
    {
        if (player == null) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            ShootProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
    }
}
