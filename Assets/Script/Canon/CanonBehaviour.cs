using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CanonBehaviour : MonoBehaviour
{
    public bool Player;
    public ScCanon scCanon;
    public StatsCanon stats;
    public StatsBullet statsbullet;
    public Pool pool;
    public AudioSource audioSource;

    private void Start()
    {
        pool = Pool.Instance;
        StatsStats();
        SetStats();
        if (Player)
        {
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.outputAudioMixerGroup = GetComponentInParent<AudioSource>().outputAudioMixerGroup;
        audioSource.volume = scCanon.VolumeAudio;

        GetComponent<Animation>().AddClip(scCanon.anim, "Shoot");
    }

    void StatsStats() { stats = scCanon.Stats; }

    public virtual void SetStats()
    {
        if (Player)
        {
            statsbullet = new StatsBullet();
            statsbullet.Player = Player;
            statsbullet.damage = Mathf.CeilToInt(stats.damage * PlayerData.Instance.Damage);
            statsbullet.Range = stats.range * PlayerData.Instance.Range;
            statsbullet.Speed = stats.bulletSpeed;
            statsbullet.Critical = stats.critical * PlayerData.Instance.Critical;
        }

        else
        {
            AIStats aIStats = GetComponentInParent<AIStats>();

            statsbullet = new StatsBullet();
            statsbullet.Player = Player;
            statsbullet.damage = Mathf.CeilToInt(stats.damage * aIStats.Damage);
            statsbullet.Range = stats.range * aIStats.Range;
            statsbullet.Speed = stats.bulletSpeed;
            statsbullet.Critical = stats.critical * aIStats.Critical;
        }
    }

    public void Updatehit()
    {
        statsbullet.damage = stats.damage;
        statsbullet.Range = stats.range;
        statsbullet.Critical = stats.critical;

        SetStats();
    }

    public bool cooldown;
    public void Shoot() { if (!cooldown) StartCoroutine(_Shoot()); }
    public virtual IEnumerator _Shoot()
    {
        yield return null;
    }
}
