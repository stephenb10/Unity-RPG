using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{


    // TO DO
    // - Change from scriptable object to normal class. DONE
    // - Each quest stored in the quest database scriptable object 
    // - Reference quests from the quest database DONE
    // - Save and load the quests the player has to json file DONE
    // - Adding a new quest to the player comes from the quest database
    // - When creating quests they are added to the databse
    // - Save quest databse to a json file as well, if new quests are generated 
    //   or created at runtime
    // - Reference quests by quest id??



    // Reference to all quests 
    public QuestDatabase questDatabase;
    // List of current quests the player has
    public List<Quest> quests = new List<Quest>();

    // Start is called before the first frame update
    void Start()
    {        
        
        loadQuests();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
            saveQuests();
    }

    // Gets an array of quests from DataManager
    // Adds each quest to the list
    public void loadQuests(){
        quests.Clear();
        Quest[] loadedQuests = DataManager.loadQuests();
        foreach (Quest q in loadedQuests)
        {
            // 
            //Quest qDB = QuestFromID(q.questID);
            //qDB.completed = q.completed;

            quests.Add(q);

        }
        
        
            
    }
    // Converts id to Quest
    // PARAMS - id of quest
    // RETURNS - Quest with equivalent questID
    Quest QuestFromID(int id)
    {
        foreach (Quest q in questDatabase.quests)
            if (q.questID == id)
                return q;

        Debug.LogError("Could not find quest for id: " + id);
        return null;
    }


    // Add a quest to the list of quests
    // PARAMS - Quest to add
    public void addQuest(Quest quest)
    {
        if (hasQuest(quest.questID))
        {
            Debug.LogError("Trying to add Quest '" + quest.questName + "' but it is already added!");
            return;
        }

        quests.Add(quest);
    }


    // Parses the list of quests to the DataManager to save to a JSON file
    public void saveQuests(){
        DataManager.saveQuests(quests);
    }

    // Checks if the parsed quest is active.
    // Checks the quest list if it is containted within and if the quest is completed.
    // PARAMS - Quest, the quest to check if it is active
    // RETURNS - bool quest active or not
    public bool isQuestActive(Quest quest){
        return quests.Contains(quest) && !quest.completed;
    }

    // Checks if the parsed quest is complete
    // PARAMS - Quest, the quest to check if completed
    // RETURNS - bool quest complete or not
    public bool isQuestComplete(Quest quest)
    {
        return quests.Contains(quest) && quest.completed;
    }

    // Checks if the player has the parsed quest
    // PARAMS - Quest, the quest to check 
    // RETURNS - bool if player has quest
    public bool hasQuest(Quest quest)
    {
        return quests.Contains(quest);
    }

    // Checks if the player has the quest from the parsed id
    // PARAMS - ID, the quest id to check 
    // RETURNS - bool if player has quest
    public bool hasQuest(int id)
    {
        foreach (Quest q in questDatabase.quests)
            if (q.questID == id)
            {
                return true;
            }

        return false;
    }
}
