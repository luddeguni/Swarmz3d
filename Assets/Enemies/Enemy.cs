using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy3D : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int hp = 50;
    public float moveSpeed = 3f;          // Sets the NavMeshAgent’s speed.
    public float attackRange = 1f;        // When within this range, enemy will shoot.
    public int projectileCount = 1;
    public float critChance = 0f;         // Chance for enemy projectile crits.

    [Header("Shooting Settings")]
    public float fireRate = 1.0f;         // Time between shots.
    public GameObject projectilePrefab;
    public Transform firePoint;

    private GameObject player;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Enemy could not find a GameObject with tag 'Player'.");
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on this enemy.");
        }
        else
        {
            agent.speed = moveSpeed;
            // Setting stoppingDistance lets the enemy know when to stop moving
            // (i.e. when the player is within attack range).
            agent.stoppingDistance = attackRange;
        }

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        // Continuously update the destination so the enemy chases the player.
        if (player != null && agent != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                // Debug log to help you see the distance in the console.
                Debug.Log("Distance to player: " + distance);
                // If the enemy is within attackRange, shoot.
                if (distance <= attackRange)
                {
                    Shoot();
                }
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    void Shoot()
    {
        int numProjectiles = projectileCount;
        float spreadAngle = 15f;
        float startAngle = -spreadAngle * (numProjectiles - 1) / 2f;

        // Calculate the base angle toward the player on the XZ plane.
        Vector3 directionToPlayer = player.transform.position - firePoint.position;
        directionToPlayer.y = 0;
        float baseAngle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x) * Mathf.Rad2Deg;

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
                projectileScript.speed = 8f;    // Adjust enemy projectile speed as needed.
                projectileScript.damage = 5;      // Base damage for enemy projectiles.
                projectileScript.critChance = critChance;
                projectileScript.source = ProjectileSource.Enemy;
            }
            else
            {
                Debug.LogError("Enemy projectile prefab is missing the Projectile3D component.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Enemy took " + damage + " damage. Remaining HP: " + hp);
        if (hp <= 0)
        {
            Debug.Log("Enemy died");
            Destroy(gameObject);
        }
    }
}
