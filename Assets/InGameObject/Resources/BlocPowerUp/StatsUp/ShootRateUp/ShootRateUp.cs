using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRateUp : SelectedBloc
{
    public override void OnSelectedForPlayer(ScBloc scriptableBloc)
    {
        ScBlocStatsUp sb = scriptableBloc as ScBlocStatsUp;
        PlayerData.Instance.ShootRate *= sb.Player.shootRate;
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
        canon.GetComponent<CanonBehaviour>().stats.shootSpeed += sb.Canon.shootRate;
        Mathf.Clamp(canon.GetComponent<CanonBehaviour>().stats.shootSpeed, 0.1f, 10f);
        canon.GetComponent<CanonBehaviour>().Updatehit();
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(this);
    }
}
