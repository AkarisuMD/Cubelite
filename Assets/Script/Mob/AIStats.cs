using System.Collections.Generic;
using UnityEngine;

public class AIStats : MonoBehaviour
{

    [Header("PREFAB")]
    [SerializeField] public SpriteRenderer spriteRenderer;


    [Space(10)]
    [Header("STATS")]
    [SerializeField] private int _HP;
    public int HP { get { return _HP; } set { _HP = value; } }
    public float Speed { get { return _Speed; } set { _Speed = value; } }
    [SerializeField] private float _Speed;
    public void ChangeSpeed(float speed) { Speed += speed; }

    public float ShootRate { get { return _ShootRate; } set { _ShootRate = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _ShootRate;
    public void ChangeShootRate(float shootRate) { ShootRate *= shootRate; }

    public float RangeDetection { get { return _RangeDetection; } set { _RangeDetection = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _RangeDetection;

    public float RangeDetectionShoot { get { return _RangeDetectionShoot; } set { _RangeDetectionShoot = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _RangeDetectionShoot;

    public float Damage { get { return _Damage; } set { _Damage = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _Damage;
    public void ChangeDamage(float damage) { Damage *= damage; UpdateStats(); }

    public float Range { get { return _Range; } set { _Range = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _Range;
    public void ChangeRange(float range) { Range *= range; UpdateStats(); }

    public float Critical { get { return _Critical; } set { _Critical = value; } }
    [SerializeField][Range(0.5f, 2f)] private float _Critical;
    public void ChangeCritical(float critical) { Critical *= critical; UpdateStats(); }

    public GameObject CanonHorlder;
    public List<GameObject> CanonsRotation;
    public List<GameObject> Canons;
    public List<CanonBehaviour> CanonsScripts;
    public List<List<Vector3>> CanonsPosition;
    public List<List<Quaternion>> CanonsPosRotation;

    void UpdateStats()
    {
        CanonBehaviour[] x = gameObject.GetComponentsInChildren<CanonBehaviour>();
        foreach (var item in x)
        {
            item.Updatehit();
        }
    }


    public int XP { get { return _XP; } set { _XP = value; } }
    [SerializeField] private int _XP;
}
