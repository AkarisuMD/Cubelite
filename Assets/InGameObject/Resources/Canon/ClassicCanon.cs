using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClassicCanon : CanonBehaviour
{
    public override IEnumerator _Shoot()
    {
        cooldown = true;

        //anim canon shoot ici

        GameObject bullet = pool.GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        bullet.GetComponent<SpriteRenderer>().sprite = scCanon.spriteBullet;
        var col = bullet.GetComponent<ParticleSystem>().colorOverLifetime;
        col.color = scCanon.bulletColor;

        Bullet b = bullet.AddComponent(Type.GetType(scCanon.scriptBullet.name)) as Bullet;
        b.stats = statsbullet;
        b.originePos = transform.position;
        b.audioSource = bullet.GetComponent<AudioSource>();
        if (b.audioSource.outputAudioMixerGroup != null && scCanon.AudioBullet != null)
        {
            b.audioSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            b.audioSource.clip = scCanon.AudioBullet;
        }

        GetComponent<Animation>().Play();

        bullet.SetActive(true);

        audioSource.PlayOneShot(scCanon.ShootAudioCanon);

        if (stats.shootSpeed * PlayerData.Instance.ShootRate < 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            yield return new WaitForSeconds(stats.shootSpeed * PlayerData.Instance.ShootRate);
        }
        cooldown = false;
    }
}
