using System.Collections.Generic;
using System.Linq;

using Mirror;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class GameInformationView : MonoBehaviour
{
    /// <summary>
    /// Bootstrap ��� ������������� ����������.
    /// </summary>
    private Bootstrap bootstrap;

    /// <summary>
    /// ���� ��� ������ � ��������.
    /// </summary>
    private Network network;

    /// <summary>
    /// ��������� �����.
    /// </summary>
    private Player localPlayer;

    /// <summary>
    /// �������� pop-up � �����������, ��� ������� ��� ���������� � ������� ����� �� ������.
    /// </summary>
    public GameObject popup;

    /// <summary>
    /// ��������� ��������.
    /// </summary>
    [SerializeField]
    private Slider slider;

    /// <summary>
    /// ��������� ��� ������ �������.
    /// </summary>
    [SerializeField]
    private Transform contentPlayer;

    /// <summary>
    /// ��������� ��� ������ ������� � ������.
    /// </summary>
    [SerializeField]
    private Transform contentPlayerPopup;

    /// <summary>
    /// ������ ��� ����������� ������ � ������.
    /// </summary>
    [SerializeField]
    private PlayerLabel playerLabel;

    /// <summary>
    /// �������� ��� ���������� �������.
    /// </summary>
    [SerializeField]
    private Joystick joystick;

    /// <summary>
    /// ������ ��������.
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
    /// �������� ���� ������ � ��������.
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
    /// ���������� ������ �������.
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
    /// ��������� ������������� ���������.
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
