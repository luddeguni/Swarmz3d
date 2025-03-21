using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    [Header("Level")]
    public float _level = 1;
    public float _experience = 0f;
    public float _experienceToLevel = 1000;


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
    private bool autoAimEnabled = false;
    private bool autoShooting = false;

    void Start()
    {
        currentHealth = baseHealth + bonusHP;
        currentMana = baseMana + bonusMana;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        ToggleAutoAim();
        ToggleAutoShooting();
    }

    void HandleMovement()
    {
        float moveZ = 0f;
        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;

        Vector3 moveDir = transform.forward * moveZ;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float rotation = 0f;
        if (Input.GetKey(KeyCode.A)) rotation -= 1f;
        if (Input.GetKey(KeyCode.D)) rotation += 1f;
        transform.Rotate(Vector3.up, rotation * rotationSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if ((Input.GetMouseButtonDown(0) || autoShooting) && Time.time >= nextFireTime)
        {
            if (autoAimEnabled)
                AutoAimShoot();
            else
                ManualAimShoot();

            nextFireTime = Time.time + fireRate;
        }
    }

    void ToggleAutoAim()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            autoAimEnabled = !autoAimEnabled;
            Debug.Log("Auto Aim: " + (autoAimEnabled ? "Enabled" : "Disabled"));
        }
    }

    void ToggleAutoShooting()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            autoShooting = !autoShooting;
            Debug.Log("Auto Shooting: " + (autoShooting ? "Enabled" : "Disabled"));
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
        float spreadAngle = 15f;
        float startAngle = -spreadAngle * (numProjectiles - 1) / 2f;
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
                projectileScript.damage = 10;
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
