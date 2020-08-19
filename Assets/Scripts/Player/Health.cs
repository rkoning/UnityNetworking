using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour {

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
    public void TakeDamage(float damage, Avatar source) {
        if (!hasAuthority) {
            return;
        }
        currentHealth -= damage;
        lastDamagedBy = source;
        if (currentHealth <= 0)
        {
            IsDead = true;
            
            Die();
        }
    }

    public virtual void Die() {
        
    }
}