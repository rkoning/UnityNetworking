using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class NPCController : BaseAvatar, IPoolableObject {

    protected NavMeshAgent agent;
    protected Vector2 smoothDeltaPosition = Vector2.zero;
    protected Vector2 velocity = Vector2.zero;
    protected bool dead;

    public void RpcInit() {
        
    }

    private void OnEnable() {
        if (agent)
            return;
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 500f, 1)) {
            transform.position = closestHit.position;
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.baseOffset = 0f;
        }
    }

    public override void OnStartServer() {
        // if (enabled) {
        //    NavMeshHit closestHit;
        //    if (NavMesh.SamplePosition(transform.position, out closestHit, 500f, 1)) {
        //       transform.position = closestHit.position;
        //       agent = gameObject.AddComponent<NavMeshAgent>();
        //    }
        // }
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    protected virtual void Update() {
        SetAttributes();
    }


    protected virtual void SetAttributes() {
        health.maxArmor = maxArmor.CurrentValue;
        health.maxHealth = maxHealth.CurrentValue;
    }

    public override void GetFromPool(string name, Vector3 position, Quaternion rotation) {
        var go = ObjectPool.singleton.GetFromPool(name, position, rotation);
        go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    public override void ApplyStatus(Health other, StatusFactory factory) {
       other.ApplyStatus(factory, this);
    }

    public override void DealDamage(Health other, float amount) {
        other.TakeDamage(amount, this);
    }
}