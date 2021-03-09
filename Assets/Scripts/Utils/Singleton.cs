using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }
    public static bool IsInitialized
    {
        get { return _instance != null; }
    }

    //protected  - доступен потомкам
    //virtual - может быть переопределен в наследнике, но не обязательно.
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class.");
        }
        else
        {
            _instance = (T)this;
        }
    }

    // на случай, если объект эого типа будет уничтожен, освободить инстанс,
    // чтобы можно было создать новый в будущем
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
