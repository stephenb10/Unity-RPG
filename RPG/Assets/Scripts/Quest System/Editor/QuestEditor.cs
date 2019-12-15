




// TO DO 

/*
 * Re create quest editor for ui elements with uss
 * Re structure editor to work with the new quests 
 */






using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class QuestEditor : EditorWindow
{


    //TODO: make Objectives chain, i.e complete objectives in order. Add a bool to check if chain.

    #region Variables
    static QuestEditor window;

    Vector2 scrollView;
    Vector2 oScroll;
    Vector2 mousePos;
    Rect questListRect;
    Rect sceneWindow = new Rect(20, 20, 200, 200);

    Texture itemicon;
    Texture talkicon;
    Texture travelicon;
    Texture saveicon;

    string searchString;

    bool contextmenuOpen;

    enum UserActions
    {
        Delete, Add
    }
    static List<bool> objectiveopen = new List<bool>();
    public static QuestDatabase database;
    //public static List<Quest> quests = new List<Quest>();
    Quest selectedQuest;
    bool clickedonquest;

    // Scene GUI Variables
    bool showSceneGUI;
    bool showObjectives;
    Objective objectiveToAdd;

    // Objective to add GUI
    int selected = 0;
    GUIContent[] questTypes;

    #endregion

    #region Init

    [MenuItem("Dev Tools/Quest Editor")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = (QuestEditor)GetWindow(typeof(QuestEditor));
        window.Show();
        window.minSize = new Vector2(700, 550);
    }

    public static void OpenFromInspector(Quest quest)
    {

        window = (QuestEditor)GetWindow(typeof(QuestEditor));
        window.Show();
        window.minSize = new Vector2(700, 550);
        window.selectedQuest = quest;
    }

    private void OnEnable()
    {
        Texture icon = EditorGUIUtility.Load("Assets/Scripts/Quest System/Editor/book.png") as Texture;
        titleContent = new GUIContent(" Quest", icon);

        itemicon = EditorGUIUtility.Load("Assets/Scripts/Quest System/Editor/swap-bag.png") as Texture;
        travelicon = EditorGUIUtility.Load("Assets/Scripts/Quest System/Editor/position.png") as Texture;
        talkicon = EditorGUIUtility.Load("Assets/Scripts/Quest System/Editor/talk.png") as Texture;
        saveicon = EditorGUIUtility.Load("Assets/Scripts/Quest System/Editor/saveicon.png") as Texture;


        questTypes = new GUIContent[]
    {
         new GUIContent("Get Items", itemicon), new GUIContent("Talk To", talkicon), new GUIContent("Travel To", travelicon), new GUIContent("Kill")
    };

        //FindAllQuests();
        findDatabase();

        objectiveopen.Clear();
        if (selectedQuest != null )
        {
            foreach (Objective o in selectedQuest.objectives)
                objectiveopen.Add(false);
        }

        showObjectives = false;
    }

    void OnFocus()
    {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;


    }

    void OnDestroy()
    {
        showObjectives = false;
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    #endregion

    #region GUI Methods

    private void OnGUI()
    {
        var e = Event.current;
        mousePos = e.mousePosition;
        DrawLayout(e);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        // Do your drawing here using Handles.
        Handles.BeginGUI();
        Handles.EndGUI();

        DrawSceneGUI();
    }

    void DrawLayout(Event e)
    {
        EditorGUILayout.BeginHorizontal();


        // All quests displayed in here
        DrawQuestList(e);

        // Quest edit info displayed here
        DrawQuestInfo(e);


        EditorGUILayout.EndHorizontal();


        UserInput(e);
    }

    void DrawQuestList(Event e)
    {
        float listWidth = position.width / 3;
        questListRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(listWidth));
        {

            // Top BAR
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUILayout.LabelField("Quests", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();

                // Save button
                if (GUILayout.Button(saveicon, GUILayout.Width(30), GUILayout.Height(30)))
                    DataManager.saveQuestDatabase(database.quests);

                GUILayout.FlexibleSpace();

                // Search field
                searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.MinWidth(100));
                if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                {
                    // Remove focus if cleared
                    searchString = "";
                    GUI.FocusControl(null);
                }
                GUILayout.FlexibleSpace();


                // Add a new quest + button
                if (GUILayout.Button("+", EditorStyles.miniButtonRight))
                {
                    CreateQuest();
                }
            }
            GUILayout.EndHorizontal();

            // List of quests
            scrollView = GUILayout.BeginScrollView(scrollView);
            {
                if (database.quests.Count <= 0)
                    GUILayout.Label("Add a Quest!", EditorStyles.centeredGreyMiniLabel);

                foreach (Quest q in database.quests)
                {
                    //if(searchString != "")
                    //if (!q.questName.ToLower().Contains(searchString))
                    //    continue;

                    if (selectedQuest == q)
                    {
                        if (GUILayout.Button(q.questName, EditorStyles.toolbarButton))
                        {

                            clickedonquest = true;
                            
                            if (e.button == 1)
                            {
                                
                                DeleteMenu(q);
                            }
                        }
                    }
                    else if (GUILayout.Button(q.questName, EditorStyles.radioButton))
                    {
                        clickedonquest = true;
                        
                        if (e.button == 0)
                        {
                            objectiveopen.Clear();
                            foreach (Objective o in q.objectives)
                                objectiveopen.Add(false);

                            SelectQuest(q);
                        }

                        if (e.button == 1)
                        {
                            DeleteMenu(q);
                        }

                    }
                }
            }
            EditorGUILayout.EndScrollView();


        }
        EditorGUILayout.EndVertical();
    }

    void DrawQuestInfo(Event e)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {

            // TOP BAR
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Edit Quest", EditorStyles.boldLabel);
                if (selectedQuest != null)
                {
                    EditorGUILayout.LabelField(selectedQuest.questName, EditorStyles.boldLabel);
                    if (GUILayout.Button("Delete", EditorStyles.miniButtonRight, GUILayout.MaxWidth(60)))
                        DeleteAsset(selectedQuest);
                }

            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Displays the selects quests information
            if (selectedQuest != null)
            {
                Quest q = selectedQuest;

                QuestManager qm = FindObjectOfType<QuestManager>();
                if (qm)
                {
                    if (!qm.hasQuest(selectedQuest))
                    {
                        if (GUILayout.Button("Add quest to Quest Manager"))
                            qm.quests.Add(selectedQuest);
                    }
                    else
                    {
                        if (GUILayout.Button("Remove quest from Quest Manager"))
                            qm.quests.Remove(selectedQuest);
                    }
                }


                //if(GUILayout.Button("Add Quest to dialogue"))
                //{
                //    EditorDialogue.DialogueEditor.OpenFromQuestEditor(selectedQuest);
                //}

                GUILayout.Space(10);

                ////File Location Button
                //GUILayout.BeginHorizontal();
                //{
                //    GUILayout.Label("File Location:", EditorStyles.miniLabel);
                //    if (GUILayout.Button(q.questName, EditorStyles.miniButtonLeft, GUILayout.Width(500)))
                //    {
                //        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Quests/" + q.name + "/" + q.name + ".asset");
                //        EditorGUIUtility.PingObject(Selection.activeObject);

                //    }
                //    GUILayout.EndHorizontal();
                //}

                //GUILayout.Space(10);

                // Quest General Info
                //EditorGUI.BeginChangeCheck();
                q.questName = EditorGUILayout.TextField(new GUIContent("Quest Name"), q.questName);
                GUILayout.Label("Quest Description");
                q.questDescription = EditorGUILayout.TextArea(q.questDescription, GUILayout.Height(50));
                //q.itemReward = (Item)EditorGUILayout.ObjectField("Item Reward", q.itemReward, typeof(Item), false);
                //q.nextQuest = (Quest)EditorGUILayout.ObjectField("Next Quest", q.nextQuest, typeof(Quest), false);

                GUILayout.Space(15);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Space(20);

                // Top BAR Objectives
                GUILayout.BeginHorizontal(EditorStyles.wordWrappedMiniLabel);
                {
                    EditorGUILayout.LabelField("Objectives", EditorStyles.boldLabel);
                    selected = EditorGUILayout.Popup(new GUIContent("Objective Type"), selected, questTypes);

                    // Add a new Objective
                    if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.MaxWidth(40)))
                        AddObjective(q);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                // Objective List
                oScroll = GUILayout.BeginScrollView(oScroll);
                {
                    if (q.objectives.Count > 0)
                        foreach (Objective o in q.objectives.ToArray())
                        {
                            DrawObjective(o, q);

                        }
                }
                GUILayout.EndScrollView();

                //if (EditorGUI.EndChangeCheck())
                //{
                    
                //}

                //EditorUtility.SetDirty(selectedQuest);
            }
        }
        EditorGUILayout.EndVertical();
    }

    void DrawObjective(Objective o, Quest q)
    {
        //EditorUtility.SetDirty(o);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.Space(5);

            GUIContent title = new GUIContent();
            if (o is ItemObjective) title = new GUIContent("Item Objective: " + o.objectiveName, itemicon);
            //if (o is TravelObjective)title = new GUIContent("Travel Objective: " + o.ObjectiveName, travelicon);
            //if (o is TalkObjective) title = new GUIContent("Talk Objective: " + o.ObjectiveName, talkicon);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("/\\", GUILayout.Width(20)))
            {
                int oIndex = q.objectives.IndexOf(o);
                if (oIndex > 0)
                    Swap(q.objectives, oIndex, oIndex - 1);
            }
            if (GUILayout.Button("\\/", GUILayout.Width(20)))
            {
                int oIndex = q.objectives.IndexOf(o);
                if (oIndex != q.objectives.Count - 1)
                    Swap(q.objectives, oIndex, oIndex + 1);
            }
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 560;
            objectiveopen[q.objectives.IndexOf(o)] = EditorGUILayout.Foldout(objectiveopen[q.objectives.IndexOf(o)], new GUIContent(title), true);
            if (objectiveopen[q.objectives.IndexOf(o)])
            {
                EditorGUIUtility.labelWidth = 0;
                o.objectiveName = EditorGUILayout.TextField(new GUIContent("Objective Name"), o.objectiveName);

                if (o is ItemObjective)
                {
                    ItemObjective i = (ItemObjective)o;
                    i.item = (Item)EditorGUILayout.ObjectField("Item To Get", i.item, typeof(Item), false);
                }

                //if (o is TravelObjective)
                //{
                //    //TravelObjective i = (TravelObjective)o;
                //    if (GUILayout.Button("Add to gameobject"))
                //    {
                //        objectiveToAdd = o;
                //        GetWindow<SceneView>().Focus();
                //    }
                //}

                //if(o is TalkObjective)
                //{
                //    TalkObjective i = (TalkObjective)o;
                //    if (GUILayout.Button("Add to Dialogue"))
                //    {
                //        objectiveToAdd = o;
                //        EditorDialogue.DialogueEditor.OpenFromQuestEditor(i);
                //    }
                //}


                if (GUILayout.Button("Delete", EditorStyles.miniButtonRight, GUILayout.MaxWidth(60)))
                {
                    q.objectives.Remove(o);
                    //DeleteAsset(o);
                }

            }

            GUILayout.Space(5);
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

    }

    void WindowScene(int id)
    {
        // showSceneGUI = EditorGUILayout.Toggle("minimse", showSceneGUI);

        GUILayout.Label("Testing label");



        if (showSceneGUI)
        {
            //if (objectiveToAdd)
            //{
            //    GUILayout.Label("Ready to add " + objectiveToAdd.ObjectiveName + " from quest " + objectiveToAdd.parentQuest.QuestName);
            //    if (showObjectives && Selection.activeGameObject && Selection.activeGameObject.GetComponent<LocationObjective>())
            //        if (GUILayout.Button("Add objective to object"))
            //            Selection.activeGameObject.GetComponent<LocationObjective>().objectives.Add((TravelObjective)objectiveToAdd);
            //}

            //if (GUILayout.Button(new GUIContent("Find objectives")))
            //    showObjectives = true;

            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            //GUILayout.Space(5);

            //if (showObjectives)
            //{
            //    foreach (LocationObjective g in FindObjectsOfType<LocationObjective>())
            //    {
            //        if (GUILayout.Button(new GUIContent(g.gameObject.name)))
            //        {
            //            Selection.activeObject = g.gameObject;
            //            EditorGUIUtility.PingObject(Selection.activeObject);
            //            SceneView.lastActiveSceneView.FrameSelected();
            //        }
            //    }
            //}
        }



    }

    void DrawSceneGUI()
    {

        BeginWindows();

        //sceneWindow = GUI.Window(0, sceneWindow, WindowScene, "Quest Editor");

        EndWindows();

    }

    void UserInput(Event e)
    {
        if (e.button == 1)
        {
            if (!clickedonquest)
            {
                if (questListRect.Contains(mousePos))
                {
                    ContextMenu(e);
                }
            }
            clickedonquest = false;
        }
        if (e.button == 0)
        {
            if (!clickedonquest)
                clickedonquest = false;

        }
        //Debug.Log(clickedonquest);
    }

    void ContextMenu(Event e)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Quest"), false, ContextCallBack, UserActions.Add);

        menu.ShowAsContext();
    }

    void DeleteMenu(Quest q)
    {
        GenericMenu menu = new GenericMenu();
        contextmenuOpen = true;
        menu.AddItem(new GUIContent("Delete Quest"), false, ContextCallBack, q);

        menu.ShowAsContext();
    }

    void ContextCallBack(object o)
    {
        if (o is UserActions)
        {

            UserActions userActions = (UserActions)o;
            switch (userActions)
            {
                case UserActions.Add:
                    CreateQuest();
                    break;
            }
        }

        if (o is Quest)
        {
            DeleteAsset((Quest)o);
        }

        contextmenuOpen = false;

    }

    #endregion

    #region Helper Functions

    // Finds all instances of Quest Scriptable Objects
    //void FindAllQuests()
    //{
    //    quests.Clear();
    //    foreach (string s in AssetDatabase.FindAssets("t:Quest"))
    //    {
    //        Object t = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(Quest));
    //        Quest quest = (Quest)t;
    //        quests.Add(quest);
    //    }
    //}

        void findDatabase()
    {
        foreach (string s in AssetDatabase.FindAssets("t:QuestDatabase"))
        {
            Object t = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(QuestDatabase));
            database = (QuestDatabase)t;
        }
    }

    void CreateQuest()
    {
        //string assetPath = "/quest" + quests.Count.ToString() + ".asset";
        //string folder = AssetDatabase.CreateFolder("Assets/Quests", "quest" + quests.Count.ToString());
        // Quest asset = CreateInstance<Quest>();
        Quest asset = new Quest();
        //AssetDatabase.CreateAsset(asset, AssetDatabase.GUIDToAssetPath(folder) + assetPath);
        asset.questID = database.quests.Count;
        asset.questName = "Quest" + asset.questID;
        database.quests.Add(asset);
        selectedQuest = asset;
    }

    void DeleteAsset(object o)
    {
        //string path = assetdatabase.getassetpath(o);
        if (o is Quest)
        {
            Quest i = (Quest)o;
            if (i == selectedQuest)
                selectedQuest = null;

            database.quests.Remove(i);

            QuestManager qm = FindObjectOfType<QuestManager>();
            if (qm.hasQuest(i))
                qm.quests.Remove(i);

            //string folder = "assets/quests/" + i.name;
            //fileutil.deletefileordirectory(folder);
            //fileutil.deletefileordirectory(folder + ".meta");
        }
        //if (o is questobjective) // todo: add the objective to an array for objectives to delete because deleting itself in a foreach each is not good.
        //{
        //    questobjective i = (questobjective)o;
        //    objectiveopen.removeat(i.parentquest.objectives.indexof(i));
        //    i.parentquest.objectives.remove(i);
        //    assetdatabase.deleteasset(path);

        //}

        //assetdatabase.refresh();
    }

    void AddObjective(Quest q)
    {
        switch (selected)
        {
            //get item
            case 0:
                ItemObjective itemObjective = new ItemObjective();
                //AssetDatabase.CreateAsset(itemObjective, "Assets/_Project/Quests/" + q.name + "/" + "itoj" + q.objectives.Count.ToString() + ".asset");
                itemObjective.objectiveName = "New Item Objective";
                //itemObjective.parentQuest = q;
                q.objectives.Add(itemObjective);
                objectiveopen.Add(false);
                break;
                // Talk to
                //case 1:
                //    TalkObjective talkObjective = CreateInstance<TalkObjective>();
                //    AssetDatabase.CreateAsset(talkObjective, "Assets/_Project/Quests/" + q.name + "/" + "taoj" + q.objectives.Count.ToString() + ".asset");
                //    talkObjective.ObjectiveName = talkObjective.name;
                //    talkObjective.parentQuest = q;
                //    q.objectives.Add(talkObjective);
                //    objectiveopen.Add(false);
                //    break;
                //// Travel to
                //case 2:
                //    TravelObjective travelObjective = CreateInstance<TravelObjective>();
                //    AssetDatabase.CreateAsset(travelObjective, "Assets/_Project/Quests/" + q.name + "/" + "troj" + q.objectives.Count.ToString() + ".asset");
                //    travelObjective.ObjectiveName = travelObjective.name;
                //    travelObjective.parentQuest = q;
                //    q.objectives.Add(travelObjective);
                //    objectiveopen.Add(false);
                //    break;
                //case 3:
                //    break;
        }

        return;
    }

    void SelectQuest(Quest quest)
    {
        selectedQuest = quest;
    }

    //Quest GetQuest(string questName)
    //{
    //    foreach (Quest q in quests)
    //        if (q.questName == questName)
    //            return selectedQuest = q;
    //    return null;
    //}

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    #endregion
}
