using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : Health
{
    public Avatar avatar;

    public Animator animator;

    public event HealthEvent OnRespawn;

    protected override void Start() {
        base.Start();
        OnRespawn += () => {};
    }
    
    public override void OnCurrentHealthChanged(float oldValue, float newValue) {
        base.OnCurrentHealthChanged(oldValue, newValue);
        if (oldValue <= 0 && newValue > 0 && OnRespawn != null)
            OnRespawn();
    }

    public void Respawn() {
        currentHealth = maxHealth;
        IsDead = false;
        OnRespawn();
    }
}
