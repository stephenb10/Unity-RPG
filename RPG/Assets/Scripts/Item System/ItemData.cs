using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData 
{
    public Item item;
    public int stackSize = 0;

    // CONSTRUCTOR
    // Sets the item to the parsed item
    // Increases the stack size by 1
    public ItemData(Item i)
    {
        item = i;
        stackSize++;
    }
    
    // Check if the item can be further stacked
    // RETURNS - bool if can stack
    public bool canStack()
    {
        return item.stackable && item.maxStackSize > stackSize;
    }
}
