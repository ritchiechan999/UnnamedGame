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

/// <summary>
/// Global Event
/// </summary>
public static partial class Core
{
    public delegate void GameEvent(object sender, object[] args);
    static Dictionary<string, GameEvent> _eventBag = new Dictionary<string, GameEvent>();

    public static void SubscribeEvent(string eventName, GameEvent listener)
    {
        //subscribe an event to the specified name
        GameEvent existing = null;
        //try to get existing event
        if (_eventBag.TryGetValue(eventName, out existing)) {
            //if event exists, subscribe from event
            existing += listener;
        } else {
            //if event not exists, subscribe from event
            existing = listener;
        }
        _eventBag[eventName] = existing;
    }

    public static void UnsubscribeEvent(string eventName, GameEvent listener)
    {
        //Unsubscribe an event
        /*
         * if event exists, unsubscribe from event
         * if event is null, remove event from dictionary
         * else update dictionary
         */
        GameEvent existing = null;
        //try to get existing event
        if (_eventBag.TryGetValue(eventName, out existing)) {
            //if event exists, unsubscribe from event
            existing -= listener;
        }
        if (existing == null) {
            //if event is null, remove event from dictionary
            _eventBag.Remove(eventName);
        } else {
            //else update dictionary
            _eventBag[eventName] = existing;
        }
    }

    public static void BroadcastEvent(string eventName, object sender, params object[] args)
    {
        GameEvent existing = null;
        if (_eventBag.TryGetValue(eventName, out existing)) {
            existing(sender, args);
        }
    }
}