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

    //Components
    private Rigidbody rb;
    private Transform objTransform;
    private InputActions playerInputActions;

    //For Jumping (more details in Awake()
    private float jumpValue;
    private bool isGrounded;

    //For Movement
    private float speed;

    //For Camera Movement
    public float viewSensitivity;
    public Vector2 mouseChangeVector;
    public float viewY;

    private void Awake()
    {
        //For Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Components
        rb = GetComponent<Rigidbody>();
        objTransform = GetComponent<Transform>();

        //Player Input Sys em
        playerInputActions = new InputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;

        //For Jump Height
        jumpValue = 8f;

        //For speed of movement
        speed = 1f;

        //For View Sensitivity
        viewSensitivity = 5f;
        viewY = 0f;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpValue, ForceMode.VelocityChange);
                Debug.Log("jumped");
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            //Movement when grounded 
            Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
            rb.AddForce(new Vector3(inputVector.x, 0.0f, inputVector.y) * speed, ForceMode.Impulse);
        } else
        {
            //Movement when in air (reduced speed)
            Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
            rb.AddForce(new Vector3(inputVector.x, 0.0f, inputVector.y) * speed *1/3, ForceMode.Impulse);
        }

        mouseChangeVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        viewY += mouseChangeVector.x * viewSensitivity;

        Vector3 rotationalChange = new Vector3(0f, viewY, 0f) * Time.deltaTime * viewSensitivity;
        Quaternion rotationChange = new Quaternion();
        rotationChange.eulerAngles = rotationalChange;
        objTransform.rotation = rotationChange;
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Touched Ground");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Left Ground");
        }
    }
}
