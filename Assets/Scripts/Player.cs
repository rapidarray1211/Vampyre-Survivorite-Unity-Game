using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public int Level { get; private set; } = 0;
    public float XP { get; private set; } = 0;
    public float xpToNextLevel = 10;
    public float maxHealth = 100f;
    public List<string> activeUpgrades = new List<string>();

    private UpgradeSystem upgradeSystem;
    private PlayerMovement movement;
    private PlayerShooting shooting;
    public Image healthBar;
    public float healthAmount = 100f;
    private float lastDamageTime = -Mathf.Infinity;
    public float damageCooldown = 0.2f;
    private bool isAscend = false;
    public float regenAmount = 2f;
    public float regenInterval = 5f;

    private float regenTimer = 0f;
    public ScoreManager ScoreManager;

    public void TakeDamage(float damage)
    {
        if (Time.time < lastDamageTime + damageCooldown) return;

        if (!movement.isDashing)
        {
            healthAmount -= damage;
            healthBar.fillAmount = healthAmount / maxHealth;
            lastDamageTime = Time.time;
        }
        if (healthAmount <= 0) {
            Die();
        }
    }


    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / maxHealth;
    }    
    
    private void Start()
    {
        upgradeSystem = FindObjectOfType<UpgradeSystem>();
        movement = GetComponent<PlayerMovement>();
        shooting = GetComponent<PlayerShooting>();
        healthAmount = maxHealth;
    }

    private void Update()
    {
        movement.HandleMovement();
        shooting.HandleShooting();

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            healthAmount += regenAmount;
            if (healthAmount > maxHealth) healthAmount = maxHealth;
            if (healthAmount > 0) {
                healthBar.fillAmount = healthAmount / maxHealth;
            }
            else {
                healthBar.fillAmount = 0;
            }
        }
    }

    public void GainXP(int amount)
    {
        ScoreManager.AddXP(amount);
        XP += amount;
        if (XP >= xpToNextLevel && Level < 9)
        {
            LevelUp();
        }
        else if (XP >= xpToNextLevel && !isAscend) {
            Ascend();
        }
    }

    private void LevelUp()
    {
        Level++;
        XP -= xpToNextLevel;
        xpToNextLevel = xpToNextLevel * 1.3f;
        upgradeSystem.LevelUp();
    }

    private void Ascend()
    {
        isAscend = true;
        Level = 10;
        upgradeSystem.Ascend();
    }
    public void ApplyUpgrade(int index)
    {
        switch (index)
        {
            case 0: // Increase Damage
                shooting.bulletDamage = shooting.bulletDamage * 1.3f;
                break;
            case 1: // Increase Movement Speed
                movement.moveSpeed = movement.moveSpeed * 1.5f;
                break;
            case 2: // Piercing Bullets
                shooting.EnableUpgrade("PiercingBullets");
                break;
            case 3: // Explosive Bullets
                shooting.EnableUpgrade("ExplosiveBullets");
                break;
            case 4: // Rapid Fire (Reduce Fire Delay)
                shooting.fireRate = shooting.fireRate * 0.7f;
                break;
            case 5: // Health Boost
                healthAmount = healthAmount * 1.6f;
                maxHealth = maxHealth * 1.6f;
                break;
            case 6: // Chaining Bullets
                shooting.EnableUpgrade("ChainingBullets");
                break;
            case 7: // +1 Projectile
                shooting.projectiles = shooting.projectiles + 1;
                break;
            case 8: // +3 Projectiles, -66% Damage
                shooting.projectiles = shooting.projectiles + 3;
                shooting.bulletDamage = shooting.bulletDamage * 0.5f;
                break;
            case 9: // Slow and Steady (2x Damage, -30% Attack Speed)
                shooting.bulletDamage = shooting.bulletDamage * 2f;
                shooting.fireRate = shooting.fireRate * 1.3f;
                break;
            case 10: // Minigun (5x Attack Speed, -60% Damage)
                shooting.fireRate = shooting.fireRate * 0.2f;
                shooting.bulletDamage = shooting.bulletDamage * 0.4f;
                break;
            case 11: // Herald of Lightning
                shooting.EnableUpgrade("HeraldLightning");
                break;
            case 12: // Herald of Ice
                shooting.EnableUpgrade("HeraldIce");
                break;
            case 13: // Herald of Ash
                shooting.EnableUpgrade("HeraldAsh");
                break;
            case 14: // Massive Bullets (Bullets 3x size, 10% increased Damage)
                shooting.EnableUpgrade("MassiveBullets");
                shooting.bulletDamage = shooting.bulletDamage * 1.1f;
                break;
            case 15: // Miniaturize (15% Attack Speed, Movement Speed, -30% Size)
                movement.moveSpeed = movement.moveSpeed * 1.15f;
                shooting.fireRate = shooting.fireRate * 1.15f;
                transform.localScale = transform.localScale * 0.7f; // Shrinks player size
                break;
            case 16: // Lucky Shot (20% chance to deal double damage)
                shooting.EnableUpgrade("LuckyShot");
                break;
            case 17: // Dash Ability
                movement.EnableUpgrade("Dash");
                break;
            case 18: // Heavy Hitter (50% increased Damage, -30% Movement Speed)
                shooting.bulletDamage = shooting.bulletDamage * 1.5f;
                movement.moveSpeed = movement.moveSpeed * 0.7f;
                break;
            case 19: // Glass Cannon (2x Damage, -60% Maximum Health)
                shooting.bulletDamage = shooting.bulletDamage * 2f;
                healthAmount = healthAmount * 0.4f;
                maxHealth = maxHealth * 0.4f;
                break;
            case 20: // Jack of All Trades (10% boost to everything)
                shooting.bulletDamage = shooting.bulletDamage * 1.1f;
                shooting.fireRate = shooting.fireRate * 1.1f;
                movement.moveSpeed = movement.moveSpeed * 1.1f;
                healthAmount = healthAmount * 1.1f;
                break;
            case 21:
                regenAmount *= 5f;
                break;
            case 22:
                shooting.EnableUpgrade("AreaofEffect");
                break;
        }
    }

    public void ApplyAscension(int index) {
        switch(index)
        {
            case 0: //Railgun
                shooting.bulletDamage *= 10f;
                shooting.fireRate *= 3.5f;
                shooting.bulletSpeed *= 5f;
                shooting.EnableAscension("Railgun");
                break;
            case 1: //Grenade Launcher
                shooting.bulletDamage *= 5f;
                shooting.fireRate *= 2f;
                shooting.bulletSpeed *= 0.75f;
                shooting.EnableAscension("Grenade Launcher");
                break;
            case 2: //Crystal machinegun
                shooting.fireRate *= 0.3f;
                shooting.bulletDamage *= 0.7f;
                shooting.bulletSpeed *= 2f;
                shooting.EnableAscension("Crystal Machinegun");
                break;
        }
    }


    public bool HasUpgrade(string upgradeName)
    {
        return activeUpgrades.Contains(upgradeName);
    }

    public void Die() {
        Time.timeScale = 0f; 
    }

}
