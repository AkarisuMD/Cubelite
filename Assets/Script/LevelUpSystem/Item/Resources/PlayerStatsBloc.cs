using System;
using UnityEngine;

[Serializable]
public class PlayerStatsBloc : BlocStats
{
    [Header("Stats for Player (multiplicator)")]
    public float hp;
    public float size;
    public float speed;
    public float shootRate;
    public float damage;
    public float range;
}
