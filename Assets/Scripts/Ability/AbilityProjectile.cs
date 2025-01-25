using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private float timeAlive = 0f;
    public float damage;
    private Vector3 moveDirection;

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        timeAlive += Time.deltaTime;

        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetupProjectile(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Fireball:
                SetupFireball();
                break;
            case AbilityType.Heal:
                // set up future ablities
                break;
            default:
                Debug.LogWarning("Unknown ability type");
                break;
        }
    }

    private void SetupFireball()
    {
        damage = 10f;  // Fireball specific damage
        speed = 15f;   // Fireball speed
        lifetime = 5f; // How long it lasts
    }


    private void OnTriggerEnter(Collider other)
    {
        Damageable damageable = other.GetComponent<Damageable>();
    
        if (damageable != null)
        {
            if (other.name == "player1") // temp, once teams are set up can swap
            {
                return; // do nothin if player is hit
            }
            damageable.TakeDamage(damage);
            Debug.Log($"Hit {other.name} with damage {damage}");

            Destroy(gameObject); // Destroy fireball on impact
        }
    }

}