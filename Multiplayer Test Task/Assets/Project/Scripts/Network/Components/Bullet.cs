using Mirror;

using UnityEngine;
public class Bullet : NetworkBehaviour
{
    public float destroyAfter = 2, damage = 15, speed = 1;
    private Transform tr;

    private void Start() => tr = transform;
    public override void OnStartServer() => Invoke(nameof(DestroySelf), destroyAfter);
    private void FixedUpdate() => tr.position += tr.up * speed;

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.TryGetComponent(out Status status))
            status.CmdTakeDamage(damage);
        DestroySelf();
    }
    /// <summary>
    /// ”ничтожить дл€ всех на сервере
    /// </summary>
    [Server]
    private void DestroySelf() => NetworkServer.Destroy(gameObject);
}
