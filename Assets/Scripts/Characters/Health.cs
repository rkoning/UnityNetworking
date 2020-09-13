using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Health : NetworkBehaviour {

    
    [SyncVar]
    public float armor;

    [SyncVar(hook = nameof(OnMaxArmorChanged))]
    public float maxArmor;

    [SyncVar(hook = nameof(OnMaxHealthChanged))]
    public float maxHealth;

    [SyncVar(hook = nameof(OnCurrentHealthChanged))]
    public float currentHealth;

    [SyncVar]
    public bool IsDead = false;

    protected Avatar lastDamagedBy;

    public delegate void HealthEvent(float damage);

    public event HealthEvent OnDeath;
    public event HealthEvent OnDamaged;
    public event HealthEvent OnHealthDamaged;
    public event HealthEvent OnArmorBroken;
    public event HealthEvent OnArmorDamaged;

    private List<StatusFactory> currentEffects = new List<StatusFactory>();

    protected virtual void Start()
    {
        OnDeath += (float damage) => {};
        OnDamaged += (float damage) => {};
        OnHealthDamaged += (float damage) => {};
        OnArmorBroken += (float damage) => {};
        OnArmorDamaged += (float damage) => {};
    }

    public override void OnStartServer() {
        ObjectPool.Register(GetComponent<NetworkIdentity>().netId, gameObject);
        currentHealth = maxHealth;
    }

    [Server]
    public virtual void TakeDamage(float damage, Avatar source) {
        if (IsDead)
            return;
        if (armor > 0) {
            float armorDamageTaken = damage - armor;
            if (armorDamageTaken > 0) {
                // if enough damage was dealt to bypass the current armor
                // destroy armor
                armor = 0;
                OnArmorBroken(damage);
                // damage health for the remainder
                currentHealth -= armorDamageTaken;
                OnHealthDamaged(armorDamageTaken);
            } else {
                // if armor absorbs all of the incoming damage, damage armor only
                armor -= damage;
                OnArmorDamaged(damage);
            }
        } else {
            currentHealth -= damage;
            OnHealthDamaged(damage);
        }

        OnDamaged(damage);
        if (currentHealth <= 0)
        {
            IsDead = true;
            OnDeath(damage);
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

    public virtual void OnMaxHealthChanged(float oldValue, float newValue) {
        if (currentHealth > newValue)
            currentHealth = newValue;           
    } 

    public virtual void OnMaxArmorChanged(float oldValue, float newValue) {
        if (armor > newValue)
            armor = newValue;
    }

    public virtual void Die() {

    }
}