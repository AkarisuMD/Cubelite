using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicBullet : Bullet
{
    private void Start()
    {
        audioSource.loop = true;
        audioSource.Play();
    }
    public override void BulletBehaviour()
    {
        transform.position += transform.up * stats.Speed * Time.deltaTime;
    }
}
