using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolXP : Singleton<PoolXP>
{
    public GameObject XpPrefab;
    public GameObject poolGameObject;
    public List<GameObject> pool;
    private void Start()
    {
        for (int i = 0; i < 200; i++)
        {
            GameObject XP = Instantiate(XpPrefab, Vector2.zero, new Quaternion(0, 0, 0, 0), transform);
            pool.Add(XP);
            XP.SetActive(false);
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
            && type != typeof(SpriteRenderer)
            && type != typeof(ParticleSystem)
            && type != typeof(ParticleSystemRenderer)
            && type != typeof(XpBehaviour)
            && type != typeof(BoxCollider2D))
            {
                Destroy(item);
            }
        }

        obj.SetActive(false);
    }

    public GameObject GetXp()
    {
        if (this != null && pool[0] != null)
        {
            GameObject XP = pool[0];
            pool.Remove(XP);
            return XP;
        }
        else if (this != null)
        {
            GameObject XP = Instantiate(XpPrefab, Vector2.zero, new Quaternion(0, 0, 0, 0), transform);
            return XP;
        }
        else return null;
    }
}
