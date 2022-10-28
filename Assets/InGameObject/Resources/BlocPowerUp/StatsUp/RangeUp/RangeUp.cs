using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUp : SelectedBloc
{
    public override void OnSelectedForPlayer(ScBloc scriptableBloc)
    {
        ScBlocStatsUp sb = scriptableBloc as ScBlocStatsUp;
        PlayerData.Instance.Range *= sb.Player.range;
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
        canon.GetComponent<CanonBehaviour>().stats.range += sb.Canon.range;
        canon.GetComponent<CanonBehaviour>().Updatehit();
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(this);
    }
}
