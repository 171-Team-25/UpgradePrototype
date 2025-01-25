using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;   // The projectile (ball) prefab to shoot
    public float attackForce = 10f;       // The force with which the projectile is shot
    public float spawnOffset = 1f;        // Offset distance to spawn the projectile outside the character
    public float rotationSpeed = 10f;     // The speed at which the player rotates toward the shooting direction

    private Camera playerCam;
    private bool canShoot = true;         // Variable to prevent multiple shots per click

    private void Start()
    {
        // Get the main camera reference
        playerCam = GetComponentInChildren<Camera>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        // Check if the action was a "started" action (meaning a single press)
        if (context.started && canShoot)
        {
            // Get mouse position in screen coordinates
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

            // Convert screen position to world position (3D)
            Ray ray = playerCam.ScreenPointToRay(mouseScreenPos);
            //Debug.DrawRay(ray.origin, Vector3.up);
            RaycastHit hit;

            // We use a raycast to determine where the mouse intersects the world plane
            if (Physics.Raycast(ray, out hit))
            {
                // The hit point is the target position in the world
                Vector3 targetPosition = hit.point;

                // Calculate the direction from the player to the target
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction.y = 0.0f;  // Ensure the player only rotates on the y-axis

                // Calculate the spawn position (offset from player)
                Vector3 spawnPosition = transform.position + direction * spawnOffset;

                // Instantiate the projectile at the new spawn position
                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                projectile.GetComponent<ProjectileScript>().SetShooter(gameObject);

                // Get the Rigidbody component and apply force to shoot the projectile
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direction * attackForce, ForceMode.Impulse);
                }

                // Rotate the player toward the target direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                //transform.rotation = targetRotation;

                // Set canShoot to false to prevent firing again until the click is released
                canShoot = false;
            }
        }
        else if (context.canceled)
        {
            // Allow shooting again when the button is released
            canShoot = true;
        }
    }
}
