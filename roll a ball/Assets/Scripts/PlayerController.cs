using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //For Singleton
    public static PlayerController instance = null;

    //Camera Singleton
    public CameraController cameraController;

    //Components
    private Rigidbody rb;
    private InputActions playerInputActions;

    //For Jumping (more details in Awake()
    private float jumpValue;
    private bool isGrounded;

    //For Movement
    private float speed;

    //For Camera Movement
    public Vector2 deltaLook;

    //For GameWinCondition
    private int PickUps;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI WinSign;

    private void Awake()
    {
        //For Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Components
        rb = GetComponent<Rigidbody>();

        //Player Input Intialize and Jump
        playerInputActions = new InputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;

        //For Jump Height
        jumpValue = 10f;

        //For speed of movement
        speed = 1f;

        PickUps = 0;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        WinSign.gameObject.SetActive(false);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpValue, ForceMode.VelocityChange);
                //Debug.Log("jumped");
            }
        }
    }

    private Vector2 movementHelper(float yAngle, Vector2 inputVector)
    {
        Vector2 outputVector = new Vector2();

        float hyp = Mathf.Sqrt(Mathf.Pow(inputVector.y, 2) + Mathf.Pow(inputVector.x, 2));
        float inputAngle;

        //Idk why something to do with quadrants or someshit but it works for now
        if (inputVector.x == 0)
        {
            inputAngle = Mathf.Asin(inputVector.y / hyp) * 57.2958f;
        }
        else
        {
            inputAngle = Mathf.Acos(inputVector.x / hyp) * 57.2958f;
        }

        float forceAngle = yAngle + inputAngle;

        //Debug.Log("Angle: " + forceAngle);
        //Debug.Log("inputY: " + inputVector.y + ", inputX: " + inputVector.x);

        outputVector.y = hyp * Mathf.Sin(forceAngle * 0.0174533f);
        outputVector.x = hyp * Mathf.Cos(forceAngle * 0.0174533f);

        //Debug.Log("Sin: " + Mathf.Sin(forceAngle * 0.0174533f) + ", Cos: " + Mathf.Cos(forceAngle * 0.0174533f));
        //Debug.Log("x Force: " + outputVector.x + ", z Force: " + outputVector.y);

        if (inputVector.y == 0)
        {
            outputVector.x = -outputVector.x;
        }
        else if (inputVector.x == 0)
        {
            outputVector.y = -outputVector.y;
        }

        return outputVector;
    }

    private void FixedUpdate()
    {
        Vector2 outputVector = new Vector2();
        //Movement when grounded 
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        if (Mathf.Abs(inputVector.x) > 0 || Mathf.Abs(inputVector.y) > 0)
        {
            float yAngle = cameraController.gameObject.transform.eulerAngles.y + 180;

            if (Mathf.Abs(inputVector.x) > 0 && Mathf.Abs(inputVector.y) > 0)
            {
                outputVector = movementHelper(yAngle, new Vector2(inputVector.x, 0)) + movementHelper(yAngle, new Vector2(0, inputVector.y)) /Mathf.Abs(2); 
            }
            else
            {
                outputVector = movementHelper(yAngle, inputVector);
            }
        }

        //Reduce speed if player is in air
        if (!isGrounded)
        {
            speed = 0.25f;
        }

        //Because the direction decides to be fucky could have put more effort into quadrants etc but its 3:37 am I just want a quick fix

        rb.AddForce(new Vector3(outputVector.x, 0.0f, outputVector.y) * speed, ForceMode.Impulse);
        speed = 1f;
    }

    private void Update()
    {
        //View Angle
        Vector2 lookChange = playerInputActions.Player.Look.ReadValue<Vector2>();
        deltaLook.x = lookChange.x;
        deltaLook.y = lookChange.y;

        if (PickUps == 12)
        {
            WinSign.gameObject.SetActive(true);
        }

        ScoreText.text = "Score: " + PickUps + "/12"; 
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            //Debug.Log("Touched Ground");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            //Debug.Log("Left Ground");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("Collected");
            PickUps += 1;
        }
    }
}
