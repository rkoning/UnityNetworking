using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour, IPoolableObject {

    public float maxHealth;

    [SyncVar]
    public float currentHealth;

    [SyncVar]
    public bool IsDead = false;

    protected Avatar lastDamagedBy;

    protected MeshRenderer meshRenderer;

    protected Color initialColor;
    public Color deadColor = Color.red;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
        currentHealth = maxHealth;
    }

    public override void OnStartServer() {
        ObjectPool.Register(GetComponent<NetworkIdentity>().netId, gameObject);
        currentHealth = maxHealth;
    }

    public void Init() {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Avatar source) {
        if (IsDead)
            return;
        Debug.Log($"CmdTakeDamage {connectionToClient} for {damage}");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            IsDead = true;
            RpcDie();
        }
    }

    [ClientRpc]
    public void RpcDie() {
        Die();
    }

    public virtual void Die() {
        gameObject.SetActive(false);
    }
}