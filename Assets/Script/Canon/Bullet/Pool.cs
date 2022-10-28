using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : Singleton<Pool>
{
    public GameObject bulletPrefab;
    public GameObject poolGameObject;
    public List<GameObject> pool;
    private void Start()
    {
        bulletPrefab = Resources.Load<GameObject>("Bullet/Bullet");
        for (int i = 0; i < 200; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, Vector2.zero, new Quaternion(0,0,0,0), transform);
            pool.Add(bullet);
            bullet.SetActive(false);
        }
    }
    public void ReturnToPool(GameObject obj)
    {
        if (pool.Contains(obj)) return;

        pool.Add(obj);

        Component[] allcompo = obj.GetComponents<Component>();
        foreach (var item in allcompo)
        {
            System.Type type = item.GetType();
            if (type != typeof(Transform) 
            &&  type != typeof(SpriteRenderer) 
            &&  type != typeof(ParticleSystem)
            &&  type != typeof(ParticleSystemRenderer)
            &&  type != typeof(PolygonCollider2D)
            &&  type != typeof(Rigidbody2D)
            &&  type != typeof(AudioSource)
            &&  type != typeof(CircleCollider2D))
            {
                Destroy(item);
            }
        }

        obj.SetActive(false);
    }

    public GameObject GetBullet()
    {
        if (pool[0] != null)
        {
            GameObject bullet = pool[0];
            pool.Remove(bullet);
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, Vector2.zero, new Quaternion(0, 0, 0, 0), transform);
            return bullet;
        }
    }
}
