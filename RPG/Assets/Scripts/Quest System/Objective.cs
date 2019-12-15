using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective
{
    public string objectiveName;
    public string description;
    public string objectiveType;
    public bool completed;

    // Called when objective is started
    public virtual void init()
    {
        GameManager.instance.events.onObjectiveStarted.Invoke(this);
    }

    // Check if objective is completed
    public virtual void checkStatus()
    {

    }

    // Called when objective is complete
    public virtual void markAsCompleted()
    {
        completed = true;
        GameManager.instance.events.onObjectiveCompleted.Invoke(this);
    }
     
}
