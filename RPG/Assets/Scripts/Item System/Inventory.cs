using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{


    List<ItemData> items;
    int invSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check inventory if it holds the parsed item
    // PARAMS - Item to check for
    // RETURNS - bool if inventory has item
    public bool hasItem(Item item)
    {
        return itemToData(item) != null;
    }

    // Check the stack of the parsed item
    // PARAMS - Item to check for
    // RETURNS - int ammount of item stacked
    public int ammountOfItem(Item item){
        ItemData data = itemToData(item);
        if (data == null)
            return 0;
        return data.stackSize;
    }

    // Adds the parsed item to the list,
    // or increases the stack if it is already in the list
    // It then triggers onItemAdded and inventoryChanged events
    // PARAMS - Item to add
    public void addItem(Item item)
    {

        if (items.Count >= invSize)
            return;

        ItemData data = itemToData(item);

        if (data != null)
            if (data.canStack())
            {
                data.stackSize++;
                return;
            }

        items.Add(new ItemData(item));

        GameManager.instance.events.onItemAdded.Invoke(item);
        GameManager.instance.events.inventoryChanged.Invoke(item);

    }

    // from JSon
    void addItems()
    {

    }

    // to Json
    void saveItems()
    {

    }

    // Clears the inventory
    public void clearInventory()
    {
        items.Clear();
    }

    // Removes the parsed item to the list,
    // or decreases the stack if it is already in the list
    // It then triggers onItemRemoved and inventoryChanged events
    // PARAMS - Item to remove
    public void removeItem(Item item)
    {
        ItemData data = itemToData(item);
        if (data != null)
            if (data.stackSize < 1)
                items.Remove(data);
            else
                data.stackSize--;

        GameManager.instance.events.onItemRemoved.Invoke(item);
        GameManager.instance.events.inventoryChanged.Invoke(item);
    }

    // Drops the item in the gameworld and remove it from the inventory
    // PARAMS - Item to drop
    public void dropItem(Item item)
    {
        // Spawn gameobject in world
        removeItem(item);
    }

    // Retrives ItemData from list from the item parsed
    // PARAMS - Item to convert
    // RETURNS - Equivalent ItemData to parsed item
    public ItemData itemToData(Item item)
    {
        foreach (ItemData data in items)
            if (data.item == item)
                return data;

        return null;
    }
}
