using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraScript : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform playerPosition;

    public float xRotation;
    public float yRotation;

    private PlayerInput inputActions;

    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Enable();

        inputActions.PlayerMovement.Look.performed += Look;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Look(InputAction.CallbackContext context)
    {
        Vector2 lookResult = context.ReadValue<Vector2>();
        lookResult.x = lookResult.x * Time.deltaTime * sensX;
        lookResult.y = lookResult.y * Time.deltaTime * sensY;

        yRotation += lookResult.x;
        xRotation += -lookResult.y;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void Update()
    {
        transform.position = playerPosition.position;
    }
}
