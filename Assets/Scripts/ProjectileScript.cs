using System.Collections;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifetime = 5f;  // Time after which the projectile is destroyed if no collision happens
    public float damage = 25f;   // Damage dealt by the projectile

    private GameObject shooter;  // Reference to the player (shooter) who fired the projectile
    
    

    private void Start()
    {
        // Store the shooter (player) who fired the projectile
    
        // Destroy the projectile after a set lifetime if it doesn't collide with anything
        Destroy(gameObject, lifetime);
    }
    // Initialize method to set properties dynamically

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hit is Damageable
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            // Apply damage to the Damageable object
            if(damageable.TakeDamage(damage, shooter.GetComponent<PlayerClass>().team)) //only if it's on another team
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetShooter(GameObject player)
    {
        shooter = player;
    }
}
