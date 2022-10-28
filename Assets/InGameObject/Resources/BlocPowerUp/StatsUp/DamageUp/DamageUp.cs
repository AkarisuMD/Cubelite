using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUp : SelectedBloc
{
    public override void OnSelectedForPlayer(ScBloc scriptableBloc)
    {
        ScBlocStatsUp sb = scriptableBloc as ScBlocStatsUp;
        PlayerData.Instance.Damage *= sb.Player.damage;
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
        canon.GetComponent<CanonBehaviour>().stats.damage += sb.Canon.damage;
        canon.GetComponent<CanonBehaviour>().Updatehit();
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(this);
    }
}
