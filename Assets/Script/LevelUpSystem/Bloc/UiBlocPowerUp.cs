using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum TypeOfBloc
{
    Canon,
    PowerUp
}

public class UiBlocPowerUp : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PowerUpSystem powerUpSystem;
    public Button button;

    public Image fond, icon;
    public TMP_Text _name, info;
    public GameObject statsbloc, bloc;

    public ScBloc[] scBlocs;
    public ScBloc scBloc;
    public BlocStats blocStats;

    void Start()
    {
        powerUpSystem = GetComponentInParent<PowerUpSystem>();
        button = GetComponent<Button>();

        StartCoroutine(GenerateScriptableBloc());
    }

    void OnSelection()
    {
        while (gameObject.transform.localScale != Vector3.one * 1.075f)
        {
            gameObject.transform.localScale = Vector3.Slerp(gameObject.transform.localScale, Vector3.one * 1.075f, 0.1f);
        }
    }

    void OnSelected()
    {
        while (gameObject.transform.localScale != Vector3.one * 1.15f)
        {
            gameObject.transform.localScale = Vector3.Slerp(gameObject.transform.localScale, Vector3.one * 1.15f, 0.1f);
        }

        powerUpSystem.IsPressed(scBloc);
    }

    public void OnNotSelected()
    {
        if (powerUpSystem.SelectedBloc != this)
        {
            while (gameObject.transform.localScale != Vector3.one)
            {
                gameObject.transform.localScale = Vector3.Slerp(gameObject.transform.localScale, Vector3.one, 0.1f);
            }
        }
    }


    IEnumerator GenerateScriptableBloc()
    {
        scBlocs = Resources.LoadAll<ScBloc>("") as ScBloc[];

        while (scBlocs == null)
            yield return new WaitForSeconds(0.05f);

        int rng = UnityEngine.Random.Range(0, scBlocs.Length);
        scBloc = scBlocs[rng];

        if (PlayerData.Instance.Canons.Count >= PlayerData.Instance.MaxCanon)
            while (scBloc.GetType() == typeof(ScBlocCanon))
            {
                rng = UnityEngine.Random.Range(0, scBlocs.Length);
                scBloc = scBlocs[rng];
            }

        SetBloc(scBloc);

        Type type = scBloc.GetType();

        if (type == typeof(ScBlocCanon))
            SetScriptableBlocCanon();
        else if (type == typeof(ScBlocStatsUp))
            SetScriptableBlocPowerUp();
    }
    void SetBloc(ScBloc _scBloc)
    {
        fond.sprite = _scBloc.Fond;
        icon.sprite = _scBloc.Icon;
        _name.text = _scBloc.Name;
        info.text = _scBloc.Info;
    }
    void SetScriptableBlocCanon()
    {
        ScBlocCanon scriptableBlocCanon = scBloc as ScBlocCanon;
    }
    public void SetScriptableBlocPowerUp()
    {
        ScBlocStatsUp scriptableBlocPowerUp = scBloc as ScBlocStatsUp;

        switch (powerUpSystem.selectedType)
        {
            case SelectedType.Player:
                SetBlocPowerUpStatPlayer(scriptableBlocPowerUp);
                break;
            case SelectedType.Canon:
                SetBlocPowerUpStatCanon(scriptableBlocPowerUp);
                break;
            default:
                break;
        }
    }

    #region SetBlocStat

    void SetBlocPowerUpStatPlayer(ScBlocStatsUp scriptableBlocPowerUp)
    {
        if (bloc != null)
            Destroy(bloc);

        GameObject _bloc = Resources.Load<GameObject>("PowerUpSystem/ForPlayer");
        bloc = Instantiate(_bloc, statsbloc.transform.position, statsbloc.transform.rotation, statsbloc.transform);

        PlayerBlocStats pbs = bloc.GetComponent<PlayerBlocStats>();

        pbs.fond.sprite = scriptableBlocPowerUp.FondStats;

        Color color = GetColorClassic(scriptableBlocPowerUp.Player.hp - 1);
        Sprite sprite = GetSpriteClassic(scriptableBlocPowerUp.Player.hp - 1);
        pbs.hpText.text = "x" + scriptableBlocPowerUp.Player.hp.ToString() + " Health";
        pbs.hpText.color = color;
        pbs.hpIcon.sprite = sprite;
        pbs.hpIcon.color = color;

        color = GetColorReverse(scriptableBlocPowerUp.Player.size - 1);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Player.size - 1);
        pbs.sizeText.text = "x" + scriptableBlocPowerUp.Player.size.ToString() + " Size";
        pbs.sizeText.color = color;
        pbs.sizeIcon.sprite = sprite;
        pbs.sizeIcon.color = color;

        color = GetColorClassic(scriptableBlocPowerUp.Player.speed - 1);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Player.speed - 1);
        pbs.speedText.text = "x" + scriptableBlocPowerUp.Player.speed.ToString() + " Speed";
        pbs.speedText.color = color;
        pbs.speedIcon.sprite = sprite;
        pbs.speedIcon.color = color;

        color = GetColorReverse(scriptableBlocPowerUp.Player.shootRate - 1);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Player.shootRate - 1);
        pbs.shootrateText.text = "x" + scriptableBlocPowerUp.Player.shootRate.ToString() + " Shoot rate";
        pbs.shootrateText.color = color;
        pbs.shootrateIcon.sprite = sprite;
        pbs.shootrateIcon.color = color;

        color = GetColorClassic(scriptableBlocPowerUp.Player.damage - 1);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Player.damage - 1);
        pbs.damageText.text = "x" + scriptableBlocPowerUp.Player.damage.ToString() + " Damage";
        pbs.damageText.color = color;
        pbs.damageIcon.sprite = sprite;
        pbs.damageIcon.color = color;

        color = GetColorClassic(scriptableBlocPowerUp.Player.range - 1);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Player.range - 1);
        pbs.rangeText.text = "x" + scriptableBlocPowerUp.Player.range.ToString() + " Range";
        pbs.rangeText.color = color;
        pbs.rangeIcon.sprite = sprite;
        pbs.rangeIcon.color = color;
    }

    void SetBlocPowerUpStatCanon(ScBlocStatsUp scriptableBlocPowerUp)
    {
        if (bloc != null)
            Destroy(bloc);

        GameObject _bloc = Resources.Load<GameObject>("PowerUpSystem/ForCanon");
        bloc = Instantiate(_bloc, statsbloc.transform.position, statsbloc.transform.rotation, statsbloc.transform);

        CanonBlocStats cbs = bloc.GetComponent<CanonBlocStats>();

        cbs.fond.sprite = scriptableBlocPowerUp.FondStats;

        Color color = GetColorReverse(scriptableBlocPowerUp.Canon.shootRate);
        Sprite sprite = GetSpriteReverse(scriptableBlocPowerUp.Canon.shootRate);
        cbs.shootrateText.text = scriptableBlocPowerUp.Canon.shootRate.ToString() + " Shoot rate";
        cbs.shootrateText.color = color;
        cbs.shootrateIcon.sprite = sprite;
        cbs.shootrateIcon.color = color;

        color = GetColorClassic(scriptableBlocPowerUp.Canon.damage);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Canon.damage);
        cbs.damageText.text = scriptableBlocPowerUp.Canon.damage.ToString() + " Damage";
        cbs.damageText.color = color;
        cbs.damageIcon.sprite = sprite;
        cbs.damageIcon.color = color;

        color = GetColorClassic(scriptableBlocPowerUp.Canon.range);
        sprite = GetSpriteClassic(scriptableBlocPowerUp.Canon.range);
        cbs.rangeText.text = scriptableBlocPowerUp.Canon.range.ToString() + " Range";
        cbs.rangeText.color = color;
        cbs.rangeIcon.sprite = sprite;
        cbs.rangeIcon.color = color;
    }

    #endregion

    void GetBlocTypeScript(BlocStats blocStats)
    {
        Type type = blocStats.GetType();

        if (type == typeof(PlayerStatsBloc))
            PlayerStatsBlocScript();
        else if (type == typeof(CanonStatsBloc))
            CanonStatsBlocScript();
    }

    void PlayerStatsBlocScript()
    {

    }

    void CanonStatsBlocScript()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        if (powerUpSystem.SelectedBloc != this)
            OnSelection();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnNotSelected();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (powerUpSystem.SelectedBloc != this)
            OnSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnNotSelected();
    }

    public void Press()
    {
        powerUpSystem.SelectedBloc = this;
        OnSelected();
    }

    Color GetColorClassic(float nb)
    {
        switch (nb)
        {
            case < 0:
                return Color.red;
            case 0:
                return Color.black;
            case > 0:
                return Color.green;
            default:
                return Color.black;
        }
    }
    Color GetColorReverse(float nb)
    {
        switch (nb)
        {
            case < 0:
                return Color.green;
            case 0:
                return Color.black;
            case > 0:
                return Color.red;
            default:
                return Color.black;
        }
    }
    Sprite GetSpriteClassic(float nb)
    {
        switch (nb)
        {
            case < 0:
                return Resources.Load<Sprite>("Icon/Negatif");
            case 0:
                return Resources.Load<Sprite>("Icon/Neutre");
            case > 0:
                return Resources.Load<Sprite>("Icon/Positif");
            default:
                return Resources.Load<Sprite>("Icon/Neutre");
        }
    }
    Sprite GetSpriteReverse(float nb)
    {
        switch (nb)
        {
            case < 0:
                return Resources.Load<Sprite>("Icon/Positif");
            case 0:
                return Resources.Load<Sprite>("Icon/Neutre");
            case > 0:
                return Resources.Load<Sprite>("Icon/Negatif");
            default:
                return Resources.Load<Sprite>("Icon/Neutre");
        }
    }

}
