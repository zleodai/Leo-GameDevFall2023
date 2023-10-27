using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public enum GameState
    {
        MAIN_MENU,
        GAMEPLAY,
        PAUSED,
        MENU
    }

    public GameState gameState;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        gameState = GameState.GAMEPLAY;
    }
}
