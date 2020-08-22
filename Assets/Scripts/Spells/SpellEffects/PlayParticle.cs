using UnityEngine;
using System.Collections.Generic;

public class PlayParticle : SpellEffect
{
    public new ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public override void Cast()
    {
        Debug.Log($"Play Particle {parent} {spell}");
        particleSystem.Play();
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
