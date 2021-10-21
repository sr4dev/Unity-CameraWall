using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }

            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
        }
    }
}