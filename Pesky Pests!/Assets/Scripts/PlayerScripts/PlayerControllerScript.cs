using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;

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
    private Transform playerCameraTransform;
    private ItemManager itemManager;

    [Header("PostProcessing")]
    VolumeProfile volumeProfile;
    Vignette vignette;

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
    private enum MovementState
    {
        Running,
        Walking,
        Paused,
        Frozen
    }
    private MovementState movementState;

    [Header("Inventory")]
    public GameObject heldItem;
    private ItemInterface heldItemScript;
    private GameObject inventoryItem1 = null;
    private GameObject inventoryItem2 = null;
    private GameObject inventoryItem3 = null;

    [Header("Actions")]
    public float hoverOverItemLength;
    private LayerMask itemLayer;
    private GameObject hoverItem;

    [Header("GUI")]
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI replaceText;
    private GameObject[] bloodSplats;
    private float maxRedColorValue = 3f;
    private float minRedColorValue = 1f;
    private float maxIntensity = 3f;
    private float minIntensity = 0.5f;

    [Header("Stats")]
    public float health;
    private float maxHealth;

    [Header("Events")]
    private Dictionary<int, Event> activeEvents;
    private Dictionary<int, float> eventTimers;
    private int eventIds = 0;
    private List<int> inactiveEvents;
    private enum Event
    {
        tookDamage
    }
    private bool tookDamageEvent;
    public float tookDamageRedColor;
    public float tookDamageIntensity;
    public float tookDamageEventTime;

    //Unity Functions
    private void Awake()
    {
        playerTransform = gameObject.transform.Find("PlayerMesh").transform;
        playerCollider = gameObject.transform.Find("PlayerMesh").GetComponent<CapsuleCollider>();
        playerRigidbody = gameObject.transform.Find("PlayerMesh").GetComponent<Rigidbody>();
        orientation = gameObject.transform.Find("Orientation");
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        inventoryManager = gameObject.GetComponent<InventoryManager>();
        playerCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;

        volumeProfile = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>().profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        groundLayer = LayerMask.GetMask("GroundLayer");
        itemLayer = LayerMask.GetMask("Item");

        //Initalize values
        playerHeight = 4f;
        playerJumpOffset = 0.3f;
        jumpHeight = 175f;
        moveSpeed = 4f;
        airMultiplier = 1f;
        groundDrag = 0.25f;
        runMult = 2.5f;
        hoverOverItemLength = 5f;

        //Debug for if public variables not assigned
        if (interactText == null || replaceText == null)
        {
            Debug.Log("Please assign interactText and replaceText in public variables for playerControllerScript object");
        }

        //For Debug delete later
        if (heldItem != null)
        {
            heldItemScript = heldItem.GetComponent<ItemInterface>();
        }

        if (maxHealth == 0f) maxHealth = 100f;
        health = maxHealth;

        //Events

        activeEvents = new Dictionary<int, Event>();
        eventTimers = new Dictionary<int, float>();
        inactiveEvents = new List<int>();

        if (tookDamageRedColor == 0f) tookDamageRedColor = 2.5f;
        if (tookDamageIntensity == 0f) tookDamageIntensity = 0.5f;
        if (tookDamageEventTime == 0f) tookDamageEventTime = 0.225f;
    }
    private void Start()
    {
        itemManager = ItemManager.instance;

        bloodSplats = GameObject.FindGameObjectsWithTag("BloodSplat");
        foreach (GameObject bloodSplat in bloodSplats)
        {
            bloodSplat.GetComponent<Image>().enabled = false;
        }

        //Player Movement Input
        playerInput = new PlayerInput();
        playerInput.Enable();

        playerInput.PlayerMovement.WASD.performed += WASDperformed;
        playerInput.PlayerMovement.WASD.canceled += WASDcanceled;
        playerInput.PlayerMovement.Space.performed += SPACEperformed;
        playerInput.PlayerMovement.Shift.started += SHIFTstarted;
        playerInput.PlayerMovement.Shift.canceled += SHIFTcanceled;

        //Player Action Input
        playerInput.PlayerActions.LeftClick.started += CLICKstarted;
        playerInput.PlayerActions.LeftClick.canceled += CLICKcanceled;
        playerInput.PlayerActions._1.performed += ONEperformed;
        playerInput.PlayerActions._2.performed += TWOperformed;
        playerInput.PlayerActions._3.performed += THREEperformed;
        playerInput.PlayerActions.E.performed += EPerformed;
        playerInput.PlayerActions.R.performed += RPerformed;
        playerInput.PlayerActions.G.performed += GPerformed;

        //For Debug
        playerInput.PlayerActions.P.performed += PPerformed;

        //Player UI Input
        playerInput.UI.Esc.performed += ESCPressed;
    }
    private void Update()
    {
        //States
        switch (gameManager.gameState)
        {
            case GameManager.GameState.GAMEPLAY:
                playerInput.Enable();
                if (shiftPressed) movementState = MovementState.Running;
                else movementState = MovementState.Walking;

                Time.timeScale = 1;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                playerTransform.rotation = orientation.rotation;

                //Movement
                MovePlayer();
                SpeedControl();

                //Actions
                hoverItem = ItemHoverOver();

                //UI
                UpdateInventoryItems();
                healthCheck();

                //Events
                EventChecker();

                break;
            case GameManager.GameState.MENU:
                playerInput.Disable();
                movementState = MovementState.Paused;
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
            default:
                break;
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
        grounded = Physics.Raycast(playerTransform.position + new Vector3(0f, playerHeight * 0.5f, 0f), Vector3.down, playerHeight + playerJumpOffset, groundLayer);


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
    private void CLICKstarted(InputAction.CallbackContext context)
    {
        if (heldItem != null)
        {
            heldItemScript = heldItem.GetComponent<ItemInterface>();
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
            //Debug.Log("Not holding any item");
        }
    }
    private void CLICKcanceled(InputAction.CallbackContext context)
    {
        if (heldItem != null)
        {
            heldItemScript = heldItem.GetComponent<ItemInterface>();
            if (heldItemScript != null)
            {
                heldItemScript.interactcancel();
            }
            else
            {
                Debug.Log("Not holding valid item");
            }
        }
        else
        {
            //Debug.Log("Not holding any item");
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
        if (heldItem == null)
        {
            return 404;
        }
        else if (heldItem == inventoryItem1)
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
    private bool inventoryFull()
    {
        foreach (int id in inventoryManager.inventory)
        {
            if (id == 0)
            {
                return false;
            }
        }
        return true;
    }
    private int getNextInventorySlot()
    {
        int count = 0;
        foreach(int id in inventoryManager.inventory)
        {
            if (id == 0)
            {
                return count;
            }
            count++;
        }
        return 404;
    }
    private int itemsInInventory()
    {
        int counter = 0;
        foreach (int id in inventoryManager.inventory)
        {
            if (id != 0)
            {
                counter++;
            }
        }
        return counter;
    }
    private GameObject ItemHoverOver()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hoverOverItemLength, itemLayer))
        {
            if (inventoryFull())
            {
                replaceText.enabled = true;
                interactText.enabled = false;
                return hit.collider.gameObject;
            } else
            {
                interactText.enabled = true;
                replaceText.enabled = false;
                return hit.collider.gameObject;
            }
        } else
        {
            interactText.enabled = false;
            replaceText.enabled = false;
            return null;
        }
    }
    private void UpdateInventoryItems()
    {
        inventoryItem1 = itemManager.getItem(inventoryManager.getSlotItem(0));
        inventoryItem2 = itemManager.getItem(inventoryManager.getSlotItem(1));
        inventoryItem3 = itemManager.getItem(inventoryManager.getSlotItem(2));
    }
    private void EPerformed(InputAction.CallbackContext context)
    {
        if (hoverItem != null)
        {
            int nextSlot = getNextInventorySlot();
            if (nextSlot != 404)
            {
                int itemId = itemManager.getId(hoverItem);
                if (itemId != 404)
                {
                    inventoryManager.addItem(nextSlot, itemId);
                    hoverItem.GetComponent<ItemInterface>().pickup();
                }
                else
                {
                    Debug.Log("Error, Item Not Found in itemDictionary");
                }
            }
            else
            {
                Debug.Log("Next slot could not be found");
            }
        }
    }
    private void RPerformed(InputAction.CallbackContext context)
    {
        if (hoverItem != null)
        {
            int slot = getHeldItemSlot();
            if (slot != 404)
            {
                int itemId = itemManager.getId(hoverItem);
                if (itemId != 404)
                {
                    int removedId = inventoryManager.removeItem(slot);
                    itemManager.getItem(removedId).GetComponent<ItemInterface>().drop();
                    inventoryManager.addItem(slot, itemId);
                    hoverItem.GetComponent<ItemInterface>().pickup();
                    heldItem = null;
                    heldItemScript = null;
                }
                else
                {
                    Debug.Log("Error, Item Not Found in itemDictionary");
                }
            }
            else
            {
                Debug.Log("Helditem slot could not be found");
            }
        }
    }
    private void GPerformed(InputAction.CallbackContext context)
    {
        int slot = getHeldItemSlot();
        if (slot != 404)
        {
            int removedId = inventoryManager.removeItem(slot);
            itemManager.getItem(removedId).GetComponent<ItemInterface>().drop();
            heldItem = null;
            heldItemScript = null;
        }
        else
        {
            Debug.Log("Helditem slot could not be found");
        }
    }

    //UI
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
    private void PPerformed(InputAction.CallbackContext context)
    {
        //Currently debug button edit later

        takeDamage(8);
    }
    public void takeDamage(float damage)
    {
        health -= damage;
        AddEvent(Event.tookDamage, tookDamageEventTime);
    }
    private void healthCheck()
    {
        int healthState = (int) (health + 20)/ 20;

        if (healthState <= 0)
        {
            bloodSplats[0].GetComponent<Image>().enabled = false;
            bloodSplats[1].GetComponent<Image>().enabled = false;
            bloodSplats[2].GetComponent<Image>().enabled = false;
            bloodSplats[3].GetComponent<Image>().enabled = false;
            vignette.color.Override(new Color(0f, 0f, 0f, 1f));
            vignette.intensity.Override(100f);
            gameManager.playerDied();
        }
        else if (!tookDamageEvent) 
        { 
            switch (healthState)
            {
                case 4:
                    bloodSplats[0].GetComponent<Image>().enabled = true;
                    bloodSplats[1].GetComponent<Image>().enabled = false;
                    bloodSplats[2].GetComponent<Image>().enabled = false;
                    bloodSplats[3].GetComponent<Image>().enabled = false;
                    healthVignetteSetter();
                    break;
                case 3:
                    bloodSplats[0].GetComponent<Image>().enabled = true;
                    bloodSplats[1].GetComponent<Image>().enabled = true;
                    bloodSplats[2].GetComponent<Image>().enabled = false;
                    bloodSplats[3].GetComponent<Image>().enabled = false;
                    healthVignetteSetter();
                    break;
                case 2:
                    bloodSplats[0].GetComponent<Image>().enabled = true;
                    bloodSplats[1].GetComponent<Image>().enabled = true;
                    bloodSplats[2].GetComponent<Image>().enabled = true;
                    bloodSplats[3].GetComponent<Image>().enabled = false;
                    healthVignetteSetter();
                    break;
                case 1:
                    bloodSplats[0].GetComponent<Image>().enabled = true;
                    bloodSplats[1].GetComponent<Image>().enabled = true;
                    bloodSplats[2].GetComponent<Image>().enabled = true;
                    bloodSplats[3].GetComponent<Image>().enabled = true;
                    healthVignetteSetter();
                    break;
                case 0:
                    bloodSplats[0].GetComponent<Image>().enabled = false;
                    bloodSplats[1].GetComponent<Image>().enabled = false;
                    bloodSplats[2].GetComponent<Image>().enabled = false;
                    bloodSplats[3].GetComponent<Image>().enabled = false;
                    vignette.color.Override(new Color(0f, 0f, 0f, 1f));
                    vignette.intensity.Override(100f);
                    break;
                default:
                    bloodSplats[0].GetComponent<Image>().enabled = false;
                    bloodSplats[1].GetComponent<Image>().enabled = false;
                    bloodSplats[2].GetComponent<Image>().enabled = false;
                    bloodSplats[3].GetComponent<Image>().enabled = false;
                    vignette.color.Override(new Color(0.08f, 0.08f, 0.08f, 1f));
                    vignette.intensity.Override(0.23f);
                    break;
            }
        }
    }
    private void healthVignetteSetter()
    {
        float healthIntensity = 1 - (health / 80);
        vignette.color.Override(new Color(healthIntensity * (maxRedColorValue - minRedColorValue) + minRedColorValue, 0f, 0f, 1f));
        vignette.intensity.Override(healthIntensity * (maxIntensity - minIntensity) + minIntensity);
    }

    //Events
    private void EventChecker()
    {
        foreach (int id in activeEvents.Keys)
        {
            eventTimers[id] -= Time.deltaTime;
            if (eventTimers[id] <= 0)
            {
                EventHandeler(id, false);
                inactiveEvents.Add(id);
            } else
            {
                EventHandeler(id, true);
            }
        }
        foreach (int id in inactiveEvents)
        {
            activeEvents.Remove(id);
            eventTimers.Remove(id);
        }
        inactiveEvents = new List<int>();
    }
    private void EventHandeler(int id, bool active){
        switch (activeEvents[id], active)
        {
            case (Event.tookDamage, true):
                if (health > 0)
                {
                    tookDamageEvent = true;
                    float healthIntensity = 1 - (health / maxHealth);
                    if (healthIntensity < 0) healthIntensity = 0;
                    float timeIntensity = (0.5f - Mathf.Abs(eventTimers[id] / tookDamageEventTime - 0.5f)) * 2f;
                    vignette.color.Override(new Color(timeIntensity * (tookDamageRedColor) + (healthIntensity * (maxRedColorValue - minRedColorValue) + minRedColorValue), 0f, 0f, 1f));
                    vignette.intensity.Override(timeIntensity * (tookDamageIntensity) + (healthIntensity * (maxIntensity - minIntensity) + minIntensity));
                }
                break;
            case (Event.tookDamage, false):
                tookDamageEvent = false;
                break;
            default:
                Debug.LogFormat("Error: {0} not added to EventHandeler", activeEvents[id]);
                break;
        }
    }
    private void AddEvent(Event evant, float time)
    {
        activeEvents[eventIds] = evant;
        eventTimers[eventIds] = time;
        eventIds++;
    }
}
