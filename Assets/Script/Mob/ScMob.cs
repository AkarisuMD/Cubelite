using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mob", menuName = "ScriptableObjects/Mob", order = 3)]
public class ScMob : BaseScriptable
{
    public GameObject prefab;


    [Space(10)]
    [Header("MOB")]
    public Sprite sprite;
    public Object agent;
    public int HP;
    public float Speed;
    public AnimationCurve SpeedCurve;
    public float ShootRate;
    public float Damage;
    public float Range;
    public float RangeDetection;
    public float RangeDetectionShoot;
    public int XPGiving;


    [Space(10)]
    [Header("CANON")]
    public ScCanon[] Canon;
    [NonReorderable] public List<PosCanon> PositionAndRotationOfCanons;
}
