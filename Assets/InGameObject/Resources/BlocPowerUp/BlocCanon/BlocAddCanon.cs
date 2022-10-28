using UnityEngine;

public class BlocAddCanon : SelectedBloc
{
    public override void OnSelectedForPlayer(ScBloc scBloc)
    {
        AddCanon(scBloc);
    }

    public override void OnSelectedForTail(ScBloc scBloc)
    {
        AddCanon(scBloc);
    }

    public override void OnSelectedForCanon(ScBloc scBloc, GameObject canon)
    {
        AddCanon(scBloc);
    }

    void AddCanon(ScBloc scBloc)
    {
        ScBlocCanon sbc = scBloc as ScBlocCanon;
        PlayerData.Instance.player.GetComponent<PlayerBehaviour>().AddCanon(sbc.Canon);
        Destroy(this); 
    }
}
