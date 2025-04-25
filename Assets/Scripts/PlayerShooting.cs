using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject RailgunPrefab;
    public GameObject GrenadePrefab;
    public GameObject CrystalPrefab;
    public Transform firePoint;
    public float bulletDamage = 12f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public int projectiles = 1;
    private float nextFireTime = 0f;

    private Player player;
    private HashSet<string> activeUpgrades = new HashSet<string>();

    public GameObject walkSide;
    public GameObject walkUp;
    public GameObject walkDown;
    public GameObject idle;
    public GameObject idleUp;
    public GameObject idleDown;
    private GameObject currentVisual;
    private PlayerMovement movement;

    private void Start()
    {
        player = GetComponent<Player>();
        movement = GetComponent<PlayerMovement>();
        walkSide.SetActive(false);
        walkUp.SetActive(false);
        walkDown.SetActive(false);
        idle.SetActive(true);
        idleUp.SetActive(false);
        idleDown.SetActive(false);
    }

    public void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 aimDirection = (mouseWorldPos - transform.position).normalized;

            GameObject nextVisual = walkSide;

            // Use idle variants if not moving
            bool isMoving = movement.movementInput.sqrMagnitude > 0.01f;

            if (Mathf.Abs(aimDirection.x) > Mathf.Abs(aimDirection.y))
            {
                // Side aiming
                nextVisual = isMoving ? walkSide : idle;

                bool flip = aimDirection.x > 0;
                foreach (var sr in nextVisual.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.flipX = flip;

                    Transform t = sr.transform;
                    Vector3 pos = t.localPosition;
                    pos.x = Mathf.Abs(pos.x) * (flip ? 1 : -1);
                    t.localPosition = pos;
                }
            }
            else if (aimDirection.y > 0)
            {
                // Aiming up
                nextVisual = isMoving ? walkUp : idleUp;
            }
            else if (aimDirection.y < 0)
            {
                // Aiming down
                nextVisual = isMoving ? walkDown : idleDown;
            }

            // SWITCH VISUALS
            if (currentVisual != nextVisual)
            {
                if (currentVisual != null) currentVisual.SetActive(false);
                nextVisual.SetActive(true);
                currentVisual = nextVisual;
            }

            // SHOOT!
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }


    private void Shoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 fireDirection = (mousePos - (Vector2)firePoint.position).normalized;

        if (projectiles > 1)
        {
            float spreadAngle = 10f;
            float startAngle = -((projectiles - 1) * spreadAngle) / 2;

            for (int i = 0; i < projectiles; i++)
            {
                float angleOffset = startAngle + (spreadAngle * i);
                Vector2 newDirection = Quaternion.Euler(0, 0, angleOffset) * fireDirection;

                CreateBullet(newDirection);
            }
        }
        else
        {
            CreateBullet(fireDirection);
        }
    }

    private void CreateBullet(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        GameObject bullet;
        if (activeUpgrades.Contains("Railgun")) {
            bullet = Instantiate(RailgunPrefab, firePoint.position, rotation);
        }
        else if (activeUpgrades.Contains("Grenade Launcher")) {
            bullet = Instantiate(GrenadePrefab, firePoint.position, rotation);
        }
        else if (activeUpgrades.Contains("Crystal Machinegun")) {
            bullet = Instantiate(CrystalPrefab, firePoint.position, rotation);
        }
        else {
            bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        }
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Initialize(
                direction, bulletSpeed, bulletDamage, 
                activeUpgrades.Contains("PiercingBullets"),
                activeUpgrades.Contains("ExplosiveBullets"),
                activeUpgrades.Contains("ChainingBullets"),
                activeUpgrades.Contains("HeraldLightning"),
                activeUpgrades.Contains("HeraldIce"),
                activeUpgrades.Contains("HeraldAsh"),
                activeUpgrades.Contains("MassiveBullets") ? 3f : 1f,
                activeUpgrades.Contains("AreaofEffect") ? 1.6f: 1f,
                activeUpgrades.Contains("Railgun"),
                activeUpgrades.Contains("Grenade Launcher"),
                activeUpgrades.Contains("Crystal Machinegun")
            );
        }

    }

    public void EnableUpgrade(string upgrade)
    {
        if (!activeUpgrades.Contains(upgrade))
        {
            activeUpgrades.Add(upgrade);
        }
    }

    public void EnableAscension(string upgrade)
    {
        if (!activeUpgrades.Contains(upgrade))
        {
            activeUpgrades.Add(upgrade);
        }
    }
}
