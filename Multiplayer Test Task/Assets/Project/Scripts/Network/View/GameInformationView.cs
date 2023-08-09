using System.Collections.Generic;
using System.Linq;

using Mirror;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class GameInformationView : MonoBehaviour
{
    /// <summary>
    /// Bootstrap для инициализации синглтонов.
    /// </summary>
    private Bootstrap bootstrap;

    /// <summary>
    /// Сеть для работы с игроками.
    /// </summary>
    private Network network;

    /// <summary>
    /// Локальный игрок.
    /// </summary>
    private Player localPlayer;

    /// <summary>
    /// победный pop-up с информацией, где указаны имя победителя и сколько монет он собрал.
    /// </summary>
    public GameObject popup;

    /// <summary>
    /// Регулятор здоровья.
    /// </summary>
    [SerializeField]
    private Slider slider;

    /// <summary>
    /// Контейнер для списка игроков.
    /// </summary>
    [SerializeField]
    private Transform contentPlayer;

    /// <summary>
    /// Контейнер для списка игроков в попапе.
    /// </summary>
    [SerializeField]
    private Transform contentPlayerPopup;

    /// <summary>
    /// Шаблон для отображения игрока в списке.
    /// </summary>
    [SerializeField]
    private PlayerLabel playerLabel;

    /// <summary>
    /// Джойстик для управления игроком.
    /// </summary>
    [SerializeField]
    private Joystick joystick;

    /// <summary>
    /// Кнопка стрельбы.
    /// </summary>
    [SerializeField]
    private Button shootButton;

    private void Start()
    {
        network = (Network)NetworkManager.singleton;
        network.popup = this;
        network.ChangeListPlayerHandler += OnChangeListPlayer;

        bootstrap = Bootstrap.singleton;
        bool playerSingletonCreated = bootstrap.singletons
            .Any((ISingleton singleton) => singleton.GetType() == typeof(Player) && !singleton.IsUnityNull());

        if (playerSingletonCreated)
            OnSingletonCreate(Player.singleton);
        else
            bootstrap.InitializeSingletonHandler += OnSingletonCreate;
    }

    /// <summary>
    /// Создание окна попапа с игроками.
    /// </summary>
    public void CreatePopup()
    {
        contentPlayerPopup.Clear();
        if (network.players.Count == 0)
            return;
        popup.SetActive(true);
        List<Player> sortPlayers = network.players;
        sortPlayers.Sort();
        for (int idPlayer = 0; idPlayer < sortPlayers.Count; idPlayer++)
        {
            PlayerLabel label = Instantiate(playerLabel, contentPlayerPopup);
            label.Build(sortPlayers[idPlayer]);
        }
    }

    /// <summary>
    /// Обновление списка игроков.
    /// </summary>
    private void OnChangeListPlayer()
    {
        contentPlayer.Clear();
        if (network.players.Count == 0)
            return;
        for (int idPlayer = 0; idPlayer < network.players.Count; idPlayer++)
        {
            PlayerLabel label = Instantiate(playerLabel, contentPlayer);
            label.Build(network.players[idPlayer]);
        }
    }

    private void OnDestroy() => network.ChangeListPlayerHandler -= OnChangeListPlayer;

    /// <summary>
    /// Обработка инициализации синглтона.
    /// </summary>
    public void OnSingletonCreate(ISingleton singleton)
    {
        if (singleton.GetType() != typeof(Player))
            return;

        bootstrap.InitializeSingletonHandler -= OnSingletonCreate;
        localPlayer = Player.singleton;

        localPlayer.joystick = joystick;
        shootButton.onClick.AddListener(() => localPlayer.Shoot());
        localPlayer.status.OnHealthUpdate += (float value) => slider.value = value;
    }
}
