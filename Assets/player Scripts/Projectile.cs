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
    public float critChance = 0f; 
    public float critMultiplier = 1.5f; 
    public Vector3 direction = Vector3.forward;
    public ProjectileSource source;

    void Update()
    {
       
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
   
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
       
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }

        Destroy(gameObject,5);
    }
}
