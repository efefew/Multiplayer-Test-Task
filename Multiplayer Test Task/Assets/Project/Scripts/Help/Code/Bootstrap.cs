using System;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class Bootstrap : MonoBehaviour, ISingleton
{
    public static Bootstrap singleton { get; private set; }
    /// <summary>
    /// Обработчик для инициализации синглтона
    /// </summary>
    public Action<ISingleton> InitializeSingletonHandler;
    /// <summary>
    /// Список синглтонов
    /// </summary>
    public List<ISingleton> singletons { get; private set; } = new List<ISingleton>();
    private void Awake() => InitializeSingleton();

    public void InitializeSingleton()
    {
        if (singleton != null)
        {
            Destroy(this);
            return;
        }

        singleton = this;
        if (Application.isPlaying)
        {
            // Принудительно переместить объект в корень сцены, на случай, если пользователь сделал его дочерним по отношению к чему-либо в сцене,
            // поскольку DontDestroyOnLoad разрешен только для корневых объектов сцены
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        OnCreateSingleton(singleton);
    }
    /// <summary>
    /// Обработчик для создания синглтона
    /// </summary>
    /// <param name="singleton">синглтон</param>
    public void OnCreateSingleton(ISingleton singleton)
    {
        _ = singletons.RemoveAll(x => x.IsUnityNull());
        InitializeSingletonHandler?.Invoke(singleton);
        singletons.Add(singleton);
    }
}
public interface ISingleton
{
    /// <summary>
    /// Инициализация синглтона
    /// </summary>
    public void InitializeSingleton();
}
