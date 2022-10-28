using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlocObject : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool OnIt;

    public GameObject obj;

    public StatsCanon stats; 
    public SelectedType Type;

    InputManager inputs;
    public PowerUpSystem powerUpSystem;
    public Image Fond;

    private void Start()
    {
        inputs = InputManager.Instance;
        powerUpSystem = GetComponentInParent<PowerUpSystem>();
    }

    #region Selecter

    public void OnSelection()
    {
        if (powerUpSystem.SelectedObject != null && powerUpSystem.SelectedObject != gameObject)
        {
            while (gameObject.transform.localScale != Vector3.one * 1.125f && Fond.color != new Color(0.5f, 1f, 1f, 0.1f))
            {
                gameObject.transform.localScale = Vector3.Slerp(gameObject.transform.localScale, Vector3.one * 1.125f, 0.1f);
                Fond.color = Color.Lerp(Fond.color, new Color(0.5f, 1f, 1f, 0.1f), 0.1f);
            }
        }
    }

    #endregion

    #region Selected

    public void OnSelected()
    {
        if (powerUpSystem.SelectedObject != null && powerUpSystem.SelectedObject == gameObject)
        {
            while (gameObject.transform.localScale != Vector3.one * 1.25f && Fond.color != new Color(0.5f, 1f, 1f, 0.35f))
            {
                gameObject.transform.localScale = Vector3.Slerp(gameObject.transform.localScale, Vector3.one * 1.25f, 0.1f);
                Fond.color = Color.Lerp(Fond.color, new Color(0.5f, 1f, 1f, 0.35f), 0.1f);
            }
        }
        powerUpSystem.selectedType = Type;
    }

    public void OnNotSelected()
    {
        if (powerUpSystem.SelectedObject != null && powerUpSystem.SelectedObject != gameObject)
        {
            while (gameObject.transform.localScale != Vector3.one && Fond.color != new Color(0.5f, 1f, 1f, 0f))
            {
                gameObject.transform.localScale = Vector3.Slerp(gameObject.transform.localScale, Vector3.one, 0.1f);
                Fond.color = Color.Lerp(Fond.color, new Color(0.5f, 1f, 1f, 0f), 0.1f);
            }
        }
    }

    #endregion

    public void OnSelect(BaseEventData eventData)
    {
        if (powerUpSystem.SelectedObject != gameObject)
            OnSelection();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnNotSelected();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (powerUpSystem.SelectedObject != gameObject)
            OnSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnNotSelected();
    }

    public void Press()
    {
        powerUpSystem.SelectedObject = gameObject;
        OnSelected();
    }
}
