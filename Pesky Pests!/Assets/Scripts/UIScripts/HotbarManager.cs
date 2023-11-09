using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    [Header("Refrences")]
    private InventoryManager inventoryManager;
    private ItemManager itemManager;
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

    private Image[] slotImageArray;
    private RectTransform[] slotTransformArray;
    private Sprite[] slotEquipedArray;
    private Sprite[] slotUnequipedArray;

    private void Start()
    {
        gameplayGUI = GameObject.FindGameObjectWithTag("GameplayGUI");
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        itemManager = ItemManager.instance;

        slot1 = gameplayGUI.transform.Find("Slot1").gameObject;
        slot1Image = slot1.GetComponent<Image>();
        slot1Transform = slot1.GetComponent<RectTransform>();

        slot2 = gameplayGUI.transform.Find("Slot2").gameObject;
        slot2Image = slot2.GetComponent<Image>();
        slot2Transform = slot2.GetComponent<RectTransform>();

        slot3 = gameplayGUI.transform.Find("Slot3").gameObject;
        slot3Image = slot3.GetComponent<Image>();
        slot3Transform = slot3.GetComponent<RectTransform>();

        slotImageArray = new Image[3];
        slotImageArray[0] = slot1Image;
        slotImageArray[1] = slot2Image;
        slotImageArray[2] = slot3Image;

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
        for (int i = 0; i < inventoryManager.inventory.Length; i++)
        {
            if (inventoryManager.getSlotItem(i) != 0)
            {
                if (inventoryManager.getSlotItem(i) != 0)
                {
                    heldItemCount += 1;
                    heldItems[heldItemCount] = inventoryManager.getSlotItem(i);
                }
            }
        }

        Debug.Log("Buttons: " + (heldItemCount + 1).ToString() + "\n");

        for (int i = 0; i < heldItemCount + 1; i++)
        {
            Debug.Log("Button " + (i + 1).ToString());
            switch (heldItemCount)
            {
                case 0:
                    slotTransformArray[i].localPosition = new Vector3(0, -400, 0);
                    Sprite sprite = itemManager.getSprite(inventoryManager.getSlotItem(i));
                    if (sprite != null)
                    {
                        slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite;
                    }
                    else
                    {
                        slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                    }
                    //Debug.Log("First button " + inventoryManager.getSlotItem(i).ToString());
                    //Debug.Log("Position: " + slotTransformArray[i].position.ToString());
                    break;
                case 1:
                    switch (i)
                    {
                        case 0:
                            slotTransformArray[i].localPosition = new Vector3(-75, -400, 0);
                            Sprite sprite1 = itemManager.getSprite(inventoryManager.getSlotItem(i));
                            if (sprite1 != null)
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite1;
                            }
                            else
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                            }
                            //Debug.Log("First button: " + inventoryManager.getSlotItem(i).ToString());
                            //Debug.Log("Position: " + slotTransformArray[i].position.ToString());
                            break;
                        case 1:
                            slotTransformArray[i].localPosition = new Vector3(75, -400, 0);
                            Sprite sprite2 = itemManager.getSprite(inventoryManager.getSlotItem(i));
                            if (sprite2 != null)
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite2;
                            } else
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                            }
                            //Debug.Log("Second button: " + inventoryManager.getSlotItem(i).ToString());
                            //Debug.Log("Position: " + slotTransformArray[i].position.ToString());
                            break;
                    }
                    break;
                case 2:
                    switch (i)
                    {
                        case 0:
                            slotTransformArray[i].localPosition = new Vector3(-150, -400, 0);
                            Sprite sprite3 = itemManager.getSprite(inventoryManager.getSlotItem(i));
                            if (sprite3 != null)
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite3;
                            }
                            else
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                            }
                            //Debug.Log("First button: " + inventoryManager.getSlotItem(i).ToString());
                            //Debug.Log("Position: " + slotTransformArray[i].position.ToString());
                            break;
                        case 1:
                            slotTransformArray[i].localPosition = new Vector3(0, -400, 0);
                            Sprite sprite4 = itemManager.getSprite(inventoryManager.getSlotItem(i));
                            if (sprite4 != null)
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite4;
                            }
                            else
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                            }
                            //Debug.Log("Second button: " + inventoryManager.getSlotItem(i).ToString());
                            //Debug.Log("Position: " + slotTransformArray[i].position.ToString());
                            break;
                        case 2:
                            slotTransformArray[i].localPosition = new Vector3(150, -400, 0);
                            Sprite sprite5 = itemManager.getSprite(inventoryManager.getSlotItem(i));
                            if (sprite5 != null)
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite5;
                            }
                            else
                            {
                                slotTransformArray[i].gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = UIMask;
                            }
                            //Debug.Log("Third button: " + inventoryManager.getSlotItem(i).ToString());
                            //Debug.Log("Position: " + slotTransformArray[i].position.ToString());
                            break;
                    }
                    break;
            }
        }

        switch (heldItemCount)
        {
            case -1:
                slot1.SetActive(false);
                slot2.SetActive(false);
                slot3.SetActive(false);
                break;
            case 0:
                slot1.SetActive(true);
                slot2.SetActive(false); 
                slot3.SetActive(false);
                break;
            case 1:
                slot1.SetActive(true);
                slot2.SetActive(true);
                slot3.SetActive(false);
                break;
            case 2:
                slot1.SetActive(true);
                slot2.SetActive(true);
                slot3.SetActive(true);
                break;
        }
    }
}
