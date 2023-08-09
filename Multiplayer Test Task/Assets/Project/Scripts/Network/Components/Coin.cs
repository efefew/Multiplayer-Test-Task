using Mirror;

using UnityEngine;

public class Coin : MonoBehaviour
{
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.TryGetComponent(out Player player))
        {
            player.CmdCoins();
            Network network = (Network)NetworkManager.singleton;
            NetworkServer.Spawn(
                   Instantiate(network.coinPrefab,
                   network.GetCoinPosition(),
                   Quaternion.identity));
            DestroySelf();
        }
    }
    /// <summary>
    /// ”ничтожить дл€ всех на сервере
    /// </summary>
    [Server]
    private void DestroySelf() => NetworkServer.Destroy(gameObject);
}
