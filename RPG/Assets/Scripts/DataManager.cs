using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


public class DataManager : MonoBehaviour
{

    static string dataFolder = "/Saved Data";
    static string questFile = "/Quests.json";
    static string questDatabaseFile = "/QuestDatabase.json";

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

    }


    // STATIC function to save quests to a JSON file
    // PARAMS - quests, list of quests parsed from the quest manager
    public static void saveQuests(List<Quest> quests)
    {
        string json = JsonHelperN.ToJson<Quest>(quests.ToArray(), true);
        //string json = JsonConvert.SerializeObject(quests);
        json = JsonHelperN.replaceInJSONString(json, "Quests");
        File.WriteAllText(Application.dataPath + dataFolder + questFile, json);
    }

    // STATIC function to load quests from a JSON file
    // RETURNS - An array of quests 
    public static Quest[] loadQuests()
    {
        Quest[] quests;

        string json = JsonHelperN.getJSONstringFromFile(dataFolder, questFile, "Quests");
        quests = JsonHelperN.FromJson<Quest>(json);

        //quests = JsonConvert.DeserializeObject<Quest[]>(json);

        return quests;

    }



    // STATIC function to save quest database to a JSON file
    // PARAMS - quests, list of quests parsed from the quest manager
    public static void saveQuestDatabase(List<Quest> quests)
    {
        string json = JsonHelperN.ToJson<Quest>(quests.ToArray(), true);
        //string json = JsonConvert.SerializeObject(quests);
        json = JsonHelperN.replaceInJSONString(json, "QuestDatabase");
        File.WriteAllText(Application.dataPath + dataFolder + questDatabaseFile, json);
    }

    // STATIC function to load the quest database from a JSON file
    // RETURNS - An array of quests
    public static Quest[] loadQuestDatabase()
    {
        Quest[] quests;

        string json = JsonHelperN.getJSONstringFromFile(dataFolder, questDatabaseFile, "QuestDatabase");
        quests = JsonHelperN.FromJson<Quest>(json);

        //quests = JsonConvert.DeserializeObject<Quest[]>(json);

        return quests;

    }

   

}

// IMPORTED CLASS TO HELP WITH SAVING 
public static class JsonHelperN
{

    public static string getJSONstringFromFile(string dataFolder, string fileName, string listName)
    {
        string json = File.ReadAllText(Application.dataPath + dataFolder + fileName);
        json = json.Replace("\"" + listName + "\"", "\"JSONARRAY\"");
        return json;
    }

    public static string replaceInJSONString(string json, string listName)
    {
        return json.Replace("\"JSONARRAY\"", "\"" + listName + "\"");
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.JSONARRAY;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.JSONARRAY = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.JSONARRAY = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] JSONARRAY;
    }
}



