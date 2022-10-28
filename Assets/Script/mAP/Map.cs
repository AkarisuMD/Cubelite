using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("Map Camera Setting")]
    public CameraSetting cameraSetting;
    public Vector3 Posistion;
    public quaternion Rotation;
    public float SpeedTransition = 2f;
    public float SizeCamera = 15f;
    public SensCamera sensCamera;

    [Header("Spawner Mobs")]
    public MobSpawner[] mobSpawners;
    public List<float> DifficultyMultiplirEveryMinute;
    public float FrequenceOfSpawn;
    private bool spawn;

    bool playerInside;

    private void OnEnable()
    {
        mobSpawners = GetComponentsInChildren<MobSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            spawn = true;
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            if (cameraSetting == CameraSetting.MapCamera)
            {
                CameraManager.Instance.FollowPlayer = false;
                CameraManager.Instance.SetCameraTransform(Posistion, Rotation, SpeedTransition, SizeCamera, sensCamera);
            }

            if (spawn)
                StartCoroutine(Spawn());
        }
    }
    IEnumerator Spawn()
    {
        spawn = false;

        float Cooldown = 0;
        if (Timer.Instance.minute < 20)
            Cooldown = FrequenceOfSpawn * DifficultyMultiplirEveryMinute[Timer.Instance.minute];
        else Cooldown = FrequenceOfSpawn * DifficultyMultiplirEveryMinute[20];

        yield return new WaitForSeconds(Cooldown / 2);
        foreach (var item in mobSpawners)
        {
            item.Spawn();
        }
        yield return new WaitForSeconds(Cooldown / 2);

        spawn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraManager.Instance.FollowPlayer = true; 
            playerInside = false;
            spawn = false;
            StopAllCoroutines();
        }
    }
}

public enum CameraSetting
{
    FollowPlayer,
    MapCamera
}
