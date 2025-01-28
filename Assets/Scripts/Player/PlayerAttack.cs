using System;
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

    public event Action OnFire;

    private Animator animator;

    private void Start()
    {
        // Get the main camera reference
        playerCam = GetComponentInChildren<Camera>();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found!");
            return;
        }

        // Bind the OnFireInput method to the Fire action
        playerInput.actions["Fire"].performed += OnFireInput;
        playerInput.actions["Fire"].canceled += OnFireInput;
        playerInput.actions["Fire"].started += OnFireInput;
    }

    public void SetModel(GameObject model)
    {
        animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            Debug.Log("Animator found.");
        }
        else
        {
            Debug.LogError("Animator component not found on the model.");
        }
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        // Check if the action was a "started" action (meaning a single press)
        if (context.started && canShoot)
        {
            if (animator != null)
            {
                animator.SetTrigger("Throwing");
            }
            // Get mouse position in screen coordinates
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

            // Convert screen position to world position (3D)
            Ray ray = playerCam.ScreenPointToRay(mouseScreenPos);
            RaycastHit hit;

            // We use a raycast to determine where the mouse intersects the world plane
            if (Physics.Raycast(ray, out hit))
            {
                // The hit point is the target position in the world
                Vector3 targetPosition = hit.point;

                // Calculate the direction from the player to the target
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction.y = 0.0f;  // Ensure the player only rotates on the y-axis

                // Shoot the main projectile
                ShootProjectile(direction);

                // Invoke the OnFire event
                OnFire?.Invoke();

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

    public void ShootProjectile(Vector3 direction)
    {
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
    }

    public Vector3 GetShootDirection()
    {
        // Get mouse position in screen coordinates
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

        // Convert screen position to world position (3D)
        Ray ray = playerCam.ScreenPointToRay(mouseScreenPos);
        RaycastHit hit;

        // We use a raycast to determine where the mouse intersects the world plane
        if (Physics.Raycast(ray, out hit))
        {
            // The hit point is the target position in the world
            Vector3 targetPosition = hit.point;

            // Calculate the direction from the player to the target
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0.0f;  // Ensure the player only rotates on the y-axis

            return direction;
        }

        return Vector3.forward; // Default direction if no hit
    }
}