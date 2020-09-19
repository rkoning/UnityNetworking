using UnityEngine;
using Mirror;

public abstract class BaseAvatar : NetworkBehaviour {
    
    public CharacterAttribute attackSpeed;
    public CharacterAttribute moveSpeed;
    public CharacterAttribute maxArmor;
    public CharacterAttribute maxHealth;

    public Transform aimTransform;
    public Vector3 lookPoint;
    public Animator animator;
    public Health health;
    protected AnimatorOverrideController overrideController;
    public Faction faction;

    public virtual void Init() {
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    public virtual void OverrideAnimation(string name, AnimationClip clip) {
        overrideController[name] = clip;
    }

    public abstract void GetFromPool(string name, Vector3 position, Quaternion rotation);
    public abstract void DealDamage(Health other, float amount);
    public abstract void ApplyStatus(Health health, StatusFactory factory);
}