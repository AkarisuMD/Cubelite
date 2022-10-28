using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBehaviour : MonoBehaviour
{
    PlayerData playerData;
    GameObject target;
    public AudioClip clip;

    private void Start()
    {
        playerData = PlayerData.Instance;
    }
    private void Update()
    {
        if (target == null && (playerData.player.transform.position - transform.position).magnitude < 10f)
        {
            target = playerData.player;
        }

        if ((playerData.player.transform.position - transform.position).magnitude > 50f)
        {
            PoolXP.Instance.ReturnToPool(gameObject);
        }

        if (target != null)
        {
            var Dir = target.transform.position - transform.position;
            transform.Translate(Dir * 0.05f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            target.GetComponent<PlayerBehaviour>().AddXP(1);
            target.GetComponent<AudioSource>().PlayOneShot(clip);
            PoolXP.Instance.ReturnToPool(gameObject);
        }
    }
}
