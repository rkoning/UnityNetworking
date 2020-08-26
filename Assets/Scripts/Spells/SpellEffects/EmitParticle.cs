using UnityEngine;
using System.Collections.Generic;

public class EmitParticle : SpellEffect
{
    public new ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public bool cancelable;
    public int numParticles = 1;
    
    public override void Cast()
    {
        particleSystem.Emit(numParticles);
    }

    private void OnParticleCollision(GameObject other)
    {
        var target = other.GetComponent<Health>();
        if (target)
        {
            spell.HitHealth(target);
        }
        HitAny(other);
        spell.HitAny(other);
    }
}
