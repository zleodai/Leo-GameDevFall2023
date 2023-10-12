using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance = null;

    private InputActions inputActions;
    private Rigidbody rb;
    private SwingMovement swingMovement;
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
        air,
        swinging
    }

    [Header("Debugging")]
    private Vector3 startPos;

    [Header("Bools")]

    public bool freeze;
    private bool sprintPressed;
    public bool swinging;
    public bool inAir;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float swingSpeed;
    public float airSpeed;

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

        inputActions.Player.Movement.started += OdmMovementEnter;
        inputActions.Player.Movement.canceled += OdmMovementExit;
        inputActions.Player.Jump.started += OdmShortenEnter;
        inputActions.Player.Jump.canceled += OdmShortenExit;

        readyToJump = true;

        swingMovement = GameObject.FindFirstObjectByType<SwingMovement>();
        startPos = transform.position;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if (isGrounded)
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
            inAir = false;
        } else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            inAir = false;
        } else if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
            inAir = false;
        } else if (inAir)
        {
            state = MovementState.air;
            moveSpeed = airSpeed;
        }

        else
        {
            state = MovementState.air;

        }
    }

    private void MovePlayer()
    {
        if (swinging) return;
        if (inAir) return;

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
        if (swinging) return;
        if (inAir) return;

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

    private Vector3 velocityToSet;
    private void SetVelocity()
    {

        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        swinging = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            SwingMovement.instance.StopSwing();
        }
    }

    private void LeftGrappleEngage(InputAction.CallbackContext context)
    {
        swingMovement.StartSwing();
    }

    private void RightGrappleEngage(InputAction.CallbackContext context)
    {
        transform.position = startPos;
    }

    private void LeftGrappleDisengage(InputAction.CallbackContext context)
    {
        swingMovement.StopSwing();
    }

    private void RightGrappleDisengage(InputAction.CallbackContext context)
    {

    }

    private void OdmMovementEnter(InputAction.CallbackContext context)
    {
        if (swinging)
        {
            if (horizontalInput > 0 && verticalInput == 0)
            {
                if (horizontalInput > 0)
                {
                    swingMovement.odmRight = true;
                }
                else
                {
                    swingMovement.odmLeft = true;
                }
            }
            else if (verticalInput > 0 && horizontalInput == 0)
            {
                if (verticalInput > 0)
                {
                    swingMovement.odmForward = true;
                }
                else
                {
                    swingMovement.odmBackward = true;
                }
            }
        }
    }

    private void OdmMovementExit(InputAction.CallbackContext context)
    {
        if (swinging)
        {
            if (horizontalInput > 0 && verticalInput == 0)
            {
                if (horizontalInput > 0)
                {
                    swingMovement.odmRight = false;
                }
                else
                {
                    swingMovement.odmLeft = false;
                }
            }
            else if (verticalInput > 0 && horizontalInput == 0)
            {
                if (verticalInput > 0)
                {
                    swingMovement.odmForward = false;
                }
                else
                {
                    swingMovement.odmBackward = false;
                }
            }
        }
    }

    private void OdmShortenEnter(InputAction.CallbackContext context)
    {
        if (swinging)
        {
            swingMovement.odmShorten = true;
        }
    }

    private void OdmShortenExit(InputAction.CallbackContext context)
    {
        if (swinging)
        {
            swingMovement.odmShorten = false;
        }
    }
}
