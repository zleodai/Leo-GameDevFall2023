using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Enable();

        inputActions.Player.Look.performed += Look;
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
}
