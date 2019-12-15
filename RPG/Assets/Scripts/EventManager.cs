using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventManager : MonoBehaviour
{


    #region Singleton
    public static EventManager instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    #endregion

    // Inventory Events
    public ItemEvent onItemAdded = new ItemEvent();
    public ItemEvent onItemRemoved = new ItemEvent();
    public ItemEvent inventoryChanged = new ItemEvent();

    // Quest Events
    public QuestEvent onQuestAdded = new QuestEvent();
    public QuestEvent onQuestCompleted = new QuestEvent();

    // Objective Events
    public ObjectiveEvent onObjectiveStarted = new ObjectiveEvent();
    public ObjectiveEvent onObjectiveCompleted = new ObjectiveEvent();
    

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
}

public class ItemEvent : UnityEvent<Item> { }
public class QuestEvent : UnityEvent<Quest> { }
public class ObjectiveEvent : UnityEvent<Objective> { }