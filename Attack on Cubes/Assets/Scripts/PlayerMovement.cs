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

    public float doubleTapDelay;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;

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

        //inputActions.Player.OdmLeft.started += ctx => swingMovement.odmLeft = true;
        //inputActions.Player.OdmRight.started += ctx => swingMovement.odmRight = true;
        //inputActions.Player.OdmForward.started += ctx => swingMovement.odmForward = true;
        //inputActions.Player.OdmBackward.started += ctx => swingMovement.odmBackward = true;
        //inputActions.Player.OdmLeft.canceled += ctx => swingMovement.odmLeft = false;
        //inputActions.Player.OdmRight.canceled += ctx => swingMovement.odmRight = false;
        //inputActions.Player.OdmForward.canceled += ctx => swingMovement.odmForward = false;
        //inputActions.Player.OdmBackward.canceled += ctx => swingMovement.odmBackward = false;

        inputActions.Player.OdmRight.started += OdmRightStart;
        inputActions.Player.OdmRight.canceled += OdmRightCanceled;

        inputActions.Player.OdmLeft.started += OdmLeftStart;
        inputActions.Player.OdmLeft.canceled += OdmLeftCanceled;

        inputActions.Player.OdmForward.started += OdmForwardStart;
        inputActions.Player.OdmForward.canceled += OdmForwardCanceled;

        inputActions.Player.OdmBackward.started += OdmBackwardStart;
        inputActions.Player.OdmBackward.canceled += OdmBackwardCanceled;

        inputActions.Player.Jump.started += ctx => swingMovement.odmShorten = true;
        inputActions.Player.Jump.canceled += ctx => swingMovement.odmShorten = false;

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

        if (odmLeftActive) OdmLeftActive();
        if (odmRightActive) OdmRightActive();
        if (odmForwardActive) OdmForwardActive();
        if (odmBackwardActive) OdmBackwardActive();
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

            SwingMovement.instance.StopSwing(0);
            SwingMovement.instance.StopSwing(1);
        }
    }

    private void LeftGrappleEngage(InputAction.CallbackContext context)
    {
        swingMovement.StartSwing(0);
    }

    private void RightGrappleEngage(InputAction.CallbackContext context)
    {
        swingMovement.StartSwing(1);
    }

    private void LeftGrappleDisengage(InputAction.CallbackContext context)
    {
        swingMovement.StopSwing(0);
    }

    private void RightGrappleDisengage(InputAction.CallbackContext context)
    {
        swingMovement.StopSwing(1);
    }

    private float odmRightCounter;
    private bool odmRightActive;
    private bool rightTappedOnce;

    private void OdmRightStart(InputAction.CallbackContext context)
    {
        odmRightCounter = 0f;
        odmRightActive = true;
        swingMovement.odmRight = false;
    }

    private void OdmRightActive()
    {
        odmRightCounter += Time.deltaTime;
        if (odmRightCounter > doubleTapDelay)
        {
            rightTappedOnce = false;
            swingMovement.odmRight = true;
        }
    }

    private void OdmRightCanceled(InputAction.CallbackContext context)
    {
        if (odmRightCounter < doubleTapDelay)
        {
            if (rightTappedOnce)
            {
                swingMovement.OdmRightBurst();
                rightTappedOnce = false;
            }
            else
            {
                rightTappedOnce = true;
            }
        }
        odmRightActive = false;
        swingMovement.odmRight = false;
    }

    private float odmLeftCounter;
    private bool odmLeftActive;
    private bool leftTappedOnce;

    private void OdmLeftStart(InputAction.CallbackContext context)
    {
        odmLeftCounter = 0f;
        odmLeftActive = true;
        swingMovement.odmLeft = false;
    }

    private void OdmLeftActive()
    {
        odmLeftCounter += Time.deltaTime;
        if (odmLeftCounter > doubleTapDelay)
        {
            leftTappedOnce = false;
            swingMovement.odmLeft = true;
        }
    }

    private void OdmLeftCanceled(InputAction.CallbackContext context)
    {
        if (odmLeftCounter < doubleTapDelay)
        {
            if (leftTappedOnce)
            {
                swingMovement.OdmLeftBurst();
                leftTappedOnce = false;
            }   
            else
            {
                leftTappedOnce = true;
            }
        }
        odmLeftActive = false;
        swingMovement.odmLeft = false;
    }

    private float odmForwardCounter;
    private bool odmForwardActive;
    private bool forwardTappedOnce;

    private void OdmForwardStart(InputAction.CallbackContext context)
    {
        odmForwardCounter = 0f;
        odmForwardActive = true;
        swingMovement.odmForward = false;
    }

    private void OdmForwardActive()
    {
        odmForwardCounter += Time.deltaTime;
        if (odmForwardCounter > doubleTapDelay)
        {
            forwardTappedOnce = false;
            swingMovement.odmForward = true;
        }
    }

    private void OdmForwardCanceled(InputAction.CallbackContext context)
    {
        if (odmForwardCounter < doubleTapDelay)
        {
            if (forwardTappedOnce)
            {
                swingMovement.OdmForwardtBurst();
                forwardTappedOnce = false;
            }
            else
            {
                forwardTappedOnce = true;
            }
        }
        odmForwardActive = false;
        swingMovement.odmForward = false;
    }

    private float odmBackwardCounter;
    private bool odmBackwardActive;
    private bool backwardTappedOnce;

    private void OdmBackwardStart(InputAction.CallbackContext context)
    {
        odmBackwardCounter = 0f;
        odmBackwardActive = true;
        swingMovement.odmBackward = false;
    }

    private void OdmBackwardActive()
    {
        odmBackwardCounter += Time.deltaTime;
        if (odmBackwardCounter > doubleTapDelay)
        {
            backwardTappedOnce = false;
            swingMovement.odmBackward = true;
        }
    }

    private void OdmBackwardCanceled(InputAction.CallbackContext context)
    {
        if (odmBackwardCounter < doubleTapDelay)
        {
            if (backwardTappedOnce)
            {
                swingMovement.OdmBackwardBurst();
                backwardTappedOnce = false;
            }
            else
            {
                backwardTappedOnce = true;
            }
        }
        odmBackwardActive = false;
        swingMovement.odmBackward = false;
    }
}
