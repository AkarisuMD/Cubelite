using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "ScriptableObjects/World", order = 20)]
public class ScWorld : BaseScriptable
{
    public string _name;
    public int MaxNbMap;
    public float MaxMapSize;

    public List<ScMap> Spawns;
    public List<ScMap> ScMaps;
}
