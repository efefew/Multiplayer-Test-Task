using Mirror;

using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Network))]
public class LobbyView : MonoBehaviour
{
    #region Fields

    private Network network;

    [SerializeField]
    private Button buttonStart;

    [SerializeField]
    private Toggle toggleReady;

    [SerializeField]
    private Button buttonStartHost, buttonStartClient/*, buttonStartServer*/;

    [SerializeField]
    private Button buttonStop;
    //[SerializeField]
    //private Button buttonStopHost, buttonStopClient, buttonStopServer;

    [SerializeField]
    private InputField networkAddress;

    [SerializeField]
    private GameObject infoPlayer;

    private bool networkClientConnected, networkClientActived, networkServerActived;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        network = GetComponent<Network>();

        network.AddRoomPlayerHandler += (NetworkRoomPlayer player) =>
        {
            if (!player.isOwned)
                return;
            toggleReady.onValueChanged.RemoveAllListeners();
            toggleReady.onValueChanged.AddListener((bool on) => player.CmdChangeReadyState(on));
        };

        network.RemoveRoomPlayerHandler += (NetworkRoomPlayer player) =>
        {
            toggleReady.isOn = false;
        };

        buttonStart.interactable = false;
        network.AllPlayersReadyHandler += (bool allPlayersReady) =>
        {
            buttonStart.interactable = allPlayersReady;
        };

        buttonStart.onClick.AddListener(() => network.ServerChangeScene(network.GameplayScene));

        networkAddress.onValueChanged.AddListener((string input) => network.networkAddress = input);

        buttonStartHost.onClick.AddListener(() =>
        {
            infoPlayer.SetActive(false);
            network.StartHost();
        });
        buttonStartClient.onClick.AddListener(() =>
        {
            infoPlayer.SetActive(false);
            network.StartClient();
        });
        //buttonStartServer.onClick.AddListener(() => network.StartServer());

        buttonStop.onClick.AddListener(() =>
        {
            if (NetworkClient.isConnected)
            {
                if (NetworkServer.active)
                    network.StopHost();
                else
                    network.StopClient();
                infoPlayer.SetActive(true);
            }
        });
        //buttonStopHost.onClick.AddListener(() => network.StopHost());
        //buttonStopClient.onClick.AddListener(() => network.StopClient());
        //buttonStopServer.onClick.AddListener(() => network.StopServer());

        OnChangeNetwork(first: true);

    }

    private void Update() => OnChangeNetwork();

    private void OnChangeNetwork(bool first = false)
    {
        if (!first && NetworkClient.isConnected == networkClientConnected && NetworkServer.active == networkServerActived && networkClientActived == NetworkClient.active)
            return;
        bool isOffline = !NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active;
        bool isOnline = NetworkClient.isConnected && NetworkServer.active && NetworkClient.active;

        network.room.SetActive(NetworkClient.isConnected && NetworkClient.active);
        networkAddress.gameObject.SetActive(isOffline);

        buttonStart.gameObject.SetActive(NetworkServer.active);
        buttonStartHost.gameObject.SetActive(isOffline);
        buttonStartClient.gameObject.SetActive(isOffline);
        //buttonStartServer.interactable = isOffline;

        buttonStop.gameObject.SetActive(isOnline || NetworkClient.isConnected);
        //buttonStopClient.interactable = isOnline || NetworkClient.isConnected;
        //buttonStopHost.interactable = NetworkServer.active && NetworkClient.isConnected;
        //buttonStopServer.interactable = NetworkServer.active && !NetworkClient.isConnected;

        networkClientConnected = NetworkClient.isConnected;
        networkServerActived = NetworkServer.active;
        networkClientActived = NetworkClient.active;
    }

    #endregion Methods
}