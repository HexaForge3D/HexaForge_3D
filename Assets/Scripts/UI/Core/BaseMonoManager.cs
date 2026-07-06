using UnityEngine;

public abstract class BaseMonoManager<T> : MonoBehaviour where T : BaseMonoManager<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }
}