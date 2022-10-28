using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Player", order = 1)]
public class ScPlayer : BaseScriptable
{
    public GameObject prefab;

    [Space (10)] [Header ("PLAYER")]
    public Sprite sprite;
    public int HP;
    public float Speed;
    public float JumpForce;
    public float ShootRate;
    public float Damage;
    public float Range;
    public float Critical;
    public List<int> XPforLevel;



    [Space(10)] [Header("CANON")]
    public ScCanon CanonDeDepart;
    public int MaxCanonPossible;
    [NonReorderable] public List<PosCanon> PositionAndRotationOfCanons;
}

[System.Serializable]
public struct PosCanon
{
    public List<Vector3> Position;
    public List<Quaternion> Rotation;
}
