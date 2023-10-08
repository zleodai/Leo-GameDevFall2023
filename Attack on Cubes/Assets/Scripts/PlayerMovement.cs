using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance = null;

    private InputActions inputActions;
    private Rigidbody rb;
    private GrapplingMovment grappleMovement;

    private ThirdPersonCam cameraObject;

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

    [Header("Grapple")]
    public GameObject grapplePrefab;
    private bool engagedLeftGrapple;
    private bool engagedRightGrapple;
    private GameObject leftGrapple;
    private GameObject rightGrapple;

    //to check if grapple has hit an object
    private bool leftGrappleStuck;
    private bool rightGrappleStuck;

    //Public values for debug
    public Vector3 grappleDirection;
    public GameObject grappleSpawnPoint;
    public float grappleSpeed;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Player.Movement.performed += Movement;
        inputActions.Player.Movement.canceled += MovementEnd;
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.LeftGrapple.started += LeftGrappleEngage;
        inputActions.Player.RightGrapple.started += RightGrappleEngage;
        inputActions.Player.LeftGrapple.canceled += LeftGrappleDisengage;
        inputActions.Player.RightGrapple.canceled += RightGrappleDisengage;

        readyToJump = true;

        cameraObject = GameObject.FindFirstObjectByType<ThirdPersonCam>();
        grappleMovement = GameObject.FindFirstObjectByType<GrapplingMovment>();

        grappleSpeed = 50f;
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

        if (leftGrapple != null)
        {
            Rigidbody grappleRB = leftGrapple.GetComponent<Rigidbody>();
        }
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

    private void LeftGrappleEngage(InputAction.CallbackContext context)
    {
        engagedLeftGrapple = true;
        GrapplingMovment.instance.StartGrapple();
    }

    private void RightGrappleEngage(InputAction.CallbackContext context)
    {

    }

    private void LeftGrappleDisengage(InputAction.CallbackContext context)
    {
        engagedLeftGrapple = false;
        GrapplingMovment.instance.StopGrapple();
    }

    private void RightGrappleDisengage(InputAction.CallbackContext context)
    {

    }
}
