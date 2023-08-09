using System;
using System.Collections;

using Mirror;
using Mirror.Experimental;

using UnityEngine;

[RequireComponent(typeof(Status))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkRigidbody2D))]
public class Player : NetworkBehaviour, IComparable<Player>, ISingleton//Игрок
{

    #region Events

    public event Action ChangeCoinsHandler;

    #endregion Events

    #region Properties

    /// <summary>
    /// singleton localPlayer
    /// </summary>
    public static Player singleton { get; private set; }

    #endregion Properties

    #region Fields

    private Network network;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Rigidbody2D rb2D;

    [SerializeField]
    private float forceScale = 1000, torqueScale = 1000, delayShoot = 0.2f, bulletOffset = 35;

    [SerializeField]
    private GameObject bullet;

    private bool canShoot;
    public Joystick joystick;
    [SyncVar(hook = nameof(ChangeCoins))]
    public int coins;
    [SyncVar(hook = nameof(ChangeNickname))]
    public string nickname;
    [SyncVar(hook = nameof(ChangeColorPlayer))]
    public Color colorPlayer;

    [SerializeField]
    public Status status;

    #endregion Fields

    #region Methods

    [Client]
    private void FixedUpdate()
    {
        if (!isLocalPlayer || !Application.isFocused)
            return;

        //Move(
        //    (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
        //    (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
        //Rotation((Input.GetKey(KeyCode.Q) ? 1 : 0) - (Input.GetKey(KeyCode.E) ? 1 : 0));

        if (joystick != null)
        {
            Move(0, joystick.inputVector.y);
            Rotation(joystick.inputVector.x);
        }
    }

    [Client]
    private void Move(float x, float y) => rb2D.AddForce((forceScale * playerTransform.up * y) + (forceScale * playerTransform.right * x));

    [Client]
    private void Rotation(float direction) => rb2D.AddTorque(torqueScale * direction);

    [Client]
    private IEnumerator IShoot()
    {
        canShoot = false;
        CmdShoot(network.spawnPrefabs.IndexOf(bullet), playerTransform.position + (playerTransform.up * bulletOffset), playerTransform.rotation);
        yield return new WaitForSeconds(delayShoot);
        canShoot = true;
    }

    private void SetInfoPlayer()
    {
        if (!isOwned)
            return;
        CmdSetInfoPlayer(network.colorPlayer, network.nickname);
    }

    public void Shoot()
    {
        if (!canShoot)
            return;
        _ = StartCoroutine(IShoot());
    }
    public void ChangeColorPlayer(Color oldColorPlayer, Color newColorPlayer) => GetComponent<SpriteRenderer>().color = newColorPlayer;
    public void ChangeNickname(string oldNickname, string newNickname) => ChangeCoinsHandler?.Invoke();

    public override void OnStartClient()
    {
        base.OnStartClient();
        network = (Network)NetworkManager.singleton;

        SetInfoPlayer();
        canShoot = true;
        InitializeSingleton();
        network.AddPlayer(this);
    }
    [Command(requiresAuthority = false)]
    public void CmdSetInfoPlayer(Color color, string nickname, NetworkConnectionToClient sender = null)
    {
        colorPlayer = color;
        this.nickname = nickname;
    }
    public void ChangeCoins(int oldValue, int newValue)
    {
        coins = newValue;
        ChangeCoinsHandler?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdCoins(NetworkConnectionToClient sender = null) => coins++;

    public override void OnStopClient()
    {
        base.OnStopClient();
        network.RemovePlayer(this);
        network.CheckGameStatus();
    }
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

    [Command(requiresAuthority = false)]
    public void CmdShoot(int prefabID, Vector3 position, Quaternion quaternion, NetworkConnectionToClient sender = null)
    {
        GameObject obj = Instantiate(network.spawnPrefabs[prefabID], position, quaternion);
        NetworkServer.Spawn(obj);
    }

    public int CompareTo(Player player) => player is null ? throw new ArgumentException("Некорректное значение параметра") : player.coins - coins;

    #endregion Methods

}