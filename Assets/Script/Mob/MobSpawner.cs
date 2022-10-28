using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MobSpawner : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public ScMob scMob;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
    public void Spawn()
    {
        StartCoroutine(_Spawn());
    }

    IEnumerator _Spawn()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0);
        while (spriteRenderer.color.a <= 0.75f)
        {
            spriteRenderer.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.05f);
        GameObject mob = Instantiate(scMob.prefab, transform.position, new Quaternion(0, 0, 0, 0));
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
}
