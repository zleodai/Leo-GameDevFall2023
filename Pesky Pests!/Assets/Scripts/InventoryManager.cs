using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PlayerControllerScript playerControllerScript;
    public int[] inventory = { 1, 2, 1 };

    private void Start()
    {
        playerControllerScript = gameObject.GetComponent<PlayerControllerScript>();
    }

    public int addItem(int slot, int item)
    {
        if (inventory[slot] == 0)
        {
            inventory[slot] = item;
            return 0;
        } else
        {
            int oldItem = inventory[slot];
            inventory[slot] = item;
            return oldItem;
        }
    }

    public int dropItem(int slot)
    {
        int oldItem = inventory[slot];
        inventory[slot] = 0;
        return oldItem;
    }

    public bool emptySlot(int slot)
    {
        if (inventory[slot] == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public int getSlotItem(int slot)
    {
        return inventory[slot];
    }

}
