using UnityEngine;
using System.Collections.Generic;

public class PlayParticle : SpellEffect
{
    public new ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public bool cancelable;
    
    public override void Cast()
    {
        if (particleSystem.isStopped)
            particleSystem.Play();
    }

    public override void Release() {
        if (particleSystem.isPlaying && cancelable)
            particleSystem.Stop();
    }

    public override bool Done() {
        return particleSystem.isStopped;
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
