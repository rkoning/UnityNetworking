using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour, IPoolableObject {

    public float maxHealth;

    [SyncVar]
    public float currentHealth;

    [SyncVar]
    public bool IsDead = false;

    protected Avatar lastDamagedBy;

    public delegate void DeathEvent();
    public event DeathEvent onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        onDeath += () => {};
    }

    public override void OnStartServer() {
        ObjectPool.Register(GetComponent<NetworkIdentity>().netId, gameObject);
        currentHealth = maxHealth;
    }

    public void Init() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Avatar source) {
        if (IsDead)
            return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            IsDead = true;
            RpcDie();
        }
    }

    [ClientRpc]
    public void RpcDie() {
        Die();
    }

    public virtual void Die() {
        onDeath();
    }
}