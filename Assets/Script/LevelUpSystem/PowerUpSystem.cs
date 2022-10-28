using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectedType
{
    Player,
    Canon
}
public class PowerUpSystem : Singleton<PowerUpSystem>
{
    public GameObject panel;
    public bool Actif;
    
    public SelectedType selectedType;
    public SelectedType actualType;

    public ScrollRect ScrollRectPlayerObject;
    public GameObject contentPlayerObject;

    public GameObject contentBloc;

    public int nbLoot = 3;
    public GameObject SelectedObject 
    
    
    {
        get { return selectedObject; }
        set 
        {
            selectedObject = value;
            foreach (BlocObject obj in contentPlayerObject.GetComponentsInChildren<BlocObject>())
            {
                obj.OnNotSelected();
            }
        } 
    }
    [SerializeField] private GameObject selectedObject;

    public UiBlocPowerUp SelectedBloc
    {
        get { return selectedBloc; }
        set
        {
            selectedBloc = value;
            foreach (UiBlocPowerUp obj in UiBloc)
            {
                obj.OnNotSelected();
            }
        }
    }
    [SerializeField] private UiBlocPowerUp selectedBloc;

    public List<UiBlocPowerUp> UiBloc;

    PlayerData playerData;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void ActiveSystem()
    {
        Actif = true;
        panel.SetActive(true);
        

        Time.timeScale = 0.0000001f;
        playerData = PlayerData.Instance;

        SetLoot();
        SetScrollRectForObject();

    }

    void SetLoot()
    {
        if (UiBloc != null && UiBloc.Count > 0)
        {
            foreach (var item in contentBloc.GetComponentsInChildren<Transform>())
            {
                if (item != contentBloc.transform)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        UiBloc = new List<UiBlocPowerUp>();

        GameObject PrefabBloc = Resources.Load<GameObject>("PowerUpSystem/Loot");
        for (int i = 0; i < nbLoot; i++)
        {
            GameObject Bloc = Instantiate(PrefabBloc, contentBloc.transform);
            UiBloc.Add(Bloc.GetComponent<UiBlocPowerUp>());
        }
    }

    void SetScrollRectForObject()
    {
        ScrollRectPlayerObject = GetComponentInChildren<ScrollRect>();

        Transform[] transforms = contentPlayerObject.GetComponentsInChildren<Transform>();
        foreach (var t in transforms)
        {
            if (t != contentPlayerObject.transform)
            {
                Destroy(t.gameObject);
            }
        }

        GameObject PrefabObjectScrollRect = Resources.Load<GameObject>("PowerUpSystem/PrefabObjectScrollRect");
        while (PrefabObjectScrollRect == null) return;

        GameObject player = Instantiate(PrefabObjectScrollRect, contentPlayerObject.transform);
        player.GetComponent<Image>().sprite = playerData.player.GetComponentInChildren<SpriteRenderer>().sprite;
        player.GetComponent<Image>().color = playerData.player.GetComponentInChildren<SpriteRenderer>().color;
        BlocObject bop = player.GetComponent<BlocObject>();
        bop.obj = playerData.player;
        bop.Type = SelectedType.Player;

        foreach (var canon in playerData.Canons)
        {
            GameObject _canon = Instantiate(PrefabObjectScrollRect, contentPlayerObject.transform);
            _canon.GetComponent<Image>().sprite = canon.GetComponent<SpriteRenderer>().sprite;
            _canon.GetComponent<Image>().color = canon.GetComponent<SpriteRenderer>().color;
            BlocObject boc = _canon.GetComponent<BlocObject>();
            boc.stats = canon.GetComponent<CanonBehaviour>().stats;
            boc.obj = canon;
            boc.Type = SelectedType.Canon;
        }

        selectedObject = player;
        bop.powerUpSystem = this;
        bop.OnSelected();
    }

    private void Update()
    {
        if (Actif && selectedType != actualType)
        {
            foreach (var item in UiBloc)
            {
                if (item.scBloc.GetType() == typeof(ScBlocStatsUp))
                {
                    item.SetScriptableBlocPowerUp();
                }
            }
            actualType = selectedType;
        }
    }

    public void IsPressed(ScBloc scBloc)
    {
        Camera.main.GetComponent<AudioSource>().Play();
        SetScriptableBloc(scBloc);
    }

    #region Bloc

    private void SetScriptableBloc(ScBloc scBloc)
    {
        SelectedBloc sb = gameObject.AddComponent(Type.GetType(scBloc.ScriptBloc.name)) as SelectedBloc;
        switch (selectedType)
        {
            case SelectedType.Player:
                sb.OnSelectedForPlayer(scBloc);
                break;
            case SelectedType.Canon:
                GameObject go = selectedObject.GetComponent<BlocObject>().obj;
                sb.OnSelectedForCanon(scBloc, go);
                break;
            default:
                break;
        }

        panel.SetActive(false);
        UnActiveSystem();
    }

    private void UnActiveSystem()
    {
        playerData.Level++;

        if (playerData.XpForLevelUp.Count > playerData.Level - 1 && 
            playerData.XP >= playerData.XpForLevelUp[playerData.Level - 1])
        {
            ActiveSystem();
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    #endregion
}
