using Mirror;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkRoomPlayer))]
/// <summary>
/// Класс игрока в комнате, управляющий отображением игрока в комнате
/// </summary>
public class RoomPlayer : NetworkBehaviour, ISingleton
{
    #region Properties

    /// <summary>
    /// Синглтон игрока в комнате
    /// </summary>
    public static RoomPlayer singleton { get; private set; }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Никнейм игрока
    /// </summary>
    [SyncVar(hook = nameof(ChangeNickname))]
    public string nickname;

    /// <summary>
    /// Цвет игрока
    /// </summary>
    [SyncVar]
    public Color colorPlayer;

    [SerializeField]
    private Button buttonKick;

    [SerializeField]
    private TMP_Text labelNamePlayer, labelIsReady;

    private NetworkRoomPlayer player;

    private Network network;

    #endregion Fields

    #region Methods

    public override void OnStartClient()
    {
        base.OnStartClient();
        network = (Network)NetworkManager.singleton;
        player = GetComponent<NetworkRoomPlayer>();
        InitializeSingleton();
        labelIsReady.text = player.readyToBegin ? "ГОТОВ" : "НЕ ГОТОВ";
        if (isOwned)
        {
            CmdNickname(network.nickname != "" ? network.nickname : $"Player [{player.index + 1}]");
            CmdColorPlayer(network.colorPlayer);
        }

        labelNamePlayer.text = nickname;

        player.ReadyToBeginHandler += (bool readyToBegin) => labelIsReady.text = readyToBegin ? "ГОТОВ" : "НЕ ГОТОВ";

        network.roomPlayers.Add(this);
        transform.SetParent(network.roomPlayerConteiner);
        transform.localScale = Vector3.one;
        if (isServer)
        {
            if (player.index > 0 || isServerOnly)
            {
                buttonKick.gameObject.SetActive(true);
                buttonKick.onClick.AddListener(() => { GetComponent<NetworkIdentity>().connectionToClient.Disconnect(); });
            }

            network.StartGameHandler += RpcStartGame;
        }

        if (isServerOnly)
        {
            network.AllPlayersReadyHandler += allPlayersReady => network.ServerChangeScene(network.GameplayScene);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        _ = network.roomPlayers.Remove(this);
    }

    [Command(requiresAuthority = false)]
    public void CmdNickname(string nickname) => this.nickname = nickname;

    public void ChangeNickname(string oldNickname, string newNickname) => labelNamePlayer.text = newNickname;

    [Command(requiresAuthority = false)]
    public void CmdColorPlayer(Color colorPlayer) => this.colorPlayer = colorPlayer;

    public override void OnStopServer()
    {
        base.OnStopServer();
        network.StartGameHandler -= RpcStartGame;
    }

    [ClientRpc]
    public void RpcStartGame(bool on) => network.room.SetActive(!on && NetworkClient.isConnected && NetworkClient.active);

    public void InitializeSingleton()
    {
        if (!isLocalPlayer)
            return;
        if (singleton != null)
        {
            Debug.LogError("singleton > 1");
            return;
        }

        singleton = this;
        Bootstrap.singleton.OnCreateSingleton(singleton);
    }

    #endregion Methods
}