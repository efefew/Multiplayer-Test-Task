using System;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class Bootstrap : MonoBehaviour, ISingleton
{
    public static Bootstrap singleton { get; private set; }
    /// <summary>
    /// ���������� ��� ������������� ���������
    /// </summary>
    public Action<ISingleton> InitializeSingletonHandler;
    /// <summary>
    /// ������ ����������
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
            // ������������� ����������� ������ � ������ �����, �� ������, ���� ������������ ������ ��� �������� �� ��������� � ����-���� � �����,
            // ��������� DontDestroyOnLoad �������� ������ ��� �������� �������� �����
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        OnCreateSingleton(singleton);
    }
    /// <summary>
    /// ���������� ��� �������� ���������
    /// </summary>
    /// <param name="singleton">��������</param>
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
    /// ������������� ���������
    /// </summary>
    public void InitializeSingleton();
}
