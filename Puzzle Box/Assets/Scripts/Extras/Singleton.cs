using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    protected virtual void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        instance = this as T;
    }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = $"{typeof(T).Name} - AutoCreated";
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public static void StartSingleton()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<T>();

            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = $"{typeof(T).Name} - AutoCreated";
                instance = obj.AddComponent<T>();
            }
        }
    }
}