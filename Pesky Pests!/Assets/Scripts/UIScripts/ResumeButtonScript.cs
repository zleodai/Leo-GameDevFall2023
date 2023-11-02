using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ResumeButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TextMeshProUGUI textBox;
    private GameManager gameManager;

    private void Awake()
    {
        textBox = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        textBox.fontSize = gameManager.EscMenuButtonDefaultSize;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager.gameState == GameManager.GameState.MENU)
        {
            gameManager.setGameState(GameManager.GameState.GAMEPLAY);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textBox.fontSize = gameManager.EscMenuButtonPopSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textBox.fontSize = gameManager.EscMenuButtonDefaultSize;
    }
}