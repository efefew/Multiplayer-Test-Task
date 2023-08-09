using System.Collections.Generic;
using System.Linq;

using Mirror;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class GameInformationView : MonoBehaviour
{
    #region Fields

    private Bootstrap bootstrap;
    private Network network;
    private Player localPlayer;
    public GameObject popup;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Transform contentPlayer, contentPlayerPopup;

    [SerializeField]
    private PlayerLabel playerLabel;

    [SerializeField]
    private Joystick joystick;

    [SerializeField]
    private Button shootButton;

    #endregion Fields

    #region Methods

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

    #endregion Methods
}