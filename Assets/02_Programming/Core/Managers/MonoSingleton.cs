using UnityEngine;

/// <summary>
/// MonoBehaviour 单例基类。子类继承后自动获得单例生命周期管理。
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
        OnSingletonAwake();
    }

    /// <summary>
    /// 单例初始化完成后的回调，子类可覆写以执行初始化逻辑。
    /// </summary>
    protected virtual void OnSingletonAwake() { }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
