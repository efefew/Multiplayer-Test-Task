using System;
using System.Collections.Generic;

using Mirror;

using UnityEngine;

public class Network : NetworkRoomManager
{
    #region Events
    /// <summary>
    /// Событие, вызываемое при начале игры
    /// </summary>
    public event Action<bool> StartGameHandler;
    /// <summary>
    /// Событие, вызываемое при изменении списка игроков
    /// </summary>
    public event Action ChangeListPlayerHandler;

    #endregion Events

    #region Properties
    /// <summary>
    /// Список игроков
    /// </summary>
    public List<Player> players = new();
    /// <summary>
    /// Список игроков в комнате
    /// </summary>
    public List<RoomPlayer> roomPlayers = new();

    #endregion Properties

    #region Fields

    [SerializeField]
    private float minX, maxX, minY, maxY;// Минимальные и максимальные координаты для создания монет

    [SerializeField]
    [Min(0)]
    private int minCount, maxCount;// Минимальное и максимальное количество монет

    /// <summary>
    /// Контейнер для игроков в комнате
    /// </summary>
    public Transform roomPlayerConteiner;
    /// <summary>
    /// Объект комнаты
    /// </summary>
    public GameObject room;
    /// <summary>
    /// Префаб монеты
    /// </summary>
    public GameObject coinPrefab;
    /// <summary>
    /// победный pop-up с информацией, где указаны имя победителя и сколько монет он собрал.
    /// </summary>
    public GameInformationView popup;
    /// <summary>
    /// Имя игрока
    /// </summary>
    public string nickname { get; set; } = "";
    /// <summary>
    /// Цвет игрока
    /// </summary>
    public Color colorPlayer;
    /// <summary>
    /// Количество живых игроков
    /// </summary>
    public int alivePlayers;
    #endregion Fields

    #region Methods
    /// <summary>
    /// Добавление игрока в список игроков
    /// </summary>
    /// <param name="player">игрок</param>
    public void AddPlayer(Player player)
    {
        players.Add(player);
        ChangeListPlayerHandler?.Invoke();
    }
    /// <summary>
    /// Удаление игрока из списка игроков
    /// </summary>
    /// <param name="player">игрок</param>
    public void RemovePlayer(Player player)
    {
        _ = players.Remove(player);
        ChangeListPlayerHandler?.Invoke();
    }
    /// <summary>
    /// Создание монет при изменении сцены
    /// </summary>
    /// <param name="sceneName">имя сцены</param>
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
    /// Вызов события при начале игры
    /// </summary>
    /// <param name="sceneName">имя сцены</param>
    public override void ServerChangeScene(string sceneName)
    {
        StartGameHandler?.Invoke(sceneName == GameplayScene);
        alivePlayers = roomPlayers.Count;
        base.ServerChangeScene(sceneName);
    }
    /// <summary>
    /// Проверка количества живых игроков
    /// </summary>
    public void CheckGameStatus()
    {
        if (alivePlayers <= 1)
            popup.CreatePopup();
    }
    /// <summary>
    /// Получение случайных координат для создания монет
    /// </summary>
    /// <returns>случайные координаты</returns>
    public Vector2 GetCoinPosition() => new(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));

    #endregion Methods
}