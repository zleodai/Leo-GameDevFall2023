using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerScript : MonoBehaviour
{
    [Header("Refrences")]
    private Transform playerTransform;
    private Transform orientation;
    private CapsuleCollider playerCollider;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private GameManager gameManager;
    private InventoryManager inventoryManager;
    
    [Header("Movement")]
    public Vector2 wasdVector;
    public Vector3 moveDirection;
    public Vector2 rigidBodyVelocity;
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

    [Header("Inventory")]
    public GameObject heldItem;
    private ItemInterface heldItemScript;
    private GameObject inventoryItem1 = null;
    private GameObject inventoryItem2 = null;
    private GameObject inventoryItem3 = null;

    
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
        inventoryManager = gameObject.GetComponent<InventoryManager>();

        groundLayer = LayerMask.GetMask("GroundLayer");

        //Initalize values
        playerHeight = 2f;
        playerJumpOffset = 0.3f;
        jumpHeight = 175f;
        moveSpeed = 1.75f;
        airMultiplier = 1f;
        groundDrag = 0.25f;
        runMult = 2.5f;

        //Player Movement Input
        playerInput = new PlayerInput();
        playerInput.Enable();

        playerInput.PlayerMovement.WASD.performed += WASDperformed;
        playerInput.PlayerMovement.WASD.canceled += WASDcanceled;
        playerInput.PlayerMovement.Space.performed += SPACEperformed;
        playerInput.PlayerMovement.Shift.started += SHIFTstarted;
        playerInput.PlayerMovement.Shift.canceled += SHIFTcanceled;

        //Player Action Input
        playerInput.PlayerActions.LeftClick.performed += CLICKperformed;
        playerInput.PlayerActions._1.performed += ONEperformed;
        playerInput.PlayerActions._2.performed += TWOperformed;
        playerInput.PlayerActions._3.performed += THREEperformed;
        playerInput.PlayerActions.E.performed += EPerformed;

        //Player UI Input
        playerInput.UI.Esc.performed += ESCPressed;

        //For Debug delete later
        if (heldItem != null)
        {
            heldItemScript = heldItem.GetComponent<ItemInterface>();
        }
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

    //Movement

    private void WASDperformed(InputAction.CallbackContext context)
    {
        wasdVector = context.ReadValue<Vector2>();
    }

    private void WASDcanceled(InputAction.CallbackContext context)
    {
        wasdVector = EMPTY_VECTOR;
    }

    private void SPACEperformed(InputAction.CallbackContext context)
    {
        if (gameManager.gameState == GameManager.GameState.GAMEPLAY && grounded) playerRigidbody.AddForce(transform.up * jumpHeight);
    }

    private void SHIFTstarted(InputAction.CallbackContext context)
    {
        shiftPressed = true;
    }

    private void SHIFTcanceled(InputAction.CallbackContext context)
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

        rigidBodyVelocity = playerRigidbody.velocity;
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

    //Actions
    private void CLICKperformed(InputAction.CallbackContext context)
    {
        if (heldItem != null)
        {
            if (heldItemScript != null)
            {
                heldItemScript.interact();
            }
            else
            {
                Debug.Log("Not holding valid item");
            }
        }
        else
        {
            Debug.Log("Not holding any item");
        }
    }

    private void ONEperformed(InputAction.CallbackContext context)
    {
        //First if statement checks if item is already equiped and then it will unequip it
        if (heldItem == inventoryItem1)
        {
            if (heldItemScript != null)
            {
                heldItemScript.unequip();
            }
            else if (heldItem != null)
            {
                Debug.Log("Error: Item does not have a valid script");
            }
            heldItem = null;
            heldItemScript = null;
        }
        //If the item is not equipped, unequip previous item then equip new item
        else
        {
            if (heldItemScript != null)
            {
                heldItemScript.unequip();
            }
            else if (heldItem != null)
            {
                Debug.Log("Error: Item does not have a valid script");
            }
            heldItem = inventoryItem1;
            if (heldItem != null)
            {
                heldItemScript = heldItem.GetComponent<ItemInterface>();
                if (heldItemScript != null)
                {
                    heldItemScript.equip();
                }
                else if (heldItem != null)
                {
                    Debug.Log("Error: Item does not have a valid script");
                }
            }
        }
    }
    private void TWOperformed(InputAction.CallbackContext context)
    {
        //First if statement checks if item is already equiped and then it will unequip it
        if (heldItem == inventoryItem2)
        {
            if (heldItemScript != null)
            {
                heldItemScript.unequip();
            }
            else if (heldItem != null)
            {
                Debug.Log("Error: Item does not have a valid script");
            }
            heldItem = null;
            heldItemScript = null;
        }
        //If the item is not equipped, unequip previous item then equip new item
        else
        {
            if (heldItemScript != null)
            {
                heldItemScript.unequip();
            }
            else if (heldItem != null)
            {
                Debug.Log("Error: Item does not have a valid script");
            }
            heldItem = inventoryItem2;
            if (heldItem != null)
            {
                heldItemScript = heldItem.GetComponent<ItemInterface>();
                if (heldItemScript != null)
                {
                    heldItemScript.equip();
                }
                else if (heldItem != null)
                {
                    Debug.Log("Error: Item does not have a valid script");
                }
            }
        }
    }
    private void THREEperformed(InputAction.CallbackContext context)
    {
        //First if statement checks if item is already equiped and then it will unequip it
        if (heldItem == inventoryItem3)
        {
            if (heldItemScript != null)
            {
                heldItemScript.unequip();
            }
            else if (heldItem != null)
            {
                Debug.Log("Error: Item does not have a valid script");
            }
            heldItem = null;
            heldItemScript = null;
        }
        //If the item is not equipped, unequip previous item then equip new item
        else
        {
            if (heldItemScript != null)
            {
                heldItemScript.unequip();
            }
            else if (heldItem != null)
            {
                Debug.Log("Error: Item does not have a valid script");
            }
            heldItem = inventoryItem3;
            if (heldItem != null)
            {
                heldItemScript = heldItem.GetComponent<ItemInterface>();
                if (heldItemScript != null)
                {
                    heldItemScript.equip();
                }
                else if (heldItem != null)
                {
                    Debug.Log("Error: Item does not have a valid script");
                }
            }
        }
    }

    public int getHeldItemSlot()
    {
        if (heldItem == inventoryItem1)
        {
            return 0;
        }
        else if (heldItem == inventoryItem2)
        {
            return 1;
        }
        else if (heldItem == inventoryItem3)
        {
            return 2;
        } else
        {
            return 404;
        }
    }

    private void EPerformed(InputAction.CallbackContext context)
    {

    }

    //UI Interactions
    private void ESCPressed(InputAction.CallbackContext context)
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
