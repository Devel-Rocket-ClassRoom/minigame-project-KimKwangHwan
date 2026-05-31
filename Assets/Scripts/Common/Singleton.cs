using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    private static bool _isQuitting = false;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_isQuitting) return null;

            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    var prefab = Resources.Load<T>(typeof(T).Name);
                    _instance = prefab != null ? Instantiate(prefab) : new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void OnApplicationQuit() => _isQuitting = true;
    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}