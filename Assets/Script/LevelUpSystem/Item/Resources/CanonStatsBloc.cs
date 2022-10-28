using System;
using UnityEngine;

[Serializable]
public class CanonStatsBloc : BlocStats
{
    [Header("Stats for Canon (Additional)")]
    public float shootRate;
    public int damage;
    public float range;
}
