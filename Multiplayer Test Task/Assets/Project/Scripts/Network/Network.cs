using System;
using System.Collections.Generic;

using Mirror;

using UnityEngine;

public class Network : NetworkRoomManager
{
    #region Events
    /// <summary>
    /// �������, ���������� ��� ������ ����
    /// </summary>
    public event Action<bool> StartGameHandler;
    /// <summary>
    /// �������, ���������� ��� ��������� ������ �������
    /// </summary>
    public event Action ChangeListPlayerHandler;

    #endregion Events

    #region Properties
    /// <summary>
    /// ������ �������
    /// </summary>
    public List<Player> players = new();
    /// <summary>
    /// ������ ������� � �������
    /// </summary>
    public List<RoomPlayer> roomPlayers = new();

    #endregion Properties

    #region Fields

    [SerializeField]
    private float minX, maxX, minY, maxY;// ����������� � ������������ ���������� ��� �������� �����

    [SerializeField]
    [Min(0)]
    private int minCount, maxCount;// ����������� � ������������ ���������� �����

    /// <summary>
    /// ��������� ��� ������� � �������
    /// </summary>
    public Transform roomPlayerConteiner;
    /// <summary>
    /// ������ �������
    /// </summary>
    public GameObject room;
    /// <summary>
    /// ������ ������
    /// </summary>
    public GameObject coinPrefab;
    /// <summary>
    /// �������� pop-up � �����������, ��� ������� ��� ���������� � ������� ����� �� ������.
    /// </summary>
    public GameInformationView popup;
    /// <summary>
    /// ��� ������
    /// </summary>
    public string nickname { get; set; } = "";
    /// <summary>
    /// ���� ������
    /// </summary>
    public Color colorPlayer;
    /// <summary>
    /// ���������� ����� �������
    /// </summary>
    public int alivePlayers;
    #endregion Fields

    #region Methods
    /// <summary>
    /// ���������� ������ � ������ �������
    /// </summary>
    /// <param name="player">�����</param>
    public void AddPlayer(Player player)
    {
        players.Add(player);
        ChangeListPlayerHandler?.Invoke();
    }
    /// <summary>
    /// �������� ������ �� ������ �������
    /// </summary>
    /// <param name="player">�����</param>
    public void RemovePlayer(Player player)
    {
        _ = players.Remove(player);
        ChangeListPlayerHandler?.Invoke();
    }
    /// <summary>
    /// �������� ����� ��� ��������� �����
    /// </summary>
    /// <param name="sceneName">��� �����</param>
    [Server]
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == GameplayScene)
        {
            int count = UnityEngine.Random.Range(minCount, maxCount);

            _ = spawnPrefabs.IndexOf(coinPrefab);
            for (int id = 0; id < count; id++)
            {
                NetworkServer.Spawn(
                    Instantiate(coinPrefab,
                    GetCoinPosition(),
                    Quaternion.identity));
            }
        }
    }
    /// <summary>
    /// ����� ������� ��� ������ ����
    /// </summary>
    /// <param name="sceneName">��� �����</param>
    public override void ServerChangeScene(string sceneName)
    {
        StartGameHandler?.Invoke(sceneName == GameplayScene);
        alivePlayers = roomPlayers.Count;
        base.ServerChangeScene(sceneName);
    }
    /// <summary>
    /// �������� ���������� ����� �������
    /// </summary>
    public void CheckGameStatus()
    {
        if (alivePlayers <= 1)
            popup.CreatePopup();
    }
    /// <summary>
    /// ��������� ��������� ��������� ��� �������� �����
    /// </summary>
    /// <returns>��������� ����������</returns>
    public Vector2 GetCoinPosition() => new(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));

    #endregion Methods
}