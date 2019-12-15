using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Database", menuName = "Quest Databse")]
public class QuestDatabase : ScriptableObject
{
   public List<Quest> quests = new List<Quest>();
}
