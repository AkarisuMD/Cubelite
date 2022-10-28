using UnityEngine;

public abstract class SingletonND<T> : MonoBehaviour where T : SingletonND<T>
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            instance = instance ?? (FindObjectOfType(typeof(T)) as T);
            instance = instance ?? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            return instance;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(Instance.gameObject);
    }
    private void OnApplicationQuit()
    {
        instance = null;
    }
}
