using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map", order = 21)]
public class ScMap : BaseScriptable
{
    public GameObject MapObj;
    public MapType type;
    public List<Vector2> point;
}