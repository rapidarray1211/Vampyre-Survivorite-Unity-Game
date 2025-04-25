using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float speed = 40f;
    public float lifetime = 3f;
    public float damage = 10f;

    public bool piercing = false;
    public bool explosive = false;
    public bool chaining = false;
    public bool heraldLightning = false;
    public bool heraldIce = false;
    public bool heraldAsh = false; 

    public bool railgun = false;
    public bool grenade = false;
    public bool crystal = false;
    private float scale = 1f;
    private float aoeScale  = 1f;

    private Rigidbody2D rb;

    public GameObject explosionPrefab;
    public GameObject lightningPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float bulletSpeed, float bulletDamage,
                           bool isPiercing, bool isExplosive, bool isChaining,
                           bool isHeraldLightning, bool isHeraldIce, bool isHeraldAsh,
                           float scaleMultiplier = 1f, float areaScale = 1f,
                           bool isRailgun = false, bool isGrenade = false, bool isCrystal = false)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        piercing = isPiercing;
        explosive = isExplosive;
        chaining = isChaining;
        heraldLightning = isHeraldLightning;
        heraldIce = isHeraldIce;
        heraldAsh = isHeraldAsh;
        railgun = isRailgun;
        grenade = isGrenade;
        crystal = isCrystal;
        scale = scaleMultiplier;
        aoeScale = areaScale;

        rb.linearVelocity = direction.normalized * speed;
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1f);
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                ApplyEffects(enemy);
            }

            if (grenade)
            {
                Explode();
                Destroy(gameObject);
                return;
            }

            if (crystal)
            {
                Fragment();
                Destroy(gameObject);
            }

            if (!piercing && !railgun)
            {
                Destroy(gameObject);
            }
            else if (!railgun)
            {
                piercing = false;
            }
        }
    }

    private void ApplyEffects(Enemy enemy)
    {
        if (explosive && !grenade) Explode();
        if (chaining) ChainToAnotherEnemy(enemy);
        if (heraldLightning) StrikeLightning(enemy);
        if (heraldIce) SlowEnemy(enemy);
        if (heraldAsh) BurnEnemy(enemy);
    }

    private void Explode()
    {
        float radius = 2.5f;

        if (grenade && explosive)
        {
            radius *= 3f;
        }
        else if (grenade)
        {
            radius *= 2.5f;
        }
        radius *= aoeScale;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(radius, radius, 1f);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage / 2);
            }
        }
    }


    private void ChainToAnotherEnemy(Enemy hitEnemy)
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(hitEnemy.transform.position, 12f);
        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.gameObject != hitEnemy.gameObject && enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 directionToEnemy = (enemy.transform.position - hitEnemy.transform.position).normalized;

                    float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
                    Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                    GameObject newBullet = Instantiate(gameObject, hitEnemy.transform.position, rotation);
                    Bullet newBulletScript = newBullet.GetComponent<Bullet>();

                    if (newBulletScript != null)
                    {
                        newBulletScript.Initialize(
                            directionToEnemy, speed / 2, damage / 3,
                            true, explosive, crystal,
                            heraldLightning, heraldIce, heraldAsh,
                            scale, aoeScale, 
                            railgun, grenade, false
                        );
                    }

                    break;
                }
            }
        }
    }


    private void Fragment()
    {
        float spreadAngle = 120f;
        float randomBaseAngle = Random.Range(0f, 360f);

        for (int i = -1; i <= 1; i++)
        {
            float angleOffset = i * spreadAngle;
            float finalAngle = randomBaseAngle + angleOffset;

            Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);
            Vector2 direction = rotation * Vector2.right;

            Vector3 spawnOffset = (Vector3)direction * 0.5f;
            GameObject frag = Instantiate(gameObject, transform.position + spawnOffset, Quaternion.identity);
            Bullet fragScript = frag.GetComponent<Bullet>();

            if (fragScript != null)
            {
                fragScript.Initialize(
                    direction,
                    speed * 0.8f,
                    damage * 0.6f,
                    true, explosive, false,
                    heraldLightning, heraldIce, heraldAsh,
                    scale, aoeScale,
                    false, false, false
                );
            }
        }
    }

    private void StrikeLightning(Enemy enemy)
    {
        if (Random.value < 0.33f)
        {
            if (lightningPrefab != null)
            {
                Instantiate(lightningPrefab, enemy.transform.position, Quaternion.identity);
            }
            enemy.TakeDamage(damage * 1.5f);
        }
    }

    private void SlowEnemy(Enemy enemy)
    {
        enemy.Slow(0.5f, 2f);
    }

    private void BurnEnemy(Enemy enemy)
    {
        enemy.ApplyBurn(damage * 2.0f, 6f);
    }
}
