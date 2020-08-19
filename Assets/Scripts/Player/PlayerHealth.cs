using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : Health
{
    [SyncVar]
    public int connectionId;

    public GamePlayer gamePlayer;

    private void StartRespawn() {
        CmdPlayerDead();
        StartCoroutine(WaitThenRespawn(3f));
    }

    private IEnumerator WaitThenRespawn(float duration) {
        yield return new WaitForSeconds(duration);
        IsDead = false;
        currentHealth = maxHealth;
        var spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        CmdPlayerAlive();
    }
    
    [Command]
    private void CmdPlayerDead() {
        RpcPlayerDead();
    }

    [ClientRpc]
    private void RpcPlayerDead() {
        meshRenderer.material.color = deadColor;
    }

    [Command]
    private void CmdPlayerAlive() {
        RpcPlayerAlive();
    }

    [ClientRpc]
    private void RpcPlayerAlive() {
        meshRenderer.material.color = initialColor;
    }
}
