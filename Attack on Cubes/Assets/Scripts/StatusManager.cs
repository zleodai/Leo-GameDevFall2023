using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    //Manages non physics events
    //Manages health/damage
    //Manages fuelcount/playerscore
    //Manages spawning in enemies
    
    public static StatusManager instance = null;

    //Managers
    private GameManager gameManager;

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

        //Gets Player
        playerInput = GetComponent<PlayerInput>();
    }   
}
