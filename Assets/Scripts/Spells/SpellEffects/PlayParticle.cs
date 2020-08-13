using UnityEngine;
using System.Collections.Generic;

public class PlayParticle : SpellEffect
{
    public new ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public override void Cast()
    {
        particleSystem.Play();
    }

    private void OnParticleCollision(GameObject other)
    {
        var target = other.GetComponent<Target>();
        if (target)
        {
            //HitTarget(target);
            spell.HitTarget(target);
        }
        HitAny(other);
        spell.HitAny(other);
        /*int numEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < numEvents; i++)
        {
            
        }*/
    }
}
