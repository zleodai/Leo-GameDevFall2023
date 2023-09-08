using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public InputAction playerControls;

    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private float jumpValue;
    private boolean isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if(count >= 12)
        {
            winTextObject.SetActive(true);
        }
    }    

    void Update()
    {
        jumpValue = playerControls.ReadValue<float>();
    }
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, jumpValue, movementY);

        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;

            SetCountText();
        }
        if(other.gameObject)
    }
}
