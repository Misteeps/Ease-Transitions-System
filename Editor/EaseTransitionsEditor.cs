using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

using EaseTransitionsSystem;
using ETS = EaseTransitionsSystem;

#region Data Containers
[Serializable]
public class TransitionGroup
{
    public string name;

    public bool expandList;

    public bool showTests;
    public bool showObjs;
    public bool showCode;

    public List<TransitionObject> objects;


    public TransitionGroup(string _name)
    {
        name = _name;

        expandList = false;

        showTests = true;
        showObjs = true;
        showCode = true;

        objects = new List<TransitionObject>();
    }
}

[Serializable]
public class TransitionObject
{
    public GameObject gameObject;
    public string name;

    public bool showTests;
    public bool showImports;
    public bool showEase;
    public bool showCode;

    public bool singleEase;
    public EaseFunctions ease;
    public EaseDirection direction;
    public float duration;

    public List<TransitionComponent> components;


    public TransitionObject(GameObject _gameObject)
    {
        gameObject = _gameObject;
        if (gameObject != null)
            name = gameObject.name;
        else
            name = "";

        showTests = true;
        showImports = true;
        showCode = true;
        showEase = true;

        singleEase = true;

        components = new List<TransitionComponent>();
    }
}

[Serializable]
public class TransitionComponent
{
    public ComponentTypes type;

    public bool showFields;

    public List<TransitionField> fields;


    public TransitionComponent(ComponentTypes _type)
    {
        type = _type;

        showFields = true;

        fields = new List<TransitionField>();
    }
}

[Serializable]
public class TransitionField
{
    public int enumInt;

    public EaseFunctions ease;
    public EaseDirection direction;
    public float duration;

    public float start;
    public float end;


    public TransitionField(int _enumInt)
    {
        enumInt = _enumInt;
    }
}

[Serializable]
public class ListAddress
{
    public int group;
    public int obj;


    public ListAddress()
    {
        group = -1;
        obj = -1;
    }
    public ListAddress(int _group, int _obj)
    {
        group = _group;
        obj = _obj;
    }
}
#endregion Data Containers

[InitializeOnLoad]
public class EaseTransitionsEditor : EditorWindow, IHasCustomMenu
{
    private EaseTransitionsData data;
    private EaseTransitions easeTransitions;

    private bool hoveringList;
    private Vector2 listScroll;
    private Vector2 editorScroll;

    private ListAddress hovered;
    private ListAddress selected;
    private List<ListAddress> selectedHistory;
    private int selectedHistoryPos;

    private TransitionGroup copiedGroup;
    private TransitionObject copiedObj;


    [MenuItem("Tools/Ease Transitions Editor", false, 400)]
    public static void ShowWindow()
    {
        GetWindow(typeof(EaseTransitionsEditor), false, "Ease Transitions Editor");
    }
    public EaseTransitionsEditor()
    {
        EditorApplication.playModeStateChanged += PlayModeChanged;
    }
    private void PlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            EaseTransitions[] eases = FindObjectsOfType<EaseTransitions>();
            for (int e = 0; e < eases.Length; e++)
                EaseTransitions.tObjects.Clear();

            Initialize(true);
        }
    }

    private void Initialize(bool keepData = false)
    {
        FindObjectsInScene();
        //SetWindowSize(283);

        FindEaseTransitons();

        if (!keepData)
        {
            hovered = new ListAddress();
            selected = new ListAddress();
            selectedHistory = new List<ListAddress> { new ListAddress(-1, -1) };
            selectedHistoryPos = 0;

            copiedGroup = null;
            copiedObj = null;
        }

        tObjects = new Dictionary<GameObject, ETS.TransitionObject>();
        transitioning = false;
    }
    private void SetWindowSize(int width)
    {
        if (data.showList)
            minSize = new Vector2(data.listWidth + width, 0);
        else
            minSize = new Vector2(width, 0);
    }

    private void Interaction()
    {
        #region Mouse
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            GUI.FocusControl(null);

        if (Event.current.type == EventType.MouseUp)
        {
            if (hovered.group != -1)
            {
                if (Event.current.button == 0)
                    SetAddress(selected, hovered.group, hovered.obj, true, true);
                else if (Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();

                    if (hovered.obj == -1)
                    {
                        menu.AddItem(new GUIContent("Create Group"), false, CM_CreateGroup);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Move Up"), false, CM_MoveGroupUp);
                        menu.AddItem(new GUIContent("Move Down"), false, CM_MoveGroupDown);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Copy Group"), false, CM_CopyGroup);
                        menu.AddItem(new GUIContent("Paste New Group"), false, CM_PasteGroup);
                        menu.AddItem(new GUIContent("Duplicate Group"), false, CM_DuplicateGroup);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Delete Group"), false, CM_DeleteGroup);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Create Object"), false, CM_CreateObject);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Move Up"), false, CM_MoveObjectUp);
                        menu.AddItem(new GUIContent("Move Down"), false, CM_MoveObjectDown);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Copy Object"), false, CM_CopyObject);
                        menu.AddItem(new GUIContent("Paste New Object"), false, CM_PasteObject);
                        menu.AddItem(new GUIContent("Duplicate Object"), false, CM_DuplicateObject);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Delete Object"), false, CM_DeleteObject);
                    }

                    menu.ShowAsContext();
                }
            }

            Repaint();
        }
        #endregion Mouse

        #region Selected
        if (Event.current.type == EventType.KeyDown && selected.group != -1)
        {
            if (Event.current.control)
                switch (Event.current.keyCode)
                {
                    #region Copy
                    case KeyCode.C:
                        if (selected.obj == -1)
                        {
                            copiedGroup = new TransitionGroup("");
                            copiedObj = null;

                            Copy(data.groups[selected.group], copiedGroup);
                        }
                        else
                        {
                            copiedGroup = null;
                            copiedObj = new TransitionObject(null);

                            Copy(data.groups[selected.group].objects[selected.obj], copiedObj);
                        }
                        break;
                    #endregion Copy
                    #region Paste
                    case KeyCode.V:
                        if (selected.obj == -1 && copiedGroup != null)
                        {
                            Add(data.groups, NewName(data.groups), selected.group + 1, true);
                            Paste(data.groups, copiedGroup, data.groups[selected.group], true);
                        }
                        else if (copiedObj != null)
                        {
                            Add(data.groups[selected.group].objects, (GameObject)null, selected.obj + 1, true);
                            Paste(copiedObj, data.groups[selected.group].objects[selected.obj]);
                        }
                        break;
                    #endregion Paste
                    #region Duplicate
                    case KeyCode.D:
                        if (selected.obj == -1)
                        {
                            TransitionGroup copy = new TransitionGroup("");

                            Copy(data.groups[selected.group], copy);

                            Add(data.groups, NewName(data.groups), selected.group + 1, true);
                            Paste(data.groups, copy, data.groups[selected.group], true);
                        }
                        else
                        {
                            TransitionObject copy = new TransitionObject(null);

                            Copy(data.groups[selected.group].objects[selected.obj], copy);

                            Add(data.groups[selected.group].objects, (GameObject)null, selected.obj + 1, true);
                            Paste(copy, data.groups[selected.group].objects[selected.obj]);
                        }
                        break;
                    #endregion Duplicate
                    #region Cut
                    case KeyCode.X:
                        if (selected.obj == -1)
                        {
                            copiedGroup = new TransitionGroup("");
                            copiedObj = null;

                            Copy(data.groups[selected.group], copiedGroup);

                            Remove(data.groups, selected.group);
                        }
                        else
                        {
                            copiedGroup = null;
                            copiedObj = new TransitionObject(null);

                            Copy(data.groups[selected.group].objects[selected.obj], copiedObj);

                            Remove(data.groups[selected.group].objects, selected.obj);
                        }
                        break;
                    #endregion Cut

                    #region Undo
                    case KeyCode.Z:
                        Repaint();
                        break;
                    #endregion Undo
                    #region Redo
                    case KeyCode.Y:
                        Repaint();
                        break;
                        #endregion Redo
                }
            else if (Event.current.alt)
                switch (Event.current.keyCode)
                {
                    #region Move Up
                    case KeyCode.UpArrow:
                        if (selected.obj == -1)
                            Move(data.groups, selected.group, selected.group - 1);
                        else
                            Move(data.groups[selected.group].objects, selected.obj, selected.obj - 1);
                        break;
                    #endregion Move Up
                    #region Move Down
                    case KeyCode.DownArrow:
                        if (selected.obj == -1)
                            Move(data.groups, selected.group, selected.group + 1);
                        else
                            Move(data.groups[selected.group].objects, selected.obj, selected.obj + 1);
                        break;
                        #endregion Move Down
                }
            else
                switch (Event.current.keyCode)
                {
                    #region Select Up
                    case KeyCode.UpArrow:
                        if (selected.obj == -1)
                        {
                            if (selected.group != 0)
                            {
                                if (data.groups[selected.group - 1].expandList)
                                    SetAddress(selected, selected.group - 1, data.groups[selected.group - 1].objects.Count - 1, true, true);
                                else
                                    SetAddress(selected, selected.group - 1, -1, true, true);
                            }
                        }
                        else
                        {
                            if (selected.obj != -1)
                                SetAddress(selected, null, selected.obj - 1, true, true);
                        }
                        break;
                    #endregion Select Up
                    #region Select Down
                    case KeyCode.DownArrow:
                        if (selected.obj == -1)
                        {
                            if (data.groups[selected.group].expandList && data.groups[selected.group].objects.Count != 0)
                                SetAddress(selected, null, 0, true, true);
                            else if (selected.group != data.groups.Count - 1)
                                SetAddress(selected, selected.group + 1, -1, true, true);
                        }
                        else
                        {
                            if (selected.obj != data.groups[selected.group].objects.Count - 1)
                                SetAddress(selected, null, selected.obj + 1, true, true);
                            else if (selected.group != data.groups.Count - 1)
                                SetAddress(selected, selected.group + 1, -1, true, true);
                        }
                        break;
                    #endregion Select Down

                    #region Expand
                    case KeyCode.RightArrow:
                        if (selected.obj == -1)
                        {
                            if (!data.groups[selected.group].expandList)
                                data.groups[selected.group].expandList = true;
                            else if (selected.group != data.groups.Count - 1)
                                SetAddress(selected, selected.group + 1, -1, true, true);
                        }
                        else if (selected.group != data.groups.Count - 1)
                            SetAddress(selected, selected.group + 1, -1, true, true);
                        break;
                    #endregion Expand
                    #region Collapse
                    case KeyCode.LeftArrow:
                        if (selected.obj == -1)
                        {
                            if (data.groups[selected.group].expandList)
                                data.groups[selected.group].expandList = false;
                            else if (selected.group != 0)
                                SetAddress(selected, selected.group - 1, -1, true, true);
                        }
                        else
                            SetAddress(selected, null, -1, true, true);
                        break;
                    #endregion Collapse

                    #region Delete
                    case KeyCode.Delete:
                        if (selected.obj == -1)
                            Remove(data.groups, selected.group);
                        else
                            Remove(data.groups[selected.group].objects, selected.obj);
                        break;
                        #endregion Delete
                }

            Repaint();
        }
        #endregion Selected

        #region Selected History
        if (Event.current.type == EventType.KeyDown && Event.current.alt)
        {
            switch (Event.current.keyCode)
            {
                #region Forward Selected History
                case KeyCode.RightArrow:
                    ForwardSelectedHistory();
                    break;
                #endregion Forward Selected History
                #region Backward Selected History
                case KeyCode.LeftArrow:
                    BackwardSelectedHistory();
                    break;
                    #endregion Backward Selected History
            }
        }
        #endregion Selected History
    }


    #region Transition Testing
    private Dictionary<GameObject, ETS.TransitionObject> tObjects;
    private bool transitioning;

    private bool FindEaseTransitons()
    {
        if (easeTransitions != null)
            return true;

        easeTransitions = FindObjectOfType<EaseTransitions>();

        if (easeTransitions == null)
        {
            Debug.LogWarning("EaseTransitions Component not found in current scene");
            return false;
        }

        return true;
    }
    private ETS.TransitionObject CheckTObject(GameObject gameObject)
    {
        if (!tObjects.ContainsKey(gameObject))
            tObjects.Add(gameObject, new ETS.TransitionObject(gameObject));

        return tObjects[gameObject];
    }

    private void SetTransition(GameObject gameObject, ComponentTypes component, int enumInt, EaseFunctions ease, EaseDirection direction, float duration, float start, float end, bool startPosition) => SetTransition(gameObject, new Vector2Int((int)component, enumInt), ease, direction, duration, start, end, startPosition);
    private void SetTransition(GameObject gameObject, Vector2Int enumInt, EaseFunctions ease, EaseDirection direction, float duration, float start, float end, bool startPosition)
    {
        ETS.TransitionObject tObject = CheckTObject(gameObject);
        StopTransition(tObject);
        if (!FindEaseTransitons())
            return;

        if (startPosition)
            easeTransitions.SetField(gameObject, (ComponentTypes)enumInt.x, enumInt.y, start);

        if (tObject.values.ContainsKey(enumInt))
            tObject.values[enumInt] = new TransitionValue(ease, direction, duration, start, end);
        else
            tObject.values.Add(enumInt, new TransitionValue(ease, direction, duration, start, end));

        if (!EaseTransitions.tObjects.Contains(tObject))
            EaseTransitions.tObjects.Add(tObject);

        transitioning = true;
    }

    private void StopTransition(ETS.TransitionObject tObject) => EaseTransitions.tObjects.Remove(tObject);
    private void StopAllTransitions() => EaseTransitions.tObjects.Clear();
    #endregion Transition Testing

    #region Context Menus
    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Refresh Data File"), false, CM_RefreshFile);
        menu.AddItem(new GUIContent("Clear Selected"), false, CM_ClearSelected);
    }
    private void CM_RefreshFile() => Initialize(true);
    private void CM_ClearSelected() => SetAddress(selected, -1, -1, false, true);

    private void CM_CreateGroup() => Add(data.groups, NewName(data.groups), hovered.group + 1, true);
    private void CM_CreateObject() => Add(data.groups[hovered.group].objects, (GameObject)null, hovered.obj + 1, true, true);

    private void CM_MoveGroupUp() => Move(data.groups, hovered.group, hovered.group - 1, true);
    private void CM_MoveGroupDown() => Move(data.groups, hovered.group, hovered.group + 1, true);
    private void CM_MoveObjectUp() => Move(data.groups[hovered.group].objects, hovered.obj, hovered.obj - 1, true);
    private void CM_MoveObjectDown() => Move(data.groups[hovered.group].objects, hovered.obj, hovered.obj + 1, true);

    private void CM_CopyGroup()
    {
        copiedGroup = new TransitionGroup("");
        copiedObj = null;

        Copy(data.groups[hovered.group], copiedGroup);
    }
    private void CM_PasteGroup()
    {
        if (copiedGroup == null)
            return;

        Add(data.groups, NewName(data.groups), hovered.group + 1, true);
        Paste(data.groups, copiedGroup, data.groups[selected.group], true);
    }
    private void CM_DuplicateGroup()
    {
        TransitionGroup copy = new TransitionGroup("");

        Copy(data.groups[hovered.group], copy);

        Add(data.groups, NewName(data.groups), hovered.group + 1, true);
        Paste(data.groups, copy, data.groups[selected.group], true);
    }
    private void CM_CopyObject()
    {
        copiedGroup = null;
        copiedObj = new TransitionObject(null);

        Copy(data.groups[hovered.group].objects[hovered.obj], copiedObj);
    }
    private void CM_PasteObject()
    {
        if (copiedObj == null)
            return;

        Add(data.groups[hovered.group].objects, (GameObject)null, hovered.obj + 1, true, true);
        Paste(copiedObj, data.groups[selected.group].objects[selected.obj]);
    }
    private void CM_DuplicateObject()
    {
        TransitionObject copy = new TransitionObject(null);

        Copy(data.groups[hovered.group].objects[hovered.obj], copy);

        Add(data.groups[hovered.group].objects, (GameObject)null, hovered.obj + 1, true, true);
        Paste(copy, data.groups[selected.group].objects[selected.obj]);
    }

    private void CM_DeleteGroup() => Remove(data.groups, hovered.group);
    private void CM_DeleteObject() => Remove(data.groups[hovered.group].objects, hovered.obj);
    #endregion Context Menus

    #region GUI Methods
    private void bgColor() => bgColor(Color.white);
    private void bgColor(float value) => bgColor(Color.white * (value / 10f));
    private void bgColor(Color color)
    {
        GUI.backgroundColor = color;
    }
    #endregion GUI Methods

    #region Data Methods
    private void SetAddress(ListAddress address, int? group = null, int? obj = null, bool? show = false, bool? save = false)
    {
        if (group != null)
            address.group = group.Value;
        if (obj != null)
            address.obj = obj.Value;

        if (show.Value && selected.group != -1 && selected.obj != -1)
            data.groups[selected.group].expandList = true;

        if (save.Value)
            SaveSelectedHistory();
    }
    private void CheckOutOfRangeHovered(bool? log = true)
    {
        if (hovered.group >= data.groups.Count)
        {
            if (log.Value)
                Debug.Log("Hovered Group out of range. Max value : " + (data.groups.Count - 1) + " | Current value : " + hovered.group);

            SetAddress(hovered, -1, -1);
        }

        if (hovered.group != -1)
        {
            TransitionGroup group = data.groups[hovered.group];

            if (hovered.obj >= group.objects.Count)
            {
                if (log.Value)
                    Debug.Log("Hovered Object out of range. Max value : " + (group.objects.Count - 1) + " | Current value : " + hovered.obj);

                SetAddress(hovered, null, -1);
            }
        }
    }
    private void CheckOutOfRangeSelected(bool? log = true)
    {
        if (selected.group >= data.groups.Count)
        {
            if (log.Value)
                Debug.Log("Selected Group out of range. Max value : " + (data.groups.Count - 1) + " | Current value : " + selected.group);

            SetAddress(selected, -1, -1);
        }

        if (selected.group != -1)
        {
            TransitionGroup group = data.groups[selected.group];

            if (selected.obj >= group.objects.Count)
            {
                if (log.Value)
                    Debug.Log("Selected Object out of range. Max value : " + (group.objects.Count - 1) + " | Current value : " + selected.obj);

                SetAddress(selected, null, -1);
            }
        }
    }

    private void SaveSelectedHistory()
    {
        while (selectedHistoryPos != 0)
        {
            selectedHistory.RemoveAt(0);
            selectedHistoryPos--;
        }

        selectedHistory.Insert(0, new ListAddress(selected.group, selected.obj));
    }
    private void ForwardSelectedHistory() => SeekSelectedHistory(--selectedHistoryPos);
    private void BackwardSelectedHistory() => SeekSelectedHistory(++selectedHistoryPos);
    private void SeekSelectedHistory(int pos)
    {
        selectedHistoryPos = Mathf.Clamp(pos, 0, selectedHistory.Count - 1);

        SetAddress(selected, selectedHistory[selectedHistoryPos].group, selectedHistory[selectedHistoryPos].obj, true, false);
    }

    private void FindObjectsInScene(bool? log = true)
    {
        List<string> foundObjs = new List<string>();
        List<string> emptyObjs = new List<string>();

        for (int g = 0; g < data.groups.Count; g++)
        {
            TransitionGroup group = data.groups[g];

            for (int o = 0; o < group.objects.Count; o++)
            {
                TransitionObject obj = group.objects[o];

                if (obj.name != "" && obj.gameObject == null)
                {
                    obj.gameObject = GameObject.Find(obj.name);

                    if (log.Value)
                    {
                        if (obj.gameObject != null)
                            foundObjs.Add(obj.name + "  :  " + group.name + " / Object #" + (o + 1));
                        else
                            emptyObjs.Add(obj.name + "  :  " + group.name + " / Object #" + (o + 1));
                    }
                }
            }
        }

        if (log.Value)
        {
            if (foundObjs.Count != 0)
            {
                string logFound = "Successfully found " + foundObjs.Count + " GameObjects in current scene\nGameObjects found:\n";
                for (int i = 0; i < foundObjs.Count; i++)
                    logFound += "> " + foundObjs[i] + "\n";
                Debug.Log(logFound);
            }

            if (emptyObjs.Count != 0)
            {
                string logEmpty = "Unable to find " + emptyObjs.Count + " GameObjects in current scene\nGameObjects found:\n";
                for (int i = 0; i < emptyObjs.Count; i++)
                    logEmpty += "> " + emptyObjs[i] + "\n";
                Debug.LogWarning(logEmpty);
            }
        }
    }
    private void CheckObjectNames(bool? log = true)
    {
        List<string> oldNames = new List<string>();
        List<string> newNames = new List<string>();

        for (int g = 0; g < data.groups.Count; g++)
        {
            TransitionGroup group = data.groups[g];

            for (int o = 0; o < group.objects.Count; o++)
            {
                TransitionObject obj = group.objects[o];

                if (obj.gameObject != null && obj.name != obj.gameObject.name)
                {
                    if (log.Value)
                    {
                        oldNames.Add(obj.name);
                        newNames.Add(obj.gameObject.name);
                    }

                    obj.name = obj.gameObject.name;
                }
            }
        }

        if (log.Value && oldNames.Count != 0)
        {
            string logName = "Renamed " + oldNames.Count + " objects\n";
            for (int i = 0; i < oldNames.Count; i++)
                logName += "> \"" + oldNames[i] + "\" to \"" + newNames[i] + "\"\n";
            Debug.Log(logName);
        }
    }

    private string WriteLine(int index, string content) => Write(index, content + "\n");
    private string Write(int indent, string content)
    {
        string line = "";

        for (int i = 0; i < indent * 4; i++)
            line += " ";

        line += content;

        return line;
    }
    private int CountLines(string str)
    {
        if (str == "")
            return 0;

        int lines = str.Length - str.Replace("\n", "").Length;
        return lines + 2;
    }
    private string ExportCode()
    {
        string code = "";

        code += WriteLine(0, "using System.Collections;");
        code += WriteLine(0, "using System.Collections.Generic;");
        code += WriteLine(0, "using UnityEngine;");
        code += WriteLine(0, "using UnityEngine.UI;");
        code += WriteLine(0, "using EaseTransitionsSystem;");
        code += WriteLine(0, "");
        code += WriteLine(0, "public class " + data.name.Replace(" ", "") + " : MonoBehaviour");
        code += WriteLine(0, "{");
        code += WriteLine(1, "[SerializeField] private EaseTransitions ease;");
        code += WriteLine(1, "private Dictionary<string, TransitionObject> tObjects;");
        code += WriteLine(0, "");
        code += WriteLine(1, "private void FindEaseTransitionsClass()");
        code += WriteLine(1, "{");
        code += WriteLine(2, "EaseTransitions[] eases = FindObjectsOfType<EaseTransitions>();");
        code += WriteLine(0, "");
        code += WriteLine(2, "if (eases.Length == 0)");
        code += WriteLine(2, "{");
        code += WriteLine(3, "Debug.LogError(\"EaseTransitions script not found in scene\");");
        code += WriteLine(3, "return;");
        code += WriteLine(2, "}");
        code += WriteLine(2, "else if (eases.Length > 1)");
        code += WriteLine(3, "Debug.LogWarning(\"Multiple EaseTransitions scripts found in scene, it is recommended to only have one\");");
        code += WriteLine(0, "");
        code += WriteLine(2, "ease = eases[0];");
        code += WriteLine(1, "}");
        code += WriteLine(0, "");
        code += WriteLine(1, "private void SetTObjectsList()");
        code += WriteLine(1, "{");
        code += WriteLine(2, "tObjects = new Dictionary<string, TransitionObject>();");
        List<GameObject> gameObjects = new List<GameObject>();
        for (int g = 0; g < data.groups.Count; g++)
            for (int o = 0; o < data.groups[g].objects.Count; o++)
                if (!gameObjects.Contains(data.groups[g].objects[o].gameObject))
                {
                    code += WriteLine(2, "SetTObject(\"" + data.groups[g].objects[o].name + "\");");
                    gameObjects.Add(data.groups[g].objects[o].gameObject);
                }
        code += WriteLine(1, "}");
        code += WriteLine(1, "private void SetTObject(string name)");
        code += WriteLine(1, "{");
        code += WriteLine(2, "if (tObjects.ContainsKey(name))");
        code += WriteLine(3, "return;");
        code += WriteLine(0, "");
        code += WriteLine(2, "GameObject gameObject = GameObject.Find(name);");
        code += WriteLine(2, "if (gameObject == null)");
        code += WriteLine(2, "{");
        code += WriteLine(3, "Debug.LogError(\"GameObject \\\"\" + name + \"\\\" not found\"); ");
        code += WriteLine(3, "return;");
        code += WriteLine(2, "}");
        code += WriteLine(0, "");
        code += WriteLine(2, "tObjects.Add(name, new TransitionObject(gameObject));");
        code += WriteLine(1, "}");
        code += WriteLine(0, "");
        code += WriteLine(1, "private void Start()");
        code += WriteLine(1, "{");
        code += WriteLine(2, "FindEaseTransitionsClass();");
        code += WriteLine(2, "SetTObjectsList();");
        code += WriteLine(1, "}");
        code += WriteLine(0, "");
        for (int g = 0; g < data.groups.Count; g++)
        {
            code += WriteLine(0, ExportCode(data.groups[g], 1));
            code += WriteLine(0, "");
        }
        code = code.Remove(code.Length - 1, 1);
        code += Write(0, "}");

        return code;
    }

    private void DefaultDataSettings()
    {
        data.showList = true;
        data.listWidth = 140;

        data.showObjectNames = true;
        data.autoSortFields = true;
    }

    #region Groups
    private string NewName(List<TransitionGroup> groups)
    {
        for (int i = 1; i < groups.Count + 2; i++)
            if (UniqueName(groups, "Group " + i.ToString()))
                return "Group " + i.ToString();

        return null;
    }
    private bool UniqueName(List<TransitionGroup> groups, string name)
    {
        for (int i = 0; i < groups.Count; i++)
            if (name.Replace(" ", "") == groups[i].name.Replace(" ", ""))
                return false;

        return true;
    }

    private void ReName(List<TransitionGroup> groups, int pos, string name) => ReName(groups, groups[pos], name);
    private void ReName(List<TransitionGroup> groups, TransitionGroup group, string name)
    {
        if (name.Replace(" ", "") == group.name.Replace(" ", "") || UniqueName(groups, name))
            group.name = name;
        else
            Debug.LogWarning("Group name \"" + name + "\" already exists");
    }

    private void Add(List<TransitionGroup> groups, string name, int? pos = null, bool? setSelected = false) => Add(groups, new TransitionGroup(name), pos, setSelected);
    private void Add(List<TransitionGroup> groups, TransitionGroup group, int? pos = null, bool? setSelected = false)
    {
        if (pos == null)
            pos = groups.Count;

        groups.Insert(pos.Value, group);

        if (setSelected.Value)
            SetAddress(selected, pos.Value, -1, false, true);
    }

    private void Remove(List<TransitionGroup> groups, TransitionGroup group) => Remove(groups, groups.IndexOf(group));
    private void Remove(List<TransitionGroup> groups, int? pos = null)
    {
        if (pos == null)
            pos = groups.Count - 1;

        if (pos != -1)
        {
            SetAddress(hovered, -1, -1);
            SetAddress(selected, -1, -1);

            groups.RemoveAt(pos.Value);
        }
    }

    private void Move(List<TransitionGroup> groups, int pos, int target, bool? setSelected = true) => Move(groups, groups[pos], target, setSelected);
    private void Move(List<TransitionGroup> groups, TransitionGroup group, int target, bool? setSelected = true)
    {
        target = Mathf.Clamp(target, 0, groups.Count - 1);

        Remove(groups, group);
        Add(groups, group, target);

        if (setSelected.Value)
            SetAddress(selected, groups.IndexOf(group), -1, true, true);
    }

    private void Copy(TransitionGroup oldGroup, TransitionGroup newGroup)
    {
        newGroup.name = oldGroup.name;

        newGroup.expandList = oldGroup.expandList;

        newGroup.showTests = oldGroup.showTests;
        newGroup.showObjs = oldGroup.showObjs;
        newGroup.showCode = oldGroup.showCode;

        newGroup.objects = new List<TransitionObject>();
        for (int o = 0; o < oldGroup.objects.Count; o++)
        {
            newGroup.objects.Add(new TransitionObject(null));
            Copy(oldGroup.objects[o], newGroup.objects[o]);
        }
    }
    private void Paste(List<TransitionGroup> groups, TransitionGroup oldGroup, TransitionGroup newGroup, bool? rename = true)
    {
        Copy(oldGroup, newGroup);

        if (rename.Value)
        {
            string name = newGroup.name + " Copy";
            int i = 0;

            if (!UniqueName(groups, name))
                while (!UniqueName(groups, name))
                {
                    i++;
                    name = newGroup.name + " Copy " + i.ToString();
                }
            newGroup.name = name;
        }
    }

    private void SetToStart(TransitionGroup group)
    {
        for (int o = 0; o < group.objects.Count; o++)
            SetToStart(group.objects[o]);
    }
    private void SetToEnd(TransitionGroup group)
    {
        for (int o = 0; o < group.objects.Count; o++)
            SetToEnd(group.objects[o]);
    }

    private void SetTransition(TransitionGroup group, bool startPosition)
    {
        for (int o = 0; o < group.objects.Count; o++)
            SetTransition(group.objects[o], startPosition);
    }

    private string ExportCode(TransitionGroup group, int indents = 0)
    {
        string code = "";

        code += WriteLine(indents, "private void " + group.name.Replace(" ", "") + "()");
        code += WriteLine(indents, "{");
        string tempCode = "";
        for (int o = 0; o < group.objects.Count; o++)
        {
            tempCode += WriteLine(0, ExportCode(group.objects[o], indents + 1));
            tempCode += WriteLine(0, "");

            if (tempCode.Trim('\n') == "")
                tempCode = "";
        }
        if (tempCode.Trim('\n') == "")
            tempCode = "";
        else
            tempCode = tempCode.Remove(tempCode.Length - 1, 1);
        code += Write(0, tempCode);
        code += Write(indents, "}");

        return code;
    }
    #endregion Groups

    #region Objects
    private void ReObject(TransitionObject obj, string name)
    {
        if (name == "")
            ReObject(obj, (GameObject)null);
        else if (GameObject.Find(name) == null)
            Debug.LogWarning("GameObject \"" + name + "\" not found in current scene");
        else
            ReObject(obj, GameObject.Find(name));
    }
    private void ReObject(TransitionObject obj, GameObject gameObject)
    {
        if (gameObject != null)
        {
            obj.gameObject = gameObject;
            obj.name = gameObject.name;
        }
        else
        {
            obj.gameObject = null;
            obj.name = "";
        }
    }

    private void Add(List<TransitionObject> objects, GameObject gameObject, int? pos = null, bool? setSelected = false, bool? setHoveredObject = false) => Add(objects, new TransitionObject(gameObject), pos, setSelected, setHoveredObject);
    private void Add(List<TransitionObject> objects, TransitionObject obj, int? pos = null, bool? setSelected = false, bool? setHoveredObject = false)
    {
        if (pos == null)
            pos = objects.Count;

        objects.Insert(pos.Value, obj);

        if (setSelected.Value)
            SetAddress(selected, null, pos.Value, true, true);

        if (setHoveredObject.Value)
            SetAddress(selected, hovered.group, pos.Value, true, true);
    }

    private void Remove(List<TransitionObject> objects, TransitionObject obj) => Remove(objects, objects.IndexOf(obj));
    private void Remove(List<TransitionObject> objects, int? pos = null)
    {
        if (pos == null)
            pos = objects.Count - 1;

        if (pos != -1)
        {
            SetAddress(hovered, null, -1);
            SetAddress(selected, null, -1);

            objects.RemoveAt(pos.Value);
        }
    }

    private void Move(List<TransitionObject> objects, int pos, int target, bool? setSelected = true) => Move(objects, objects[pos], target, setSelected);
    private void Move(List<TransitionObject> objects, TransitionObject obj, int target, bool? setSelected = true)
    {
        target = Mathf.Clamp(target, 0, objects.Count - 1);

        Remove(objects, obj);
        Add(objects, obj, target);

        if (setSelected.Value)
            SetAddress(selected, null, objects.IndexOf(obj), true, true);
    }

    private void Copy(TransitionObject oldObj, TransitionObject newObj)
    {
        newObj.gameObject = oldObj.gameObject;
        newObj.name = oldObj.name;

        newObj.showTests = oldObj.showTests;
        newObj.showImports = oldObj.showImports;
        newObj.showCode = oldObj.showCode;

        newObj.singleEase = oldObj.singleEase;
        newObj.ease = oldObj.ease;
        newObj.duration = oldObj.duration;

        newObj.components = new List<TransitionComponent>();
        for (int c = 0; c < oldObj.components.Count; c++)
        {
            newObj.components.Add(new TransitionComponent(oldObj.components[c].type));
            Copy(oldObj.components[c], newObj.components[c]);
        }
    }
    private void Paste(TransitionObject oldObj, TransitionObject newObj)
    {
        Copy(oldObj, newObj);
    }

    private void ImportToStart(TransitionObject obj)
    {
        for (int c = 0; c < obj.components.Count; c++)
            ImportToStart(obj.components[c], obj.gameObject);
    }
    private void ImportToEnd(TransitionObject obj)
    {
        for (int c = 0; c < obj.components.Count; c++)
            ImportToEnd(obj.components[c], obj.gameObject);
    }

    private void SetToStart(TransitionObject obj)
    {
        for (int c = 0; c < obj.components.Count; c++)
            SetToStart(obj.components[c], obj.gameObject);
    }
    private void SetToEnd(TransitionObject obj)
    {
        for (int c = 0; c < obj.components.Count; c++)
            SetToEnd(obj.components[c], obj.gameObject);
    }

    private void SetTransition(TransitionObject obj, bool startPosition)
    {
        for (int c = 0; c < obj.components.Count; c++)
            SetTransition(obj.components[c], obj.gameObject, startPosition);
    }

    private string ExportCode(TransitionObject obj, int indents = 0)
    {
        string code = "";

        if (obj.gameObject == null)
            return code;

        for (int c = 0; c < obj.components.Count; c++)
            code += Write(0, ExportCode(obj.components[c], obj.name, indents));

        if (code.Length != 0)
            code = code.Remove(code.Length - 1, 1);

        return code;
    }
    #endregion Objects

    #region Components
    private void FindComponent(List<TransitionComponent> components, GameObject gameObject, ComponentTypes type)
    {
        TransitionComponent component = null;
        for (int c = 0; c < components.Count; c++)
            if (components[c].type == type)
            {
                component = components[c];
                break;
            }

        if (gameObject.GetComponent(GetComponentFromType(type)) != null)
        {
            if (component == null)
                components.Add(new TransitionComponent(type));
        }
        else
        {
            if (component != null)
                components.Remove(component);
        }
    }
    private Type GetComponentFromType(ComponentTypes type)
    {
        switch (type)
        {
            case ComponentTypes.Transform:
                return typeof(Transform);
            case ComponentTypes.SpriteRenderer:
                return typeof(SpriteRenderer);
            case ComponentTypes.RectTransform:
                return typeof(RectTransform);
            case ComponentTypes.Image:
                return typeof(Image);
            case ComponentTypes.Text:
                return typeof(Text);
        }
        return null;
    }
    private Type GetComponentFieldsEnum(ComponentTypes type)
    {
        switch (type)
        {
            case ComponentTypes.Transform:
                return typeof(TransformFields);
            case ComponentTypes.SpriteRenderer:
                return typeof(SpriteRendererFields);
            case ComponentTypes.RectTransform:
                return typeof(RectTransformFields);
            case ComponentTypes.Image:
                return typeof(ImageFields);
            case ComponentTypes.Text:
                return typeof(TextFields);
        }
        return null;
    }

    private string GetFieldName(ComponentTypes type, int enumInt, bool? nicify = true) => GetFieldNames(type, nicify)[enumInt];
    private string[] GetFieldNames(ComponentTypes type, bool? nicify = true)
    {
        string[] names = Enum.GetNames(GetComponentFieldsEnum(type));

        if (nicify.Value)
            for (int i = 0; i < names.Length; i++)
                names[i] = ObjectNames.NicifyVariableName(names[i]);

        return names;
    }

    private void Copy(TransitionComponent oldComponent, TransitionComponent newComponent)
    {
        newComponent.type = oldComponent.type;

        newComponent.showFields = oldComponent.showFields;

        newComponent.fields = new List<TransitionField>();
        for (int f = 0; f < oldComponent.fields.Count; f++)
        {
            newComponent.fields.Add(new TransitionField(0));
            Copy(oldComponent.fields[f], newComponent.fields[f]);
        }
    }

    private void SortFields(TransitionComponent component)
    {
        Dictionary<int, TransitionField> fields = new Dictionary<int, TransitionField>();

        for (int f = 0; f < component.fields.Count; f++)
            fields.Add(component.fields[f].enumInt, component.fields[f]);

        component.fields.Clear();

        for (int i = 0; i < GetFieldNames(component.type).Length; i++)
            if (fields.ContainsKey(i))
                component.fields.Add(fields[i]);
    }

    private void ImportToStart(TransitionComponent component, GameObject gameObject)
    {
        for (int f = 0; f < component.fields.Count; f++)
            ImportToStart(component.fields[f], gameObject, component.type);
    }
    private void ImportToEnd(TransitionComponent component, GameObject gameObject)
    {
        for (int f = 0; f < component.fields.Count; f++)
            ImportToEnd(component.fields[f], gameObject, component.type);
    }

    private void SetToStart(TransitionComponent component, GameObject gameObject)
    {
        for (int f = 0; f < component.fields.Count; f++)
            SetToStart(component.fields[f], gameObject, component.type);
    }
    private void SetToEnd(TransitionComponent component, GameObject gameObject)
    {
        for (int f = 0; f < component.fields.Count; f++)
            SetToEnd(component.fields[f], gameObject, component.type);
    }

    private void SetTransition(TransitionComponent component, GameObject gameObject, bool startPosition)
    {
        for (int f = 0; f < component.fields.Count; f++)
            SetTransition(component.fields[f], gameObject, component.type, startPosition);
    }

    private string ExportCode(TransitionComponent component, string name, int indents)
    {
        string code = "";

        for (int f = 0; f < component.fields.Count; f++)
            code += WriteLine(0, ExportCode(component.fields[f], name, component.type, indents));

        return code;
    }
    #endregion Components

    #region Fields
    private int NewEnumInt(List<TransitionField> fields)
    {
        for (int f = 0; f < fields.Count + 1; f++)
            if (UniqueEnumInt(fields, f))
                return f;

        return -1;
    }
    private bool UniqueEnumInt(List<TransitionField> fields, int enumInt)
    {
        for (int f = 0; f < fields.Count; f++)
            if (fields[f].enumInt == enumInt)
                return false;

        return true;
    }

    private void ReField(List<TransitionField> fields, int pos, ComponentTypes type, int enumInt) => ReField(fields, fields[pos], type, enumInt);
    private void ReField(List<TransitionField> fields, TransitionField field, ComponentTypes type, int enumInt)
    {
        if (UniqueEnumInt(fields, enumInt))
            field.enumInt = enumInt;
        else
            Debug.LogWarning("Field \"" + GetFieldName(type, enumInt) + "\" already exists");
    }

    private void Add(List<TransitionField> fields, int enumInt, int? pos = null) => Add(fields, new TransitionField(enumInt), pos);
    private void Add(List<TransitionField> fields, TransitionField field, int? pos = null)
    {
        if (pos == null)
            pos = fields.Count;

        fields.Insert(pos.Value, field);
    }

    private void Remove(List<TransitionField> fields, TransitionField field) => Remove(fields, fields.IndexOf(field));
    private void Remove(List<TransitionField> fields, int? pos = null)
    {
        if (pos == null)
            pos = fields.Count - 1;

        if (pos != -1)
            fields.RemoveAt(pos.Value);
    }

    private void Move(List<TransitionField> fields, int pos, int target) => Move(fields, fields[pos], target);
    private void Move(List<TransitionField> fields, TransitionField field, int target)
    {
        target = Mathf.Clamp(target, 0, fields.Count - 1);

        Remove(fields, field);
        Add(fields, field, target);
    }

    private void Copy(TransitionField oldField, TransitionField newField)
    {
        newField.enumInt = oldField.enumInt;

        newField.ease = oldField.ease;
        newField.duration = oldField.duration;

        newField.start = oldField.start;
        newField.end = oldField.end;
    }

    private void ImportToStart(TransitionField field, GameObject gameObject, ComponentTypes type)
    {
        if (gameObject == null)
            return;
        if (!FindEaseTransitons())
            return;

        field.start = easeTransitions.GetField(gameObject, type, field.enumInt);
    }
    private void ImportToEnd(TransitionField field, GameObject gameObject, ComponentTypes type)
    {
        if (gameObject == null)
            return;
        if (!FindEaseTransitons())
            return;

        field.end = easeTransitions.GetField(gameObject, type, field.enumInt);
    }

    private void SetToStart(TransitionField field, GameObject gameObject, ComponentTypes type)
    {
        if (gameObject == null)
            return;
        if (!FindEaseTransitons())
            return;

        ETS.TransitionObject tObject = CheckTObject(gameObject);
        StopTransition(tObject);

        easeTransitions.SetField(gameObject, type, field.enumInt, field.start);
    }
    private void SetToEnd(TransitionField field, GameObject gameObject, ComponentTypes type)
    {
        if (gameObject == null)
            return;
        if (!FindEaseTransitons())
            return;

        ETS.TransitionObject tObject = CheckTObject(gameObject);
        StopTransition(tObject);

        easeTransitions.SetField(gameObject, type, field.enumInt, field.end);
    }

    private void SetTransition(TransitionField field, GameObject gameObject, ComponentTypes type, bool startPosition)
    {
        if (gameObject == null)
            return;
        if (!FindEaseTransitons())
            return;

        ETS.TransitionObject tObject = CheckTObject(gameObject);
        StopTransition(tObject);

        SetTransition(gameObject, type, field.enumInt, field.ease, field.direction, field.duration, field.start, field.end, startPosition);
    }

    private string ExportCode(TransitionField field, string name, ComponentTypes type, int indents)
    {
        string code = "";

        code += Write(indents, "tObjects[\"" + name + "\"].SetTransition(" + GetComponentFieldsEnum(type).ToString().Remove(0, 22) + "." + GetFieldName(type, field.enumInt, false) + ", EaseFunctions." + field.ease + ", EaseDirections." + field.direction + "," + field.duration + "f, " + field.start + "f, " + field.end + "f);");

        return code;
    }
    #endregion Fields
    #endregion Data Methods


    private void Update()
    {
        if (hoveringList)
            Repaint();

        if (!transitioning)
            return;
        if (EaseTransitions.tObjects.Count == 0)
            transitioning = false;

        EditorApplication.QueuePlayerLoopUpdate();
    }

    private void OnGUI()
    {
        GUIStyle bg = new GUIStyle("Box") { margin = new RectOffset(-4, -4, -4, -4), padding = new RectOffset(0, 0, 0, 0) };

        #region Null Check
        if (data == null)
        {
            #region Toolbar
            {
                bgColor(11);
                GUILayout.BeginHorizontal();

                #region Groups Count
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(140));

                    GUILayout.Label("Groups", EditorStyles.toolbarButton);

                    int count = EditorGUILayout.DelayedIntField(0, EditorStyles.toolbarTextField, GUILayout.Width(32));
                    if (GUILayout.Button("+", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                        count++;
                    if (GUILayout.Button("-", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                        count--;

                    GUILayout.EndHorizontal();
                }
                #endregion Groups Count

                #region Selected Path
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                    GUIStyle button = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };

                    GUILayout.Button("_ AWAITING DATA FILE _", button, GUILayout.MinWidth(80));

                    GUILayout.EndHorizontal();
                }
                #endregion Selected Path

                GUILayout.EndHorizontal();
                bgColor();
            }
            #endregion Toolbar

            GUILayout.BeginHorizontal();

            #region List
            {
                bgColor(8);
                GUILayout.BeginVertical(bg, GUILayout.Width(140));
                bgColor();
                listScroll = GUILayout.BeginScrollView(listScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
                bgColor();

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            #endregion List

            #region Editor
            {
                bgColor(12);
                GUILayout.BeginHorizontal();
                GUILayout.Space(2);
                GUILayout.BeginVertical(bg);
                bgColor(16);
                editorScroll = GUILayout.BeginScrollView(editorScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
                bgColor();

                GUIStyle toolbarLabel = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };

                #region Data File
                {
                    #region Header
                    {
                        bgColor(9);
                        GUILayout.BeginHorizontal(bg, GUILayout.Height(26));
                        bgColor();

                        data = (EaseTransitionsData)EditorGUILayout.ObjectField(data, typeof(EaseTransitionsData), false, GUILayout.Height(22));
                        if (data != null)
                            Initialize();

                        GUILayout.EndHorizontal();
                    }
                    #endregion Header

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(48);
                }
                #endregion Data File

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(2);
                GUILayout.EndHorizontal();
            }
            #endregion Editor

            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                GUI.FocusControl(null);

            return;
        }

        CheckOutOfRangeHovered();
        CheckOutOfRangeSelected();
        #endregion Null Check

        #region Toolbar
        {
            bgColor(12);
            GUILayout.BeginHorizontal();

            #region Groups Count
            if (data.showList)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(data.listWidth));

                GUILayout.Label("Groups", EditorStyles.toolbarButton);

                int count = EditorGUILayout.DelayedIntField(data.groups.Count, EditorStyles.toolbarTextField, GUILayout.Width(32));
                if (GUILayout.Button("+", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                    count++;
                if (GUILayout.Button("-", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                    count--;

                if (count != data.groups.Count)
                {
                    count = Mathf.Clamp(count, 0, data.groups.Count + 100);

                    while (count > data.groups.Count)
                        Add(data.groups, NewName(data.groups));
                    while (count < data.groups.Count)
                        Remove(data.groups);
                }

                GUILayout.EndHorizontal();
            }
            #endregion Groups Count

            #region Selected Path
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);

                GUIStyle button = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };

                if (GUILayout.Button(data.name, button, GUILayout.MinWidth(80)))
                    SetAddress(selected, -1, -1, true, true);

                if (selected.group != -1)
                {
                    if (GUILayout.Button(data.groups[selected.group].name, button, GUILayout.MinWidth(80)))
                        SetAddress(selected, null, -1, true, true);

                    if (selected.obj != -1)
                        GUILayout.Button(data.groups[selected.group].objects[selected.obj].name, button, GUILayout.MinWidth(80));
                }

                GUILayout.EndHorizontal();
            }
            #endregion Selected Path

            GUILayout.EndHorizontal();
            bgColor();
        }
        #endregion Toolbar

        GUILayout.BeginHorizontal();

        #region List
        if (data.showList)
        {
            GUIStyle textLabel = new GUIStyle(EditorStyles.label) { clipping = TextClipping.Overflow };

            bgColor(6);
            Rect listRect = EditorGUILayout.BeginVertical(bg, GUILayout.Width(data.listWidth));

            if (listRect.Contains(Event.current.mousePosition))
                hoveringList = true;
            else if (listRect.width != 1)
                hoveringList = false;

            bgColor(16);
            listScroll = GUILayout.BeginScrollView(listScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            bgColor();

            #region For Each Group
            for (int g = 0; g < data.groups.Count; g++)
            {
                TransitionGroup group = data.groups[g];

                #region Group
                {
                    GUILayout.Space(4);
                    if (selected.group == g && selected.obj == -1)
                        bgColor(new Color(0.35f, 0.7f, 1.1f));
                    else if (selected.group == g)
                        bgColor(new Color(0.3f, 0.6f, 1f));
                    else
                        bgColor(14);
                    if (hovered.group == g && hovered.obj == -1)
                        GUI.backgroundColor -= Color.white * 0.12f;
                    GUILayout.BeginHorizontal(bg, GUILayout.Width(data.listWidth), GUILayout.Height(24));
                    bgColor();

                    group.expandList = EditorGUILayout.Toggle(group.expandList, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(22));

                    textLabel.fontSize = 12;
                    textLabel.fontStyle = FontStyle.Bold;
                    textLabel.normal.textColor = (selected.group == g) ? Color.white : Color.black;
                    EditorGUILayout.LabelField(group.name, textLabel, GUILayout.Width(1), GUILayout.Height(24));

                    GUILayout.EndHorizontal();
                }
                #endregion Group

                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    SetAddress(hovered, g, -1);
                else if (GUILayoutUtility.GetLastRect().width != 1 && hovered.group == g && hovered.obj == -1)
                    SetAddress(hovered, -1, -1);

                if (group.expandList)
                {
                    if (group.objects.Count != 0)
                    {
                        #region For Each Object
                        for (int o = 0; o < group.objects.Count; o++)
                        {
                            TransitionObject obj = group.objects[o];

                            #region Object
                            {
                                GUILayout.Space(-3);
                                if (selected.group == g && selected.obj == o)
                                    bgColor(new Color(0.35f, 0.7f, 1.1f));
                                else
                                    bgColor(13);
                                if (hovered.group == g && hovered.obj == o)
                                    GUI.backgroundColor -= Color.white * 0.12f;
                                GUILayout.BeginHorizontal("Box", GUILayout.Width(data.listWidth), GUILayout.Height(14));
                                bgColor();

                                textLabel.fontSize = 10;
                                textLabel.fontStyle = FontStyle.Normal;
                                if (obj.gameObject == null)
                                    textLabel.normal.textColor = Color.red;
                                else
                                    textLabel.normal.textColor = (selected.group == g && selected.obj == o) ? Color.white : Color.black;
                                EditorGUILayout.LabelField(obj.name, textLabel, GUILayout.Width(1), GUILayout.Height(14));

                                GUILayout.EndHorizontal();
                            }
                            #endregion Object

                            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                                SetAddress(hovered, g, o);
                            else if (GUILayoutUtility.GetLastRect().width != 1 && hovered.group == g && hovered.obj == o)
                                SetAddress(hovered, -1, -1);
                        }
                        #endregion For Each Object
                    }
                    else
                        GUILayout.Space(16);
                }
            }
            #endregion For Each Group

            GUILayout.Space(48);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        #endregion List

        #region Editor
        {
            bgColor(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginVertical(bg);
            bgColor(16);
            editorScroll = GUILayout.BeginScrollView(editorScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            bgColor();

            GUIStyle toolbarLabel = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };

            if (selected.group == -1)
            {
                #region Data File
                {
                    #region Header
                    {
                        bgColor(9);
                        GUILayout.BeginHorizontal(bg, GUILayout.Height(26));
                        bgColor();

                        data = (EaseTransitionsData)EditorGUILayout.ObjectField(data, typeof(EaseTransitionsData), false, GUILayout.Height(22));
                        if (data == null)
                            return;

                        GUILayout.EndHorizontal();
                    }
                    #endregion Header

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Settings
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            data.showSettings = EditorGUILayout.Toggle(data.showSettings, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Settings", toolbarLabel);

                            if (GUILayout.Button("Defaults", EditorStyles.toolbarButton, GUILayout.Width(78)))
                                DefaultDataSettings();

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Options
                        if (data.showSettings)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor();
                            GUILayout.Space(4);

                            data.showList = EditorGUILayout.Toggle("Show List", data.showList);
                            data.listWidth = Mathf.Clamp(EditorGUILayout.IntField("List Width", data.listWidth), 40, 400);
                            //SetWindowSize(283);

                            GUILayout.Space(4);

                            data.showObjectNames = EditorGUILayout.Toggle("Show Object Names", data.showObjectNames);
                            data.autoSortFields = EditorGUILayout.Toggle("Auto Sort Fields", data.autoSortFields);

                            GUILayout.Space(4);

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(149));
                            if (GUILayout.Button("Find GameObjects"))
                                FindObjectsInScene();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(149));
                            if (GUILayout.Button("Check Object Names"))
                                CheckObjectNames();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(4);

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Transitions : " + EaseTransitions.tObjects.Count, GUILayout.Width(149));
                            if (GUILayout.Button("Stop All Transitions"))
                                StopAllTransitions();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(4);
                            GUILayout.EndVertical();
                        }
                        #endregion Options
                    }
                    #endregion Settings

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Groups
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            data.showGroups = EditorGUILayout.Toggle(data.showGroups, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Groups", toolbarLabel);

                            int count = EditorGUILayout.DelayedIntField(data.groups.Count, EditorStyles.toolbarTextField, GUILayout.Width(32));
                            if (GUILayout.Button("+", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                                count++;
                            if (GUILayout.Button("-", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                                count--;

                            if (count != data.groups.Count)
                            {
                                count = Mathf.Clamp(count, 0, 999);

                                while (count > data.groups.Count)
                                    Add(data.groups, NewName(data.groups));
                                while (count < data.groups.Count)
                                    Remove(data.groups);
                            }

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region List
                        if (data.showGroups)
                        {
                            for (int g = 0; g < data.groups.Count; g++)
                            {
                                TransitionGroup group = data.groups[g];
                                int devisor = 0;
                                if (data.groups.Count % 2 == 0)
                                    devisor = 1;

                                if (g % 2 == devisor)
                                    bgColor(7);
                                else
                                    bgColor(11);
                                GUILayout.BeginHorizontal(bg);
                                bgColor();

                                int order = EditorGUILayout.DelayedIntField(g + 1, EditorStyles.textField, GUILayout.Width(32));
                                if (order != g + 1)
                                    Move(data.groups, group, order - 1, false);

                                string name = Regex.Replace(EditorGUILayout.DelayedTextField(group.name), @"[^a-zA-Z0-9_ ]", "");
                                if (name != group.name)
                                {
                                    if (name == "")
                                        Debug.LogWarning("Group name cannot be empty");
                                    else if (char.IsDigit(name[0]))
                                        Debug.LogWarning("Group name cannot start with a number");
                                    else
                                        ReName(data.groups, group, name);
                                }

                                if (GUILayout.Button("Select", GUILayout.Width(48), GUILayout.Height(18)))
                                    SetAddress(selected, g, null, true, true);

                                GUILayout.EndHorizontal();
                                GUILayout.Space(4);
                            }
                        }
                        #endregion List
                    }
                    #endregion Groups

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Export Code
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            data.showCode = EditorGUILayout.Toggle(data.showCode, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Export Code", toolbarLabel);

                            if (GUILayout.Button("Copy", EditorStyles.toolbarButton, GUILayout.Width(78)))
                                EditorGUIUtility.systemCopyBuffer = ExportCode();

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Code
                        if (data.showCode)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor(16);
                            GUILayout.Space(4);

                            GUILayout.BeginVertical("Box");
                            string code = ExportCode();
                            EditorGUILayout.SelectableLabel(code, new GUIStyle(EditorStyles.label) { fontSize = 9, focused = new GUIStyleState() { textColor = Color.black } }, GUILayout.Height(11 * CountLines(code)));
                            GUILayout.EndVertical();

                            GUILayout.Space(4);
                            bgColor();
                            GUILayout.EndVertical();
                        }
                        #endregion Code
                    }
                    #endregion Export Code

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(48);
                }
                #endregion Data File
            }
            else if (selected.obj == -1)
            {
                #region Group
                {
                    TransitionGroup group = data.groups[selected.group];

                    #region Header
                    {
                        bgColor(9);
                        GUILayout.BeginHorizontal(bg, GUILayout.Height(26));
                        bgColor();

                        int order = EditorGUILayout.DelayedIntField(selected.group + 1, new GUIStyle(EditorStyles.textField) { fontSize = 13 }, GUILayout.Width(32), GUILayout.Height(22));
                        if (order != selected.group + 1)
                            Move(data.groups, group, order - 1);

                        string name = Regex.Replace(EditorGUILayout.DelayedTextField(group.name, new GUIStyle(EditorStyles.textField) { fontSize = 13 }, GUILayout.Height(22)), @"[^a-zA-Z0-9_ ]", "");
                        if (name != group.name)
                        {
                            if (name == "")
                                Debug.LogWarning("Group name cannot be empty");
                            else if (char.IsDigit(name[0]))
                                Debug.LogWarning("Group name cannot start with a number");
                            else
                                ReName(data.groups, group, name);
                        }

                        GUILayout.EndHorizontal();
                    }
                    #endregion Header

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Transition Testing
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            group.showTests = EditorGUILayout.Toggle(group.showTests, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Transition Testing", toolbarLabel);

                            if (GUILayout.Button("Transition", EditorStyles.toolbarButton, GUILayout.Width(78)))
                                SetTransition(group, false);

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Buttons
                        if (group.showTests)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor();
                            GUILayout.Space(4);

                            if (GUILayout.Button("Set to Start"))
                                SetToStart(group);

                            if (GUILayout.Button("Set to End"))
                                SetToEnd(group);

                            GUILayout.Space(4);

                            if (GUILayout.Button("Transition from Start"))
                                SetTransition(group, true);

                            if (GUILayout.Button("Transition from Current"))
                                SetTransition(group, false);

                            GUILayout.Space(4);

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Transitions : " + EaseTransitions.tObjects.Count, GUILayout.Width(149));
                            if (GUILayout.Button("Stop All Transitions"))
                                StopAllTransitions();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(4);
                            GUILayout.EndVertical();
                        }
                        #endregion Buttons
                    }
                    #endregion Transition Testing

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region GameObjects
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            group.showObjs = EditorGUILayout.Toggle(group.showObjs, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("GameObjects", toolbarLabel);

                            int count = EditorGUILayout.DelayedIntField(group.objects.Count, EditorStyles.toolbarTextField, GUILayout.Width(32));
                            if (GUILayout.Button("+", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                                count++;
                            if (GUILayout.Button("-", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                                count--;

                            if (count != group.objects.Count)
                            {
                                count = Mathf.Clamp(count, 0, 999);

                                while (count > group.objects.Count)
                                    Add(group.objects, (GameObject)null);
                                while (count < group.objects.Count)
                                    Remove(group.objects);
                            }

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region List
                        if (group.showObjs)
                        {
                            for (int o = 0; o < group.objects.Count; o++)
                            {
                                TransitionObject obj = group.objects[o];
                                int devisor = 0;
                                if (group.objects.Count % 2 == 0)
                                    devisor = 1;

                                if (o % 2 == devisor)
                                    bgColor(7);
                                else
                                    bgColor(11);
                                GUILayout.BeginHorizontal(bg);
                                bgColor();

                                int order = EditorGUILayout.DelayedIntField(o + 1, EditorStyles.textField, GUILayout.Width(32));
                                if (order != o + 1)
                                    Move(group.objects, obj, order - 1, false);

                                GUILayout.BeginVertical();
                                GameObject gameObject = (GameObject)EditorGUILayout.ObjectField(obj.gameObject, typeof(GameObject), true);
                                if (gameObject != obj.gameObject)
                                    ReObject(obj, gameObject);
                                if (data.showObjectNames)
                                {
                                    string name = EditorGUILayout.DelayedTextField(obj.name);
                                    if (name != obj.name)
                                        ReObject(obj, name);
                                }
                                GUILayout.EndVertical();

                                if (GUILayout.Button("Select", GUILayout.Width(48), GUILayout.Height(18)))
                                    SetAddress(selected, null, o, true, true);

                                GUILayout.EndHorizontal();
                                GUILayout.Space(4);
                            }
                        }
                        #endregion List
                    }
                    #endregion GameObjects

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Export Code
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            group.showCode = EditorGUILayout.Toggle(group.showCode, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Export Code", toolbarLabel);

                            if (GUILayout.Button("Copy", EditorStyles.toolbarButton, GUILayout.Width(78)))
                                EditorGUIUtility.systemCopyBuffer = ExportCode(group);

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Code
                        if (group.showCode)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor(16);
                            GUILayout.Space(4);

                            GUILayout.BeginVertical("Box");
                            string code = ExportCode(group);
                            EditorGUILayout.SelectableLabel(code, new GUIStyle(EditorStyles.label) { fontSize = 9, focused = new GUIStyleState() { textColor = Color.black } }, GUILayout.Height(11 * CountLines(code)));
                            GUILayout.EndVertical();

                            GUILayout.Space(4);
                            bgColor();
                            GUILayout.EndVertical();
                        }
                        #endregion Code
                    }
                    #endregion Export Code

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(48);
                }
                #endregion Group
            }
            else
            {
                #region Object
                {
                    TransitionGroup group = data.groups[selected.group];
                    TransitionObject obj = data.groups[selected.group].objects[selected.obj];

                    #region Header
                    {
                        bgColor(9);
                        GUILayout.BeginHorizontal(bg, GUILayout.Height(26));
                        bgColor();

                        int order = EditorGUILayout.DelayedIntField(selected.obj + 1, new GUIStyle(EditorStyles.textField) { fontSize = 13 }, GUILayout.Width(32), GUILayout.Height(22));
                        if (order != selected.obj + 1)
                            Move(group.objects, obj, order - 1);

                        GUILayout.BeginVertical();
                        GameObject gameObject = (GameObject)EditorGUILayout.ObjectField(obj.gameObject, typeof(GameObject), true, GUILayout.Height(22));
                        if (gameObject != obj.gameObject)
                            ReObject(obj, gameObject);
                        string name = EditorGUILayout.DelayedTextField(obj.name);
                        if (name != obj.name)
                            ReObject(obj, name);
                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();
                    }
                    #endregion Header

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Transition Testing
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            obj.showTests = EditorGUILayout.Toggle(obj.showTests, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Transition Testing", toolbarLabel);

                            if (GUILayout.Button("Transition", EditorStyles.toolbarButton, GUILayout.Width(78)))
                                SetTransition(obj, false);

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Buttons
                        if (obj.showTests)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor();
                            GUILayout.Space(4);

                            if (GUILayout.Button("Set to Start"))
                                SetToStart(obj);

                            if (GUILayout.Button("Set to End"))
                                SetToEnd(obj);

                            GUILayout.Space(4);

                            if (GUILayout.Button("Transition from Start"))
                                SetTransition(obj, true);

                            if (GUILayout.Button("Transition from Current"))
                                SetTransition(obj, false);

                            GUILayout.Space(4);

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Transitions : " + EaseTransitions.tObjects.Count, GUILayout.Width(149));
                            if (GUILayout.Button("Stop All Transitions"))
                                StopAllTransitions();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(4);
                            GUILayout.EndVertical();
                        }
                        #endregion Buttons
                    }
                    #endregion Transition Testing

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Import Values
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            obj.showImports = EditorGUILayout.Toggle(obj.showImports, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Import Values", toolbarLabel);

                            GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(78));

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Buttons
                        if (obj.showImports)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor();
                            GUILayout.Space(4);

                            if (GUILayout.Button("Set Start to Current Values"))
                                ImportToStart(obj);

                            if (GUILayout.Button("Set End to Current Values"))
                                ImportToEnd(obj);

                            GUILayout.Space(4);
                            GUILayout.EndVertical();
                        }
                        #endregion Buttons
                    }
                    #endregion Import Values

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region Ease
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            obj.showEase = EditorGUILayout.Toggle(obj.showEase, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Ease", toolbarLabel);

                            GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(78));

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Buttons
                        if (obj.showEase)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor();
                            GUILayout.Space(4);


                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Singular Ease", GUILayout.Width(124), GUILayout.Height(16));
                            obj.singleEase = EditorGUILayout.Toggle(obj.singleEase);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Ease Function", GUILayout.Width(124), GUILayout.Height(16));
                            obj.ease = (EaseFunctions)EditorGUILayout.EnumPopup(obj.ease, GUILayout.MinWidth(84));
                            obj.direction = (EaseDirection)EditorGUILayout.EnumPopup(obj.direction, GUILayout.MinWidth(52));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Duration (Seconds)", GUILayout.Width(124), GUILayout.Height(16));
                            obj.duration = EditorGUILayout.FloatField(obj.duration);
                            GUILayout.EndHorizontal();

                            GUILayout.Space(4);
                            GUILayout.EndVertical();
                        }
                        #endregion Buttons
                    }
                    #endregion Ease

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(2);

                    #region ForEach Components
                    if (obj.gameObject != null)
                    {
                        foreach (ComponentTypes type in Enum.GetValues(typeof(ComponentTypes)))
                            FindComponent(obj.components, obj.gameObject, type);

                        for (int c = 0; c < obj.components.Count; c++)
                        {
                            TransitionComponent component = obj.components[c];

                            #region Component
                            {
                                #region Toolbar
                                {
                                    bgColor(12);
                                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                                    GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                                    component.showFields = EditorGUILayout.Toggle(component.showFields, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                                    GUILayout.Label(component.type.ToString(), toolbarLabel);

                                    int count = EditorGUILayout.DelayedIntField(component.fields.Count, EditorStyles.toolbarTextField, GUILayout.Width(32));
                                    if (GUILayout.Button("+", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                                        count++;
                                    if (GUILayout.Button("-", new GUIStyle("ToolbarButton") { padding = new RectOffset(1, 0, 0, 0) }, GUILayout.Width(20)))
                                        count--;

                                    if (count != component.fields.Count)
                                    {
                                        count = Mathf.Clamp(count, 0, GetFieldNames(component.type).Length);
                                        while (count > component.fields.Count)
                                            Add(component.fields, NewEnumInt(component.fields));
                                        while (count < component.fields.Count)
                                            Remove(component.fields);
                                    }

                                    bgColor();
                                    GUILayout.EndHorizontal();
                                }
                                #endregion Toolbar

                                #region Fields List
                                if (component.showFields)
                                {
                                    if (data.autoSortFields)
                                        SortFields(component);

                                    for (int f = 0; f < component.fields.Count; f++)
                                    {
                                        TransitionField field = component.fields[f];
                                        int devisor = 0;
                                        if (component.fields.Count % 2 == 0)
                                            devisor = 1;

                                        if (f % 2 == devisor)
                                            bgColor(7);
                                        else
                                            bgColor(11);
                                        GUILayout.BeginVertical(bg, GUILayout.Height(23));
                                        bgColor();
                                        GUILayout.Space(4);

                                        GUILayout.BeginHorizontal();
                                        if (!data.autoSortFields)
                                        {
                                            int order = EditorGUILayout.DelayedIntField(f + 1, EditorStyles.textField, GUILayout.Width(32));
                                            if (order != f + 1)
                                                Move(component.fields, field, order - 1);
                                        }
                                        int enumInt = EditorGUILayout.Popup(field.enumInt, GetFieldNames(component.type));
                                        if (enumInt != field.enumInt)
                                            ReField(component.fields, field, component.type, enumInt);
                                        if (GUILayout.Button("Delete", GUILayout.Width(48), GUILayout.Height(18)))
                                            Remove(component.fields, field);
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Start:");
                                        field.start = EditorGUILayout.FloatField(field.start);
                                        GUILayout.Space(4);
                                        GUILayout.Label("End:");
                                        field.end = EditorGUILayout.FloatField(field.end);
                                        GUILayout.EndHorizontal();

                                        if (!obj.singleEase)
                                        {
                                            GUILayout.BeginHorizontal();
                                            field.ease = (EaseFunctions)EditorGUILayout.EnumPopup(field.ease, GUILayout.Width(84));
                                            field.direction = (EaseDirection)EditorGUILayout.EnumPopup(field.direction, GUILayout.Width(52));
                                            field.duration = EditorGUILayout.FloatField(field.duration);
                                            GUILayout.Space(-4);
                                            GUILayout.Label("Seconds", GUILayout.Width(52));
                                            GUILayout.EndHorizontal();
                                        }
                                        else
                                        {
                                            field.ease = obj.ease;
                                            field.direction = obj.direction;
                                            field.duration = obj.duration;
                                        }

                                        GUILayout.Space(4);
                                        GUILayout.EndVertical();
                                    }
                                }
                                #endregion Fields List
                            }
                            #endregion Component

                            GUILayout.Space(2);
                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                            GUILayout.Space(2);
                        }
                    }
                    #endregion ForEach Components

                    #region Export Code
                    {
                        #region Toolbar
                        {
                            bgColor(12);
                            GUILayout.BeginHorizontal(EditorStyles.toolbar);

                            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(1));
                            obj.showCode = EditorGUILayout.Toggle(obj.showCode, EditorStyles.foldout, GUILayout.Width(16), GUILayout.Height(16));

                            GUILayout.Label("Export Code", toolbarLabel);

                            if (GUILayout.Button("Copy", EditorStyles.toolbarButton, GUILayout.Width(78)))
                                EditorGUIUtility.systemCopyBuffer = ExportCode(obj);

                            bgColor();
                            GUILayout.EndHorizontal();
                        }
                        #endregion Toolbar

                        #region Code
                        if (obj.showCode)
                        {
                            bgColor(9);
                            GUILayout.BeginVertical(bg);
                            bgColor(16);
                            GUILayout.Space(4);

                            GUILayout.BeginVertical("Box");
                            string code = ExportCode(obj);
                            EditorGUILayout.SelectableLabel(code, new GUIStyle(EditorStyles.label) { fontSize = 9, focused = new GUIStyleState() { textColor = Color.black } }, GUILayout.Height(11 * CountLines(code)));
                            GUILayout.EndVertical();

                            GUILayout.Space(4);
                            bgColor();
                            GUILayout.EndVertical();
                        }
                        #endregion Code
                    }
                    #endregion Export Code

                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0));
                    GUILayout.Space(48);
                }
                #endregion Object
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion Editor

        GUILayout.EndHorizontal();

        Interaction();
        Undo.RecordObject(data, "Transition Data Changed");
        EditorUtility.SetDirty(data);
    }
}