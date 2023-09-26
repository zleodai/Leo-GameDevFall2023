using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHandler : MonoBehaviour
{
    //Manages the physics
    //This is so the physics is organized and optimized for pausing
    //Queues events for pauses
    //Handles player movements too so it will be pretty chunky

    public static PhysicsHandler instance = null;

    //Managers
    private GameManager gameManager;
    private StatusManager statusManager;

    //Player
    private PlayerInput playerInput;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Gets Managers
        gameManager = GetComponent<GameManager>();
        statusManager = GetComponent<StatusManager>();

        //Gets Player
        playerInput = GetComponent<PlayerInput>();
    }


    private void FixedUpdate()
    {
        if (!gameManager.paused)
        {
            //Not paused code
        }
    }
}
