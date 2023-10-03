using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //Singleton 
    public static PlayerInput instance = null;
    public StatusManager statusManager;
    private Rigidbody rigidBody;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        statusManager = GameObject.FindFirstObjectByType<StatusManager>();

        rigidBody = GetComponent<Rigidbody>();
        Debug.Log(rigidBody.gameObject.name);


    }


}
