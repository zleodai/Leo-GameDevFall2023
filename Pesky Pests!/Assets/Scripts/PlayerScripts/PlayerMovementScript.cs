using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Refrences")]
    private Transform playerTransform;
    private Transform orientation;
    private CapsuleCollider playerCollider;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private GameManager gameManager;

    
    [Header("Movement")]
    private Vector2 wasdVector;
    private Vector3 moveDirection;
    private Vector2 EMPTY_VECTOR = new Vector2(0,0);
    public bool grounded;
    private LayerMask groundLayer;
    private float playerHeight;
    private float playerJumpOffset;
    public float jumpHeight;
    public float moveSpeed;
    public float airMultiplier;
    public float groundDrag;
    public float runMult;

    private bool shiftPressed;
    
    private enum MovementState
    {
        Running,
        Walking,
        Paused,
        Frozen
    }

    private MovementState movementState;

    private void Awake()
    {
        playerTransform = gameObject.transform.Find("PlayerMesh").transform;
        playerCollider = gameObject.transform.Find("PlayerMesh").GetComponent<CapsuleCollider>();
        playerRigidbody = gameObject.transform.Find("PlayerMesh").GetComponent<Rigidbody>();
        orientation = gameObject.transform.Find("Orientation");
        gameManager = GameObject.FindFirstObjectByType<GameManager>();

        groundLayer = LayerMask.GetMask("GroundLayer");

        //Initalize values
        playerHeight = 2f;
        playerJumpOffset = 0.3f;
        jumpHeight = 175f;
        moveSpeed = 1.75f;
        airMultiplier = 1f;
        groundDrag = 0.25f;
        runMult = 2.5f;

        //Player Input
        playerInput = new PlayerInput();
        playerInput.Enable();

        playerInput.PlayerMovement.WASD.performed += WASDperformed;
        playerInput.PlayerMovement.WASD.canceled += WASDcanceled;
        playerInput.PlayerMovement.Space.performed += Spaceperformed;
        playerInput.PlayerMovement.Shift.started += Shiftstarted;
        playerInput.PlayerMovement.Shift.canceled += Shiftcanceled;


        //UI Player Input
        playerInput.UI.Esc.performed += escPressed;
    }

    private void Update()
    {
        grounded = Physics.Raycast(playerTransform.position + new Vector3(0f, playerHeight * 0.5f, 0f), Vector3.down, playerHeight + playerJumpOffset, groundLayer);

        MovePlayer();
        SpeedControl();

        playerTransform.rotation = orientation.rotation;

        if (gameManager.gameState == GameManager.GameState.GAMEPLAY)
        {
            if (shiftPressed) movementState = MovementState.Running;
            else movementState = MovementState.Walking;

            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else if (gameManager.gameState == GameManager.GameState.MENU)
        {
            movementState = MovementState.Paused;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    private void WASDperformed(InputAction.CallbackContext context)
    {
        wasdVector = context.ReadValue<Vector2>();
    }

    private void WASDcanceled(InputAction.CallbackContext context)
    {
        wasdVector = EMPTY_VECTOR;
    }

    private void Spaceperformed(InputAction.CallbackContext context)
    {
        if (gameManager.gameState == GameManager.GameState.GAMEPLAY && grounded) playerRigidbody.AddForce(transform.up * jumpHeight);
    }

    private void Shiftstarted(InputAction.CallbackContext context)
    {
        shiftPressed = true;
    }

    private void Shiftcanceled(InputAction.CallbackContext context)
    {
        shiftPressed = false;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            playerRigidbody.velocity = new Vector3(limitedVel.x, playerRigidbody.velocity.y, limitedVel.z);
        }

        if (wasdVector.Equals(EMPTY_VECTOR) && grounded)
        {
            Vector3 velocity = playerRigidbody.velocity;
            playerRigidbody.velocity = new Vector3(velocity.x * groundDrag, playerRigidbody.velocity.y, velocity.z * groundDrag);
            //Add Script to bind player to ground
        }   
    }

    private void MovePlayer()
    {
        if (movementState == MovementState.Walking)
        {
            moveDirection = orientation.forward * wasdVector.y + orientation.right * wasdVector.x;

            if (grounded) playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            else if (!grounded) playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        } else if (movementState == MovementState.Running)
        {
            moveDirection = orientation.forward * wasdVector.y + orientation.right * wasdVector.x;

            if (grounded) playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * runMult * 10f, ForceMode.Force);

            else if (!grounded) playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * runMult * 10f * airMultiplier, ForceMode.Force);
        } else if (movementState == MovementState.Paused)
        {

        }
    }


    //UI Interactions
    private void escPressed(InputAction.CallbackContext context)
    {
        if (gameManager.gameState == GameManager.GameState.GAMEPLAY)
        {
            gameManager.setGameState(GameManager.GameState.MENU);
        }
        else if (gameManager.gameState == GameManager.GameState.MENU)
        {
            gameManager.setGameState(GameManager.GameState.GAMEPLAY);
        }
    }
}
