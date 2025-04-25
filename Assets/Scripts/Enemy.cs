using UnityEngine;
using UnityEngine.UI;
public abstract class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth = 50f;
    public float speed = 3f;
    public float attackRange = 1f;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public int xp = 5;
    
    protected Transform player;
    protected float lastAttackTime = 0f;
    protected State currentState;
    public Player playerObject;
    private float originalSpeed;
    private bool isBurning = false;

    public bool IsDead => health <= 0;
    public Image healthBar;


    protected virtual void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerObject = player.GetComponent<Player>();
        originalSpeed = speed;
    }

    protected virtual void Update()
    {
        if (currentState != null)
        {
            currentState.Execute(this);
        }
    }

    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    public virtual bool IsWithinAttackRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            ChangeState(new StateDead());
        }
        healthBar.fillAmount = health/maxHealth;
    }

    public virtual void AttackPlayer()
    {
        if (player == null) return;
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void Slow(float slowFactor, float duration)
    {
        speed *= slowFactor;
        Invoke(nameof(RestoreSpeed), duration);
    }

    private void RestoreSpeed()
    {
        speed = originalSpeed;
    }

    public void ApplyBurn(float burnDamage, float duration)
    {
        if (!isBurning)
        {
            isBurning = true;
            StartCoroutine(BurnEffect(burnDamage, duration));
        }
    }

    private System.Collections.IEnumerator BurnEffect(float burnDamage, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            TakeDamage(burnDamage * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isBurning = false;
    }

    public virtual void Die()
    {
        playerObject.GainXP(xp);
        Destroy(gameObject);
    }

    public virtual void InitializeStats(float difficultyMultiplier)
    {
        maxHealth *= difficultyMultiplier;
        health = maxHealth;
        speed *= (1f + (difficultyMultiplier - 1f) * 0.5f); // Increases speed slightly slower than health
    }
}
