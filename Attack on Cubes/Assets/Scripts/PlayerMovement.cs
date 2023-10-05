using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputActions inputActions;
    private Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    public Transform orientation;

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump; 


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool isGrounded;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Player.Movement.performed += Movement;
        inputActions.Player.Movement.canceled += MovementEnd;
        inputActions.Player.Jump.performed += Jump;

        readyToJump = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Movement(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;
    }

    private void MovementEnd(InputAction.CallbackContext context)
    {
        horizontalInput = 0;
        verticalInput = 0;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (readyToJump && isGrounded)
        {
            readyToJump = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
