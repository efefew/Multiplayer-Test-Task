using System;
using System.Collections.Generic;

using Mirror;

using UnityEngine;

public class Network : NetworkRoomManager
{
    #region Events

    public event Action<bool> StartGameHandler;

    public event Action ChangeListPlayerHandler;

    #endregion Events

    #region Properties

    public List<Player> players = new();
    public List<RoomPlayer> roomPlayers = new();

    #endregion Properties

    #region Fields

    [SerializeField]
    private float minX, maxX, minY, maxY;

    [SerializeField]
    [Min(0)]
    private int minCount, maxCount;

    public Transform roomPlayerConteiner;
    public GameObject room;
    public GameObject coinPrefab;
    public GameInformationView popup;

    public string nickname { get; set; } = "";
    public Color colorPlayer;
    public int alivePlayers;
    #endregion Fields

    #region Methods

    public void AddPlayer(Player player)
    {
        players.Add(player);
        ChangeListPlayerHandler?.Invoke();
    }

    public void RemovePlayer(Player player)
    {
        _ = players.Remove(player);
        ChangeListPlayerHandler?.Invoke();
    }

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

    public override void ServerChangeScene(string newSceneName)
    {
        StartGameHandler?.Invoke(newSceneName == GameplayScene);
        alivePlayers = roomPlayers.Count;
        base.ServerChangeScene(newSceneName);
    }
    public void CheckGameStatus()
    {
        if (alivePlayers <= 1)
            popup.CreatePopup();
    }
    public Vector2 GetCoinPosition() => new(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));

    #endregion Methods
}