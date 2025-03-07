using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Stats")]
    public int baseHealth = 100;
    public int baseMana = 50;
    public int bonusMana;
    public int bonusHP = 0;
    public int bonusProjectiles = 0;
    public float critChance = 0.1f; // 10% chance

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f; // seconds between shots

    private int currentHealth;
    private int currentMana;
    private float nextFireTime = 0f;

    void Start()
    {
        currentHealth = baseHealth + bonusHP;
        currentMana = baseMana + bonusMana;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        float moveX = 0f, moveZ = 0f;
        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void HandleShooting()
    {
       
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            if (Input.GetKey(KeyCode.X))
                AutoAimShoot();
            else
                ManualAimShoot();

            nextFireTime = Time.time + fireRate;
        }
    }

    void ManualAimShoot()
    {
      
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, firePoint.position.y, 0));
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            Vector3 aimDirection = (hitPoint - firePoint.position).normalized;
            ShootAtDirection(aimDirection);
        }
        else
        {
            Debug.LogWarning("Raycast did not hit the ground plane.");
        }
    }

    void AutoAimShoot()
    {
  .
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = enemies[0];
            float minDistance = Vector3.Distance(firePoint.position, nearestEnemy.transform.position);
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(firePoint.position, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestEnemy = enemy;
                }
            }
            Vector3 aimDirection = (nearestEnemy.transform.position - firePoint.position).normalized;
            ShootAtDirection(aimDirection);
        }
        else
        {
            Debug.LogWarning("No enemies found, falling back to manual aim.");
            ManualAimShoot();
        }
    }

    void ShootAtDirection(Vector3 aimDirection)
    {
        int numProjectiles = 1 + bonusProjectiles;
        float spreadAngle = 15f; // degrees between each projectile
        float startAngle = -spreadAngle * (numProjectiles - 1) / 2f;
        // Determine the base angle (in degrees) on the XZ plane.
        float baseAngle = Mathf.Atan2(aimDirection.z, aimDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < numProjectiles; i++)
        {
            float currentAngle = baseAngle + startAngle + i * spreadAngle;
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 projectileDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile3D projectileScript = proj.GetComponent<Projectile3D>();
            if (projectileScript != null)
            {
                projectileScript.direction = projectileDir;
                projectileScript.speed = projectileSpeed;
                projectileScript.damage = 10; // base damage value
                projectileScript.critChance = critChance;
                projectileScript.source = ProjectileSource.Player;
            }
            else
            {
                Debug.LogError("Projectile prefab is missing the Projectile3D component.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Player died");
            Destroy(gameObject);
        }
    }
}
