using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Refrences")]
    private GameObject playerObject;
    private InventoryManager playerInventoryScript;
    public GameObject spiderPrefab;
    public Transform pestParent;

    [Header("UI")]
    public GameObject EscMenu;
    public float EscMenuButtonDefaultSize;
    public float EscMenuButtonPopSize;
    public GameObject ControlsMenu;
    public GameObject GameplayGUI;
    public GameObject WinGUI;
    public GameObject LoseGUI;

    [Header("Gameplay")]
    public float timeToSpawnSpider = 30f;
    private float counter = 0f;

    public enum GameState 
    {
        MAIN_MENU,
        GAMEPLAY,
        PAUSED,
        MENU,
        WON,
        LOSE
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
        if (GameplayGUI == null)
        {
            GameplayGUI = GameObject.FindGameObjectWithTag("GameplayGUI");
        }
        if (WinGUI == null)
        {
            WinGUI = GameObject.FindGameObjectWithTag("WinGUI");
        }
        if (LoseGUI == null)
        {
            LoseGUI = GameObject.FindGameObjectWithTag("LoseGUI");
        }
    }

    private void Start()
    {
        setGameState(GameState.GAMEPLAY);
    }

    private void Update()
    {
        counter += Time.deltaTime;

        if (counter >= timeToSpawnSpider)
        {
            counter = 0f;
            Instantiate(spiderPrefab, pestParent);
        }
    }

    public void setGameState(GameState state)
    {
        //Replace with swith case later
        gameState = state;
        if (gameState == GameState.GAMEPLAY)
        {
            GameplayGUI.SetActive(true);
            EscMenu.SetActive(false);
            ControlsMenu.SetActive(false);
            WinGUI.SetActive(false);
            LoseGUI.SetActive(false);
        }
        else if (gameState == GameState.MENU)
        {
            GameplayGUI.SetActive(false);
            EscMenu.SetActive(true);
            ControlsMenu.SetActive(false);
            WinGUI.SetActive(false);
            LoseGUI.SetActive(false);
        }
        else if (gameState == GameState.WON)
        {
            GameplayGUI.SetActive(false);
            EscMenu.SetActive(false);
            ControlsMenu.SetActive(false);
            WinGUI.SetActive(true);
            LoseGUI.SetActive(false);
        }
        else if (gameState == GameState.LOSE)
        {
            GameplayGUI.SetActive(false);
            EscMenu.SetActive(false);
            ControlsMenu.SetActive(false);
            WinGUI.SetActive(false);
            LoseGUI.SetActive(true);
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

    public void winStateMet()
    {
        setGameState(GameState.WON);
    }

    public void playerDied()
    {
        setGameState(GameState.LOSE);
    }
}
