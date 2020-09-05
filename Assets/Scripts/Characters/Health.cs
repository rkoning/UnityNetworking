using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Health : NetworkBehaviour {

    [SyncVar]
    public float maxHealth;

    [SyncVar(hook = nameof(OnCurrentHealthChanged))]
    public float currentHealth;

    [SyncVar]
    public bool IsDead = false;

    protected Avatar lastDamagedBy;

    public delegate void HealthEvent();
    public event HealthEvent OnDeath;
    public event HealthEvent OnDamaged;

    private List<StatusFactory> currentEffects = new List<StatusFactory>();

    protected virtual void Start()
    {
        OnDeath += () => {};
        OnDamaged += () => {};
    }

    public override void OnStartServer() {
        ObjectPool.Register(GetComponent<NetworkIdentity>().netId, gameObject);
        currentHealth = maxHealth;
    }

    [Server]
    public virtual void TakeDamage(float damage, Avatar source) {
        if (IsDead)
            return;
        currentHealth -= damage;
        OnDamaged();
        if (currentHealth <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    [Server]
    public void ApplyStatus(StatusFactory factory, Avatar source) {
        if (currentEffects.Contains(factory))
            return;
        Status s = factory.GetStatus(this, source);
        currentEffects.Add(factory);
        s.OnUnapply += () => currentEffects.Remove(factory);
        s.Apply();
    }

    public virtual void OnCurrentHealthChanged(float oldValue, float newValue) {
        if (newValue <= 0)
            Die();
    } 

    public virtual void Die() {
        OnDeath();
    }
}