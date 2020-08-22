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

    public Animator animator;

    public delegate void HealthEvent();
    public event HealthEvent OnDeath;
    public event HealthEvent OnRespawn;

    private void Start() {
        OnDeath += () => {};
        OnRespawn += () => {};
    }

    private void StartRespawn() {
        CmdPlayerDead();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            TakeDamage(100f, null);
        }
    }

    private IEnumerator WaitThenRespawn(float duration) {
        yield return new WaitForSeconds(duration);
        IsDead = false;
        currentHealth = maxHealth;
        var spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        RpcPlayerAlive();
    }

    public override void Die() {
        Debug.Log($"Die called, client: {connectionToClient} server: {connectionToServer}, Authority? {hasAuthority}");
        if (hasAuthority)
            StartRespawn();
    }
    
    [Command]
    private void CmdPlayerDead() {
        Debug.Log("Player dying on server" + connectionToClient);
        RpcPlayerDead();
        var clips = animator.GetCurrentAnimatorClipInfo(0);
        StartCoroutine(WaitThenRespawn(clips[0].clip.averageDuration + 3f));
    }

    [ClientRpc]
    private void RpcPlayerDead() {
        Debug.Log("Player dying on client" + connectionToServer);
        animator.SetTrigger("Dying");
        OnDeath();
    }

    [Command]
    private void CmdPlayerAlive() {
        RpcPlayerAlive();
    }

    [ClientRpc]
    private void RpcPlayerAlive() {
        animator.SetTrigger("Alive");
        OnRespawn();
    }
}
