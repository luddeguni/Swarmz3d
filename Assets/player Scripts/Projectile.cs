using UnityEngine;

public enum ProjectileSource
{
    Player,
    Enemy
}

public class Projectile3D : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float critChance = 0f; // chance to deal critical damage
    public float critMultiplier = 1.5f; // multiplier for critical damage
    public Vector3 direction = Vector3.forward;
    public ProjectileSource source;

    void Update()
    {
        // Move the projectile along its direction.
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // If this is a player's projectile hitting an enemy.
        if (source == ProjectileSource.Player && other.CompareTag("Enemy"))
        {
            Enemy3D enemy = other.GetComponent<Enemy3D>();
            if (enemy != null)
            {
                int finalDamage = damage;
                if (Random.value < critChance)
                    finalDamage = Mathf.RoundToInt(damage * critMultiplier);
                enemy.TakeDamage(finalDamage);
            }
            Destroy(gameObject);
        }
        // If this is an enemy projectile hitting the player.
        else if (source == ProjectileSource.Enemy && other.CompareTag("Player"))
        {
            PlayerController3D player = other.GetComponent<PlayerController3D>();
            if (player != null)
            {
                int finalDamage = damage;
                if (Random.value < critChance)
                    finalDamage = Mathf.RoundToInt(damage * critMultiplier);
                player.TakeDamage(finalDamage);
            }
            Destroy(gameObject);
        }
        // Optionally, destroy projectile on hitting an obstacle.
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
