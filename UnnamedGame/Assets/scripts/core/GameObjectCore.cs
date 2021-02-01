using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private static T instance = null;

    public static T Instance {
        get
        {
            return instance;
        }
    }

    protected virtual bool DestroyIfAlreadyExists {
        get
        {
            return true;
        }
    }

    protected virtual void Awake()
    {
        InitSingleton();
    }

    protected virtual void OnDestroy()
    {
        UninitSingleton();
    }

    protected virtual void InitSingleton()
    {
        if (Instance != null && DestroyIfAlreadyExists)//there could be only one...
            Destroy(this.gameObject);
        else
            instance = (T)((MonoBehaviour)this);//Dear compiler, Please shut up. Sincerely, developer
    }
    protected void UninitSingleton()
    {
        if (instance == this)
            instance = null;
    }
}