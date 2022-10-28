using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Bloque : TMP_Dropdown
{
    Menu m;

    public override void OnSubmit(BaseEventData eventData)
    {
        m = GetComponentInParent<Menu>();
        Debug.Log("Submitted!");
        m.back = true;
    }
}