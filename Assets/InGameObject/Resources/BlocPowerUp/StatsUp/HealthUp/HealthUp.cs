using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : SelectedBloc
{
    public override void OnSelectedForPlayer(ScBloc scriptableBloc)
    {
        ScBlocStatsUp sb = scriptableBloc as ScBlocStatsUp;
        PlayerData.Instance.HP = Mathf.CeilToInt(PlayerData.Instance.HP * sb.Player.hp);
        DestroySelf();
    }

    public override void OnSelectedForTail(ScBloc scriptableBloc)
    {
        ScBlocStatsUp sb = scriptableBloc as ScBlocStatsUp;

        DestroySelf();
    }

    public override void OnSelectedForCanon(ScBloc scriptableBloc, GameObject canon)
    {
        ScBlocStatsUp sb = scriptableBloc as ScBlocStatsUp;

        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(this);
    }
}
