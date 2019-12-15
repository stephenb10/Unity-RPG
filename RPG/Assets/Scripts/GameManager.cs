using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;
    public EventManager events;
    public ItemManager items;
    public DataManager data;
    public QuestManager quests;
    public Inventory inventory;
        
    private void Awake()
    {
        if (!instance)
            instance = this;

    }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        if (!events || !items || !data || !quests)
            Debug.LogError("One or more references are missing on GameManager!");

        onGameLoaded();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void onGameLoaded(){
        //quests.loadQuests();
    }
}
