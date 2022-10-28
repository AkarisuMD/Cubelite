using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public StatsBullet stats;
    public Vector3 originePos;
    Pool pool;
    public AudioSource audioSource;

    private void Awake()
    {
        pool = Pool.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        BulletBehaviour();

        if ((transform.position - originePos).magnitude > stats.Range)
        {
            ReturnToPool();
        }
    }

    public virtual void BulletBehaviour() { }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (stats.Player && collision.CompareTag("Mob"))
        {
            HitMob(collision);

            ReturnToPool();
        }

        else if (!stats.Player && collision.CompareTag("Player"))
        {
            HitPlayer(collision);

            ReturnToPool();
        }
    }

    bool returnOnlyOnce = false;
    void ReturnToPool()
    {
        if (!returnOnlyOnce)
        {
            returnOnlyOnce = true;
            pool.ReturnToPool(gameObject);
        }
    }

    public virtual void HitMob(Collider2D collision)
    {
        AIAgent ai = collision.GetComponentInParent<AIAgent>();
        bool crit = IsCrit();
        ai.HPModifier(stats.damage);
    }
    public virtual void HitPlayer(Collider2D collision)
    {
        PlayerBehaviour pb = collision.GetComponentInParent<PlayerBehaviour>();
        bool crit = IsCrit();
        pb.TakeDamage(stats.damage, crit);
    }

    bool IsCrit()
    {
        float cc = Random.Range(0f, 1f);
        if (cc < stats.Critical)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
