using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : MonoBehaviour
{
    public Avatar avatar;

    public float maxHealth;

    public float currentHealth;

    public bool IsDead = false;

    public Animator animator;

    public delegate void HealthEvent();
    public event HealthEvent OnDeath;
    public event HealthEvent OnRespawn;

    private void Start() {
        OnDeath += () => {};
        OnRespawn += () => {};
    }

    private void StartRespawn() {
        avatar.gamePlayer.PlayerDead();
        // CmdPlayerDead();
    }

    public void TakeDamage(float damage, Avatar source) {
        if (IsDead)
            return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // IsDead = true;
            StartRespawn();
        }
    }

    public void Death() {
        OnDeath();
    }

    public void Respawn() {
        OnRespawn();
    }
}
