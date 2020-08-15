using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    public float maxHealth;
    [SyncVar]
    public float currentHealth;

    [SyncVar(hook = nameof(OnDeath))]
    public bool IsDead = false;

    private Avatar lastDamagedBy;

    public GamePlayer gamePlayer;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            TakeDamage(100f, null);
        }
    }

    public void TakeDamage(float damage, Avatar source) {
        Debug.Log(connectionToServer + " " + connectionToClient + " " + hasAuthority);
        currentHealth -= damage;
        lastDamagedBy = source;
        if (currentHealth <= 0)
        {
            IsDead = true;
            gamePlayer.CmdStartRespawn();
        }
    }

    private void OnDeath(bool oldValue, bool newValue) {
        // gamePlayer.CmdStartRespawn();
    }
}
