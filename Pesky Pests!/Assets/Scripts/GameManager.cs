using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Refrences")]
    private GameObject playerObject;
    private InventoryManager playerInventoryScript;

    [Header("UI")]
    public GameObject EscMenu;
    public float EscMenuButtonDefaultSize;
    public float EscMenuButtonPopSize;
    public GameObject ControlsMenu;

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
        //Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Player
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerInventoryScript = playerObject.GetComponent<InventoryManager>();

        //UI
        if (EscMenu == null)
        {
            EscMenu = GameObject.FindGameObjectWithTag("EscMenu");
        }
        EscMenuButtonDefaultSize = 22;
        EscMenuButtonPopSize = 23.5f;
        if (ControlsMenu == null)
        {
            ControlsMenu = GameObject.FindGameObjectWithTag("OptionsMenu");
        }
    }

    private void Start()
    {
        setGameState(GameState.GAMEPLAY);
    }

    public void setGameState(GameState state)
    {
        gameState = state;
        if (gameState == GameState.GAMEPLAY)
        {
            EscMenu.SetActive(false);
            ControlsMenu.SetActive(false);
        }
        else if (gameState == GameState.MENU)
        {
            EscMenu.SetActive(true);
            ControlsMenu.SetActive(false);
        }
    }

    public void optionsState(bool mode)
    {   
        if (mode)
        {
            ControlsMenu.SetActive(true);
            EscMenu.SetActive(false);
        } else
        {
            ControlsMenu.SetActive(false);
            EscMenu.SetActive(true);
        }
    }
}
