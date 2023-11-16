using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    [Header("Refrences")]
    public PlayerControllerScript playerControllerScript;
    private InventoryManager inventoryManager;
    private ItemManager itemManager;
    private GameManager gameManager;
    private GameObject gameplayGUI;
    public Sprite UIMask;

    [Header("Slot Refrences")]
    private GameObject slot1;
    private Image slot1Image;
    private RectTransform slot1Transform;
    public Sprite slot1Unequiped;
    public Sprite slot1Equiped;

    private GameObject slot2;
    private Image slot2Image;
    private RectTransform slot2Transform;
    public Sprite slot2Unequiped;
    public Sprite slot2Equiped;

    private GameObject slot3;
    private Image slot3Image;
    private RectTransform slot3Transform;
    public Sprite slot3Unequiped;
    public Sprite slot3Equiped;

    private RectTransform[] slotTransformArray;
    private Sprite[] slotEquipedArray;
    private Sprite[] slotUnequipedArray;


    public int equippedSlot;

    private void Start()
    {
        gameManager = GameManager.instance;
        gameplayGUI = gameManager.GameplayGUI;
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        itemManager = ItemManager.instance;
        playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerScript>();

        slot1 = gameplayGUI.transform.Find("Slot1").gameObject;
        slot1Image = slot1.GetComponent<Image>();
        slot1Transform = slot1.GetComponent<RectTransform>();

        slot2 = gameplayGUI.transform.Find("Slot2").gameObject;
        slot2Image = slot2.GetComponent<Image>();
        slot2Transform = slot2.GetComponent<RectTransform>();

        slot3 = gameplayGUI.transform.Find("Slot3").gameObject;
        slot3Image = slot3.GetComponent<Image>();
        slot3Transform = slot3.GetComponent<RectTransform>();

        slotTransformArray = new RectTransform[3];
        slotTransformArray[0] = slot1Transform;
        slotTransformArray[1] = slot2Transform;
        slotTransformArray[2] = slot3Transform;

        slotEquipedArray = new Sprite[3];
        slotEquipedArray[0] = slot1Equiped;
        slotEquipedArray[1] = slot2Equiped;
        slotEquipedArray[2] = slot3Equiped;

        slotUnequipedArray = new Sprite[3];
        slotUnequipedArray[0] = slot1Unequiped;
        slotUnequipedArray[1] = slot2Unequiped;
        slotUnequipedArray[2] = slot3Unequiped;
    }

    private void Update()
    {
        Dictionary<int, int> heldItems = new Dictionary<int, int>();
        int heldItemCount = -1;
        int counter = 0;
        int[] slotOrder = new int[inventoryManager.inventory.Length];
        for (int i = 0; i < inventoryManager.inventory.Length; i++)
        {
            slotOrder[i] = -1;
        }
        int slotOrderCounter = 0;
        for (int i = 0; i < inventoryManager.inventory.Length; i++)
        {
            if (inventoryManager.getSlotItem(i) != 0)
            {
                if (inventoryManager.getSlotItem(i) != 0)
                {
                    heldItemCount += 1;
                    heldItems[heldItemCount] = inventoryManager.getSlotItem(i);
                    slotOrder[slotOrderCounter] = counter;
                    slotOrderCounter++;
                }
            }
            counter++;
        }

        int hotbarOrderIndex = 0;
        foreach (int slot in slotOrder)
        {
            if (slot != -1)
            {
                int xValue = (150 * hotbarOrderIndex) - (slotOrderCounter * 150 / 2);
                xValue += 75;
                slotTransformArray[slot].localPosition = new Vector3(xValue, -400, 0);
                Sprite sprite = itemManager.getSprite(inventoryManager.getSlotItem(slot));
                if (sprite != null)
                {
                    slotTransformArray[slot].Find("Image").gameObject.GetComponent<Image>().sprite = sprite;
                }
                else
                {
                    slotTransformArray[slot].Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                }
                hotbarOrderIndex++;
            }
        }

        int heldItemSlot = playerControllerScript.getHeldItemSlot();
        if (heldItemSlot != 404)
        {
            slotTransformArray[heldItemSlot].gameObject.GetComponent<Image>().sprite = slotEquipedArray[heldItemSlot];
        }

        for (int i = 0; i < inventoryManager.inventory.Length; i++)
        {
            bool slotFound = false;
            foreach (int slot in slotOrder)
            {
                if (slot == i)
                {
                    slotFound = true;
                }
            }
            if (slotFound)
            {
                slotTransformArray[i].gameObject.SetActive(true);
                if (heldItemSlot != i)
                {
                    slotTransformArray[i].gameObject.GetComponent<Image>().sprite = slotUnequipedArray[i];
                }
            } else
            {
                slotTransformArray[i].gameObject.SetActive(false);
            }
        }
    }
}
