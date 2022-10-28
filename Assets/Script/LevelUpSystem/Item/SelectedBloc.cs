using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectedBloc : MonoBehaviour
{
    public virtual void OnSelectedForPlayer(ScBloc scBloc) { }
    public virtual void OnSelectedForTail(ScBloc scBloc) { }
    public virtual void OnSelectedForCanon(ScBloc scBloc, GameObject canon) { }
}
