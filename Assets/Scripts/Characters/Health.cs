using UnityEngine;
using Mirror;

public class Health : MonoBehaviour {

    public float maxHealth;

    public float currentHealth;

    public bool IsDead = false;

    protected Avatar lastDamagedBy;

    public delegate void DeathEvent();
    public event DeathEvent onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        onDeath += () => {};
    }

    // public override void OnStartServer() {
    //     ObjectPool.Register(GetComponent<NetworkIdentity>().netId, gameObject);
    //     currentHealth = maxHealth;
    // }

    public void Init() {
        // currentHealth = maxHealth;
        // if (isServer) {
        //     RpcInit();
        // }
    }

    public void RpcInit() {
        Init();
    }

    public void TakeDamage(float damage, Avatar source) {
        if (IsDead)
            return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            IsDead = true;
            // RpcDie();
        }
    }

    public void ApplyStatus(StatusFactory factory, Avatar source) {
        Status s = factory.GetStatus(this, source);
        s.Apply();
    }

    // [ClientRpc]
    // public void RpcDie() {
    //     Die();
    // }

    public virtual void Die() {
        onDeath();
    }
}