using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    /* [System.NonSerialized] does not convert to json, so we dont need to save the 
     * name and description of the quest only reference the ID and wether the quest is completed.
     */
    public int questID;
   // [System.NonSerialized]
    public string questName;
   // [System.NonSerialized]
    public string questDescription;
    public List<Objective> objectives = new List<Objective>();
    public bool completed;
    //List<Rewards> rewards;

    // Called when the quest has been added to the quest Manager
    // Or called on startup
    // Subscribe to objective completed event
    public void init()
    {
        // Calls the quest added event, parsing this quest
        GameManager.instance.events.onQuestAdded.Invoke(this);
        // Subscribe the checkObjectiveCompleted function to obObjectiveCompleted
        GameManager.instance.events.onObjectiveCompleted.AddListener(checkObjectiveCompleted);
    }

    // Check if all objectives are completed then trigger quest completed event with reference to this quest
    public void checkCompleted()
    {
        foreach (Objective objective in objectives)
            if (!objective.completed)
                return;

        completed = true;
        GameManager.instance.events.onQuestCompleted.Invoke(this);
    }

    // When an objective is completed, check if it is one of the Quests objectives
    // If it is check if the quest is completed
    // PARAMS - Objective, the objective parsed from the event
    public void checkObjectiveCompleted(Objective objective)
    {
        foreach (Objective obj in objectives)
            if(obj == objective)
            {
                checkCompleted();
                return;
            }
    }
    

}

