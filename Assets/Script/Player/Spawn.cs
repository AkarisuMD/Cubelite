using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] private bool SkipAnim;

    public GameObject prefab;
    public ParticleSystem ps;
    public AnimationCurve curve;

    public float SpeedNeed;
    public float speedAnim;

    [SerializeField] private AudioSource _source;

    void Start()
    {
        if (PlayerData.Instance.player == null && !SkipAnim)
            StartCoroutine(AnimationSpawn());
        else if (PlayerData.Instance.player == null && SkipAnim)
            SpawnPlayer();
    }

    IEnumerator AnimationSpawn()
    {
        CameraManager cm = CameraManager.Instance;
        while (cm.transform.position != transform.position)
        {
            cm.SetCameraTransform(transform.position, new Quaternion(0, 0, 0, 0), 20, 4.5f, SensCamera.Down);
        }

        ps.Play();
        _source.Play();
        var velocityOverLifetime = ps.velocityOverLifetime;

        float speed = 0;
        while (speed < SpeedNeed/100)
        {
            speed += speedAnim/10;
            ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve(speed, curve);
            velocityOverLifetime.speedModifier = minMaxCurve;
            yield return new WaitForSeconds(0.01f);
        } while (speed < SpeedNeed/20)
        {
            speed += speedAnim/3;
            ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve(speed, curve);
            velocityOverLifetime.speedModifier = minMaxCurve;
            yield return new WaitForSeconds(0.01f);
        } while (speed < SpeedNeed)
        {
            speed += speedAnim;
            ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve(speed, curve);
            velocityOverLifetime.speedModifier = minMaxCurve;
            yield return new WaitForSeconds(0.01f);
        }
        ps.Stop();
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Instantiate(prefab, transform.position, new Quaternion(0, 0, 0, 0));
        CameraManager.Instance.FollowPlayer = true;
        PlayerData.Instance.CanChangeGravity = true;
        Timer.Instance.TimerStart();
    }
}
