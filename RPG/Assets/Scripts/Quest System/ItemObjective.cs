using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjective : Objective
{

    public Item item;
    public int itemAmmount;
    int currentAmmount;

    // Subscribe to inventoryChanged event 
    public override void init(){
        base.init();
        GameManager.instance.events.inventoryChanged.AddListener(checkItem);
    }

    // Check the objective's status
    public override void checkStatus()
    {
        base.checkStatus();
        if (currentAmmount >= itemAmmount)
            markAsCompleted();

    }

    // Check if the item changed is the same as the objective's item
    // Then update the ammount
    public void checkItem(Item i){
        if(i == item){
            currentAmmount = GameManager.instance.inventory.ammountOfItem(item);

            if (currentAmmount > 0)
                checkStatus();
        }
    }


}
