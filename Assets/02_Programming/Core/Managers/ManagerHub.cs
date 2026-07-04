using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 总管理器，负责统一创建与初始化所有子管理器。
/// 新增管理器时，在 <see cref="RegisterManagers"/> 中调用 <see cref="CreateManager{T}"/> 即可。
/// </summary>
[DefaultExecutionOrder(-10000)]
public class ManagerHub : MonoSingleton<ManagerHub>
{
    [SerializeField] private AudioLibrary _audioLibrary;

    private readonly List<MonoBehaviour> _managers = new();

    protected override void OnSingletonAwake()
    {
        DontDestroyOnLoad(gameObject);
        RegisterManagers();
    }

    /// <summary>
    /// 在此注册所有子管理器。新增管理器只需添加一行 CreateManager 调用。
    /// </summary>
    protected virtual void RegisterManagers()
    {
        AudioManager.PendingLibrary = _audioLibrary;
        CreateManager<AudioManager>();
    }

    /// <summary>
    /// 创建并挂载一个子管理器，返回其实例。
    /// </summary>
    protected T CreateManager<T>() where T : MonoSingleton<T>
    {
        var go = new GameObject(typeof(T).Name);
        go.transform.SetParent(transform, false);

        var manager = go.AddComponent<T>();
        _managers.Add(manager);
        return manager;
    }
}
