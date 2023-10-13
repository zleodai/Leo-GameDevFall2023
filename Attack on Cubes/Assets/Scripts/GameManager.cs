using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Manages UI and Time Control
    //Calls other manager functions
    //In Charge of gameloop

    public static GameManager instance = null;
    public List<GameObject> rings;
    public TextMeshProUGUI winUI;

    //Managers
    private StatusManager statusManager;

    public int gameMode;
    //For now:
    //0 = Menu State
    //1 = Gameplay State
    //2 = Paused State

    public bool paused;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);


        //For Fps
        Application.targetFrameRate = 60;

        //Gets Managers
        statusManager = GetComponent<StatusManager>();

        //For now gamemode = 1 because i need to get the physics worked out
        gameMode = 1;

        //Paused set to false obv
        paused = false;

        winUI.enabled = false;
    }


    private void LateUpdate()
    {
        if (gameMode == 0)
        {
            //Code for Menu
        } else if (gameMode == 1)
        {
            


        } else if (gameMode == 2)
        {
            //Code for Pauses
        }

        if (rings.Count <= 0)
        {
            winUI.enabled = true;
        }
    }

    public void removeMe(GameObject x)
    {
        GameObject removedObj = null;
        foreach (GameObject obj in rings)
        {
            if (x.GetInstanceID() == obj.GetInstanceID())
            {
                removedObj = obj;
            }
        }

        if (removedObj != null){
            rings.Remove(removedObj);
            Destroy(x);
        }
    }
}
