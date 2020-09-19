using System.Collections;
using UnityEngine;
using Mirror;

public class Spell : NetworkBehaviour, IPoolableObject
{
    public BaseAvatar owner;
    public delegate void CastAction();
    public event CastAction OnCast;
    public event CastAction OnHold;
    public event CastAction OnRelease;
    public event CastAction OnDone;

    public delegate void HitAnyAction(GameObject other);
    public event HitAnyAction OnHitAny;

    public delegate void HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;

    public bool anchorForDuration;
    public bool blockForDuration;

    private float spellEnd;

    public bool parentToCaster;

    private SpellEffect[] effects;

    public void Init()
    {
        // Register event defaults
        OnCast += () => { };

        OnHold += () => { };

        OnRelease += () => { };

        OnDone += () => { };
        OnHitAny += (GameObject other) => { };
        OnHitHealth += (Health target) => { };
 
        // Register all spell effects to effect events
        effects = GetComponents<SpellEffect>();
        foreach(var effect in effects)
        {
            effect.Register(this);
        }
    }

    [ClientRpc]
    public void RpcInit() {
        Init();
    }

    public override void OnStartClient() {
        ObjectPool.singleton.spawnedObjects.Add(GetComponent<NetworkIdentity>().netId, gameObject);
    }

    public void Cast()
    {
        if (parentToCaster) {
            transform.SetParent(owner.aimTransform);
        }
        OnCast();
    }

    public void Hold()
    {
        OnHold();
    }

    public void Release()
    {
        OnRelease();
    }

    public void HitAny(GameObject other)
    {
        OnHitAny(other);
    }

    public void HitHealth(Health target)
    {
        OnHitHealth(target);
    }

    public bool Done() {
        foreach (var effect in effects) {
            if (!effect.Done()) {
                return false;
            }
        }
        CmdDone();
        return true;
    }

    [Command]
    public void CmdDone() {
        RpcDone();
    }

    [ClientRpc]
    public void RpcDone() {
        OnDone();
    }

    public void Despawn() {
        if (hasAuthority)
            CmdDespawn();
    }

    [Command]
    public void CmdDespawn() {
        StartCoroutine(WaitThenDespawn(5f));
    }

    private IEnumerator WaitThenDespawn(float duration) {
        yield return new WaitForSeconds(duration);
        transform.SetParent(null);
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);
    }
}
