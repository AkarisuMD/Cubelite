using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : SingletonND<PlayerData>
{
    public GameObject player;
    public GameObject CanonHorlder;
    public List<GameObject> CanonsRotation;
    public List<GameObject> Canons;
    public List<CanonBehaviour> CanonsScripts;
    public List<List<Vector3>> CanonsPosition;
    public List<List<Quaternion>> CanonsPosRotation;
    public bool CanChangeGravity;

    public GroundType Ground
    {
        get { return ground; }
        set { 
            if (CanChangeGravity) ground = value;
        }
    }
            [SerializeField] private GroundType ground;
    public GravityChangeType gravityChangeType;

    #region Stats

    public int HP { get { return _HP; } set { _HP = value; } }
    [SerializeField] private int _HP;
    public float Speed { get { return _Speed; } set { _Speed = value; } }
    [SerializeField] private float _Speed;
    public float JumpForce { get { return _JumpForce; } set { _JumpForce = value; } }
    [SerializeField] private float _JumpForce; 
    public float ShootRate { get { return _ShootRate; } set { _ShootRate = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _ShootRate; 
    public float Damage { get { return _Damage; } set { _Damage = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _Damage;
    public float Range { get { return _Range; } set { _Range = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _Range;
    public float Critical { get { return _Critical; } set { _Critical = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _Critical;

    public int MaxCanon { get { return _MaxCanon; } set { _MaxCanon = value; } }
    [SerializeField] private int _MaxCanon;

    #endregion

    #region Level

    public int Level { get { return _Level; } set { _Level = value; } }
    [SerializeField] private int _Level;
    public int XP { get { return _XP; } set { _XP = value; } }
    [SerializeField] private int _XP;

    public List<int> XpForLevelUp;

    #endregion
}
public enum GroundType
{
    Down,
    Right,
    Left,
    Up
}
public enum GravityChangeType
{
    Collision,
    Bloc,
    Zone
}
