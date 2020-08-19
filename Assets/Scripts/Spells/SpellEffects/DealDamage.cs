using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : SpellEffect
{
    public float damage;

    public override void HitTarget(Target target)
    {
        var health = target.GetComponent<PlayerHealth>();
        if (health && health != spell.owner.GetComponent<PlayerHealth>())
        {
            health.TakeDamage(damage, spell.owner);
        }
    }
}
