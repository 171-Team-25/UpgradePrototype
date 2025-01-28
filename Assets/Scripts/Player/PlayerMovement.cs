using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f, sprintSpeed = 2f;

    private CharacterController characterController;
    private Vector2 inputDirection;
    private bool isSprinting = false;  // Track if Sprint is active
    private Animator animator;
    private Transform modelTransform;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void SetModel(GameObject model)
    {
        animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            modelTransform = animator.transform;
            Debug.Log("Animator found and modelTransform assigned.");
        }
        else
        {
            Debug.LogError("Animator component not found on the model.");
        }
    }

    // This method matches the "move" action in the Input Actions asset
    public void OnMove(InputAction.CallbackContext context)
    {
        // Read the input value (Vector2)
        inputDirection = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValue<float>() >= 0.1f;
    }

    private void Update()
    {
        // Convert input direction to world space
        Vector3 move = new Vector3(inputDirection.x, 0, inputDirection.y);
        //move = transform.TransformDirection(move);    MAKE IT SO IT DOESNT MATTER WHAT THE ROTATION IS

        // Apply movement
        if(isSprinting)
        {
            characterController.Move(move * moveSpeed * Time.deltaTime * sprintSpeed);
        }
        else
        {
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }

        // Rotate the model to face the movement direction
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Update animator parameters
        if (animator != null)
        {
            bool isMoving = move != Vector3.zero;
            animator.SetBool("Running", isSprinting && isMoving);
            animator.SetBool("Walking", !isSprinting && isMoving);
        }
    }
}
