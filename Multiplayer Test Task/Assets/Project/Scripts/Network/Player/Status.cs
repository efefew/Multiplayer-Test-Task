using System;

using Mirror;

using UnityEngine;

public class Status : NetworkBehaviour
{

    #region Events

    public event Action<float> OnHealthUpdate;

    #endregion Events

    #region Fields

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    [SyncVar(hook = nameof(ChangeHealth))]
    private float health;

    [SyncVar]
    public uint? fractionID = null;

    #endregion Fields

    #region Methods

    private void Start() => health = maxHealth;

    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(float damage) => health -= damage;

    public void ChangeHealth(float oldHealth, float newHealth)
    {
        OnHealthUpdate?.Invoke(Mathf.Max(0, newHealth) / maxHealth);
        if (newHealth <= 0)
        {
            gameObject.SetActive(false);
            Network network = (Network)NetworkManager.singleton;
            network.alivePlayers--;
            network.CheckGameStatus();
        }
    }

    #endregion Methods

}