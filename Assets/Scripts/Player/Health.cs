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

    private Avatar lastDamagedBy;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Avatar source) {
        currentHealth -= damage;
        lastDamagedBy = source;
        if (currentHealth <= 0)
        {
            CmdDie();
        }
    }

    [Command]
    public void CmdDie()
    {
        try {
            lastDamagedBy.GetComponent<PlayerScore>().score += 1;
            var spawn = NetworkManager.singleton.GetStartPosition();
            var newPlayer = Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
            NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer);
            RpcRespawn(spawn);
            NetworkServer.Destroy(this.gameObject);
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
    }

    public void Respawn() {

    }
}
