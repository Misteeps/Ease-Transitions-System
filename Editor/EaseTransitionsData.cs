using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Ease Transition Data", menuName = "Ease Transition Data", order = 390)]
public class EaseTransitionsData : ScriptableObject
{
    public List<TransitionGroup> groups = new List<TransitionGroup>();

    public bool showSettings = true;
    public bool showGroups = true;
    public bool showCode = true;

    public bool showList = true;
    public int listWidth = 160;

    public bool showObjectNames = true;
    public bool autoSortFields = true;

    public ListAddress selected = new ListAddress();
}