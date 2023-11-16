using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;

    [Header("Prefabs")]
    public GameObject flashlightPrefab;


    public Dictionary<int, GameObject> itemDictionary;
    public Dictionary<int, Sprite> spriteDictionary;

    private int dictionaryIndex = 1;

    public bool itemDictionaryInitialized = false;
    public bool spriteDictionaryIntialized = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        GameObject[] playerItems = GameObject.FindGameObjectsWithTag("PlayerItem");
        itemDictionary = new Dictionary<int, GameObject>();
        spriteDictionary = new Dictionary<int, Sprite>();

        foreach (GameObject item in playerItems)
        {
            if(item.GetComponent<FlashlightScript>() != null)
            {
                itemDictionary.Add(dictionaryIndex, item);
                spriteDictionary.Add(dictionaryIndex, item.GetComponent<SpriteMask>().sprite);
                dictionaryIndex++;
            }
            else if(item.GetComponent<FlamethrowerScript>() != null)
            {
                itemDictionary.Add(dictionaryIndex, item);
                spriteDictionary.Add(dictionaryIndex, item.GetComponent<SpriteMask>().sprite);
                dictionaryIndex++;
            }
        }

        itemDictionaryInitialized = true;
        spriteDictionaryIntialized = true;
    }

    public void addItem(GameObject item)
    {
        itemDictionary.Add(dictionaryIndex, item);
        spriteDictionary.Add(dictionaryIndex, item.GetComponent<SpriteMask>().sprite);
        dictionaryIndex++;
    }

    public GameObject getItem(int id)
    {
        if (id == 0)
        {
            return null;
        }
        return itemDictionary[id];
    }

    public int getId(GameObject item)
    {
        foreach(int id in itemDictionary.Keys)
        {
            if (itemDictionary[id] == item)
            {
                return id;
            }
            else if (itemDictionary[id] == item.transform.parent.gameObject)
            {
                return id;
            }
        }
        return 404;
    }

    public Sprite getSprite(int id)
    {
        if (spriteDictionary.ContainsKey(id))
        {
            return spriteDictionary[id];
        }
        return null;
    }
}
