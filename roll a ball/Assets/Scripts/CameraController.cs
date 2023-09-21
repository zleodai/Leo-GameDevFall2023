using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //For Singleton
    public static CameraController instance = null;

    public GameObject player;
    private PlayerController playerController;
    private float viewSensitivity;

    private void Start()
    {
        //For Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        playerController = PlayerController.instance;
        viewSensitivity = 0.075f;
    }

    private void Update()
    {
        Vector2 lookChange = new Vector2(playerController.deltaLook.x, playerController.deltaLook.y);
        transform.eulerAngles += new Vector3(-lookChange.y * viewSensitivity, lookChange.x * viewSensitivity, 0f);

        //Debug.Log("lookChange: " + lookChange);

        //For Changing Camera Position
        float offsetMult = -2f;

        float yAngle = transform.rotation.eulerAngles.y;
        float xAngle = transform.rotation.eulerAngles.x;
        //Debug.Log("Degrees: " + yAngle + ", Radians: " + yAngle * 0.0174533);
        float xOffset = Mathf.Sin(yAngle * 0.0174533f) * offsetMult;
        float zOffset = Mathf.Cos(yAngle * 0.0174533f) * offsetMult;
        float yOffset = Mathf.Cos(xAngle * 0.0174533f) * offsetMult * 0.75f;

        Vector3 offset = new Vector3(xOffset, yOffset + 2.5f, zOffset);
        transform.position = player.transform.position + offset;
    }
}
