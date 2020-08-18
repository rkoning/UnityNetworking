using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int connectionId;

    public float maxHealth;
    [SyncVar]
    public float currentHealth;

    [SyncVar(hook = nameof(OnDeath))]
    public bool IsDead = false;

    private Avatar lastDamagedBy;

    public GamePlayer gamePlayer;
    private MeshRenderer meshRenderer;

    private Color initialColor;
    public Color deadColor = Color.red;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
        currentHealth = maxHealth;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            TakeDamage(100f, null);
        }
    }

    public void TakeDamage(float damage, Avatar source) {
        if (!hasAuthority) {
            return;
        }
        currentHealth -= damage;
        lastDamagedBy = source;
        if (currentHealth <= 0)
        {
            IsDead = true;
            
            StartRespawn();
        }
    }

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
    
    private void OnDeath(bool oldValue, bool newValue) {
        // gamePlayer.CmdStartRespawn();
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
