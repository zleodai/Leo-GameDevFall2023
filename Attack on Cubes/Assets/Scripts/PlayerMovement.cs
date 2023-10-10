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
    private GameObject playerObject;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    public Transform orientation;

    public MovementState state;

    public enum MovementState
    {
        freeze,
        walking,
        spriting,
        air
    }

    [Header("Bools")]

    public bool freeze;
    private bool sprintPressed;
    public bool activeGrapple;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump; 

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

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

        playerObject = gameObject.transform.GetChild(0).gameObject;

        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Player.Movement.performed += Movement;
        inputActions.Player.Movement.canceled += MovementEnd;
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Sprint.started += SprintStart;
        inputActions.Player.Sprint.canceled += SprintStop;
        inputActions.Player.LeftGrapple.started += LeftGrappleEngage;
        inputActions.Player.RightGrapple.started += RightGrappleEngage;
        inputActions.Player.LeftGrapple.canceled += LeftGrappleDisengage;
        inputActions.Player.RightGrapple.canceled += RightGrappleDisengage;

        readyToJump = true;

        grappleMovement = GameObject.FindFirstObjectByType<GrapplingMovment>();

        grappleSpeed = 50f;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if (isGrounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        SpeedControl();
        StateHandeler();

        playerObject.transform.rotation = orientation.transform.rotation;
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

    private void SprintStart(InputAction.CallbackContext context)
    {
        sprintPressed = true;
    }

    private void SprintStop(InputAction.CallbackContext context)
    {
        sprintPressed = false;
    }

    private void StateHandeler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        } else if(isGrounded && sprintPressed)
        {
            state = MovementState.spriting;
            moveSpeed = sprintSpeed;
        } else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;

        }
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope()&& !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        if (OnSlope()&& !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            } else
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        } else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (readyToJump && isGrounded)
        {
            readyToJump = false;
            exitingSlope = true;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private bool enableMovementOnNextTouch;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    } 

    private Vector3 velocityToSet;
    private void SetVelocity()
    {

        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            grappleMovement.StopGrapple();
        }
    }

    private void LeftGrappleEngage(InputAction.CallbackContext context)
    {
        engagedLeftGrapple = true;
        GrapplingMovment.instance.StartGrapple();
        Debug.Log("Engaged");
    }

    private void RightGrappleEngage(InputAction.CallbackContext context)
    {

    }

    private void LeftGrappleDisengage(InputAction.CallbackContext context)
    {
        engagedLeftGrapple = false;
        GrapplingMovment.instance.StopGrapple();
        Debug.Log("Disengaged");
    }

    private void RightGrappleDisengage(InputAction.CallbackContext context)
    {

    }
}
