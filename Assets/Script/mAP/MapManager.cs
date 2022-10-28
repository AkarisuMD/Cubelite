using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MapManager : Singleton<MapManager>
{
    [SerializeField, NonReorderable] public List<Map> _maps = new List<Map>();
    public ScWorld CurrentWorld;

    private void Start() => StartCoroutine(GenerateMap());

    public bool buttonPassTestDebug = false;
    public void Button()
    {
        buttonPassTestDebug = true;
    }

    [SerializeField] private int NombreMap = 0;
    [SerializeField, NonReorderable] private List<Entry> entries = new List<Entry>();
    IEnumerator GenerateMap()
    {
        NombreMap = 0;

        //debug
        /*
        while (!buttonPassTestDebug)
        {
            yield return null;
        }
        buttonPassTestDebug = false;*/

        ScMap scMap = CurrentWorld.Spawns[UnityEngine.Random.Range(0, CurrentWorld.Spawns.Count)];
        GenerateMap(scMap, new Entry(), new Entry(), true);

        for (int i = 1; i < CurrentWorld.MaxNbMap; i++)
        {
            yield return null;
            //debug 
            /*
            while (!buttonPassTestDebug)
            {
                yield return null;
            }
            buttonPassTestDebug = false;*/

            List<Entry> AllEntrys = new List<Entry>();
            List<ScMap> AllMaps = new List<ScMap>();

            Entry entry = entries[UnityEngine.Random.Range(0, entries.Count)];
            int rd = UnityEngine.Random.Range(0, entry.entrysOrientation.Count);

            switch (entry.entrysOrientation[rd])
            {
                case Orientation.NORD:

                    switch (entry.Side)
                    {
                        case Orientation.NORD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.NORD) && 
                                        _Entrys.Side == Orientation.SUD && 
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdNord = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryNord = AllEntrys[rdNord];
                            ScMap scMapNord = AllMaps[rdNord];

                            GenerateMap(scMapNord, entry, entryNord, false);
                            break;

                        case Orientation.EST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.NORD) && 
                                        _Entrys.Side == Orientation.OUEST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdEst = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryEst = AllEntrys[rdEst];
                            ScMap scMapEst = AllMaps[rdEst];

                            GenerateMap(scMapEst, entry, entryEst, false);
                            break;
                        case Orientation.SUD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.NORD) && 
                                        _Entrys.Side == Orientation.NORD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdSud = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entrySud = AllEntrys[rdSud];
                            ScMap scMapSud = AllMaps[rdSud];

                            GenerateMap(scMapSud, entry, entrySud, false);
                            break;

                        case Orientation.OUEST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.NORD) && 
                                        _Entrys.Side == Orientation.EST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdOuest = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryOuest = AllEntrys[rdOuest];
                            ScMap scMapOuest = AllMaps[rdOuest];

                            GenerateMap(scMapOuest, entry, entryOuest, false);
                            break;

                        default:
                            break;
                    }
                    break;


                case Orientation.EST:

                    switch (entry.Side)
                    {
                        case Orientation.NORD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.EST) && 
                                        _Entrys.Side == Orientation.SUD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdNord = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryNord = AllEntrys[rdNord];
                            ScMap scMapNord = AllMaps[rdNord];

                            GenerateMap(scMapNord, entry, entryNord, false);
                            break;

                        case Orientation.EST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.EST) && 
                                        _Entrys.Side == Orientation.OUEST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdEst = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryEst = AllEntrys[rdEst];
                            ScMap scMapEst = AllMaps[rdEst];

                            GenerateMap(scMapEst, entry, entryEst, false);
                            break;
                        case Orientation.SUD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.EST) && 
                                        _Entrys.Side == Orientation.NORD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdSud = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entrySud = AllEntrys[rdSud];
                            ScMap scMapSud = AllMaps[rdSud];

                            GenerateMap(scMapSud, entry, entrySud, false);
                            break;

                        case Orientation.OUEST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.EST) && 
                                        _Entrys.Side == Orientation.EST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdOuest = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryOuest = AllEntrys[rdOuest];
                            ScMap scMapOuest = AllMaps[rdOuest];

                            GenerateMap(scMapOuest, entry, entryOuest, false);
                            break;

                        default:
                            break;
                    }
                    break;


                case Orientation.SUD:

                    switch (entry.Side)
                    {
                        case Orientation.NORD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.SUD) && 
                                        _Entrys.Side == Orientation.SUD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdNord = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryNord = AllEntrys[rdNord];
                            ScMap scMapNord = AllMaps[rdNord];

                            GenerateMap(scMapNord, entry, entryNord, false);
                            break;

                        case Orientation.EST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.SUD) && 
                                        _Entrys.Side == Orientation.OUEST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdEst = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryEst = AllEntrys[rdEst];
                            ScMap scMapEst = AllMaps[rdEst];

                            GenerateMap(scMapEst, entry, entryEst, false);
                            break;
                        case Orientation.SUD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.SUD) &&
                                        _Entrys.Side == Orientation.NORD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdSud = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entrySud = AllEntrys[rdSud];
                            ScMap scMapSud = AllMaps[rdSud];

                            GenerateMap(scMapSud, entry, entrySud, false);
                            break;

                        case Orientation.OUEST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.SUD) && 
                                        _Entrys.Side == Orientation.EST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdOuest = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryOuest = AllEntrys[rdOuest];
                            ScMap scMapOuest = AllMaps[rdOuest];

                            GenerateMap(scMapOuest, entry, entryOuest, false);
                            break;

                        default:
                            break;
                    }
                    break;


                case Orientation.OUEST:

                    switch (entry.Side)
                    {
                        case Orientation.NORD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.OUEST) && 
                                        _Entrys.Side == Orientation.SUD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdNord = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryNord = AllEntrys[rdNord];
                            ScMap scMapNord = AllMaps[rdNord];

                            GenerateMap(scMapNord, entry, entryNord, false);
                            break;

                        case Orientation.EST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.OUEST) && 
                                        _Entrys.Side == Orientation.OUEST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdEst = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryEst = AllEntrys[rdEst];
                            ScMap scMapEst = AllMaps[rdEst];

                            GenerateMap(scMapEst, entry, entryEst, false);
                            break;
                        case Orientation.SUD:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.OUEST) && 
                                        _Entrys.Side == Orientation.NORD &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdSud = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entrySud = AllEntrys[rdSud];
                            ScMap scMapSud = AllMaps[rdSud];

                            GenerateMap(scMapSud, entry, entrySud, false);
                            break;

                        case Orientation.OUEST:

                            foreach (var _MapForEntrys in CurrentWorld.ScMaps)
                            {
                                foreach (var _Entrys in _MapForEntrys.type.entries)
                                {
                                    if (_Entrys.entrysOrientation.Contains(Orientation.OUEST) && 
                                        _Entrys.Side == Orientation.EST &&
                                        _Entrys.entrySize == entry.entrySize)
                                    {
                                        AllEntrys.Add(_Entrys);
                                        AllMaps.Add(_MapForEntrys);
                                    }
                                }
                            }
                            int rdOuest = UnityEngine.Random.Range(0, AllEntrys.Count);
                            Entry entryOuest = AllEntrys[rdOuest];
                            ScMap scMapOuest = AllMaps[rdOuest];

                            GenerateMap(scMapOuest, entry, entryOuest, false);
                            break;

                        default:
                            break;
                    }
                    break;


                default:
                    break;
            }

            entries.Remove(entry);
            NombreMap++;
        }
    }

    void GenerateMap(ScMap scMap, Entry entry, Entry toEntry, bool IsSpawn)
    {
        if (IsSpawn)
        {
            GameObject Spawn = Instantiate(scMap.MapObj, transform.position, new Quaternion(0, 0, 0, 0), transform);

            foreach (var _entry in scMap.type.entries)
            {
                Entry __entry = new Entry();
                __entry.EntryPos = _entry.EntryPos + Spawn.transform.position;
                __entry.entrysOrientation = _entry.entrysOrientation;
                __entry.Side = _entry.Side;
                __entry.entryType = _entry.entryType;
                __entry.entrySize = _entry.entrySize;

                Debug.Log("AddSpawnEntries");
                entries.Add(__entry);
            }
        }
        else
        {
            GameObject map = Instantiate(scMap.MapObj, transform.position, new Quaternion(0, 0, 0, 0), transform);

            map.transform.position = entry.EntryPos - toEntry.EntryPos;

            foreach (var _entry in scMap.type.entries)
            {
                if (toEntry.EntryPos != _entry.EntryPos)
                {

                    Entry __entry = new Entry();

                    __entry.EntryPos = Quaternion.AngleAxis(map.transform.rotation.z, Vector3.forward) * _entry.EntryPos + map.transform.position;
                    /*
                    List<Orientation> newOrientations = new List<Orientation>();
                    foreach (var entryOrientation in _entry.entrysOrientation)
                    {
                        switch (entryOrientation)
                        {
                            case Orientation.NORD:
                                if (map.transform.eulerAngles.z == 0)
                                {
                                    newOrientations.Add(Orientation.NORD);
                                }
                                else if (map.transform.eulerAngles.z == 90)
                                {
                                    newOrientations.Add(Orientation.EST);
                                }
                                else if (map.transform.eulerAngles.z == 180)
                                {
                                    newOrientations.Add(Orientation.SUD);
                                }
                                else if (map.transform.eulerAngles.z == 270)
                                {
                                    newOrientations.Add(Orientation.OUEST);
                                }
                                else { Debug.LogError(map.transform.rotation.z + map.name + "Wrong map rotation !!!"); }
                                break;
                            case Orientation.EST:
                                if (map.transform.eulerAngles.z == 0)
                                {
                                    newOrientations.Add(Orientation.EST);
                                }
                                else if (map.transform.eulerAngles.z == 90)
                                {
                                    newOrientations.Add(Orientation.SUD);
                                }
                                else if (map.transform.eulerAngles.z == 180)
                                {
                                    newOrientations.Add(Orientation.OUEST);
                                }
                                else if (map.transform.eulerAngles.z == 270)
                                {
                                    newOrientations.Add(Orientation.NORD);
                                }
                                else { Debug.LogError("Wrong map rotation !!!"); }
                                break;
                            case Orientation.SUD:
                                if (map.transform.eulerAngles.z == 0)
                                {
                                    newOrientations.Add(Orientation.SUD);
                                }
                                else if (map.transform.eulerAngles.z == 90)
                                {
                                    newOrientations.Add(Orientation.OUEST);
                                }
                                else if (map.transform.eulerAngles.z == 180)
                                {
                                    newOrientations.Add(Orientation.NORD);
                                }
                                else if (map.transform.eulerAngles.z == 270)
                                {
                                    newOrientations.Add(Orientation.EST);
                                }
                                else { Debug.LogError("Wrong map rotation !!!"); }
                                break;
                            case Orientation.OUEST:
                                if (map.transform.eulerAngles.z == 0)
                                {
                                    newOrientations.Add(Orientation.OUEST);
                                }
                                else if (map.transform.eulerAngles.z == 90)
                                {
                                    newOrientations.Add(Orientation.NORD);
                                }
                                else if (map.transform.eulerAngles.z == 180)
                                {
                                    newOrientations.Add(Orientation.EST);
                                }
                                else if (map.transform.eulerAngles.z == 270)
                                {
                                    newOrientations.Add(Orientation.SUD);
                                }
                                else { Debug.LogError("Wrong map rotation !!!"); }
                                break;
                            default:
                                break;
                        }
                    }
                    __entry.entrysOrientation = newOrientations;

                    Orientation newSide = Orientation.NORD;
                    switch (_entry.Side)
                    {
                        case Orientation.NORD:
                            if (map.transform.eulerAngles.z == 0)
                            {
                                newSide = Orientation.NORD;
                            }
                            else if (map.transform.eulerAngles.z == 90)
                            {
                                newSide = Orientation.EST;
                            }
                            else if (map.transform.eulerAngles.z == 180)
                            {
                                newSide = Orientation.SUD;
                            }
                            else if (map.transform.eulerAngles.z == 270)
                            {
                                newSide = Orientation.OUEST;
                            }
                            else { Debug.LogError("Wrong map rotation !!!"); }
                            break;
                        case Orientation.EST:
                            if (map.transform.eulerAngles.z == 0)
                            {
                                newSide = Orientation.EST;
                            }
                            else if (map.transform.eulerAngles.z == 90)
                            {
                                newSide = Orientation.SUD;
                            }
                            else if (map.transform.eulerAngles.z == 180)
                            {
                                newSide = Orientation.OUEST;
                            }
                            else if (map.transform.eulerAngles.z == 270)
                            {
                                newSide = Orientation.NORD;
                            }
                            else { Debug.LogError("Wrong map rotation !!!"); }
                            break;
                        case Orientation.SUD:
                            if (map.transform.eulerAngles.z == 0)
                            {
                                newSide = Orientation.SUD;
                            }
                            else if (map.transform.eulerAngles.z == 90)
                            {
                                newSide = Orientation.OUEST;
                            }
                            else if (map.transform.eulerAngles.z == 180)
                            {
                                newSide = Orientation.NORD;
                            }
                            else if (map.transform.eulerAngles.z == 270)
                            {
                                newSide = Orientation.EST;
                            }
                            else { Debug.LogError("Wrong map rotation !!!"); }
                            break;
                        case Orientation.OUEST:
                            if (map.transform.eulerAngles.z == 0)
                            {
                                newSide = Orientation.OUEST;
                            }
                            else if (map.transform.eulerAngles.z == 90)
                            {
                                newSide = Orientation.NORD;
                            }
                            else if (map.transform.eulerAngles.z == 180)
                            {
                                newSide = Orientation.EST;
                            }
                            else if (map.transform.eulerAngles.z == 270)
                            {
                                newSide = Orientation.SUD;
                            }
                            else { Debug.LogError("Wrong map rotation !!!"); }
                            break;
                        default:
                            break;
                    }
                    __entry.Side = newSide;
                */

                    __entry.entrysOrientation = _entry.entrysOrientation;
                    __entry.Side = _entry.Side;
                    __entry.entryType = _entry.entryType;
                    __entry.entrySize = _entry.entrySize;

                    Debug.Log("AddMapEntries");
                    entries.Add(__entry);
                }
            }
        }
    }
}


[Serializable] public struct MapType
{
    public Orientation orientation;
    public MapSize mapSize;
    [NonReorderable] public List<Entry> entries;
}
[Serializable] public struct MapSize
{
    public int x;
    public int y;
}
[Serializable] public enum Orientation
{
    NORD,
    EST,
    SUD,
    OUEST,
}
[Serializable] public enum EntryType
{
    CLASSIC,
}
[Serializable] public struct Entry
{
    public Vector3 EntryPos;
    public List<Orientation> entrysOrientation;
    public Orientation Side;
    public EntryType entryType;
    public int entrySize;
}