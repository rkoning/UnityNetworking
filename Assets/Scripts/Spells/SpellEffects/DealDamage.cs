using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : SpellEffect
{
    public float damage;

    public override void HitHealth(Health target)
    {
        var health = target.GetComponent<Health>();
        if (health)
        {
            health.TakeDamage(damage, spell.owner);
        }
    }
}
