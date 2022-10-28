using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Canon", menuName = "ScriptableObjects/Canon", order = 2)]
public class ScCanon : BaseScriptable
{
    [Header("CANON")]
    public Sprite sprite;
    public Object scriptCanon;
    public StatsCanon Stats;
    public float VolumeAudio;
    public AudioClip AudioCanon;
    public AudioClip ShootAudioCanon;

    public AnimationClip anim;

    [Space(10)]
    [Header("BULLET")]
    public Sprite spriteBullet;
    public Object scriptBullet;
    public AudioClip AudioBullet;
    public Gradient bulletColor;

}
