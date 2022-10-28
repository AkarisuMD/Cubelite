using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlocPowerUp", menuName = "ScriptableObjects/BlocPowerUp", order = 11)]
public class ScBlocStatsUp : ScBloc
{
    public CanonStatsBloc Canon;
    public PlayerStatsBloc Player;
}
