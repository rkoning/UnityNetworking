using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : SpellEffect
{
    public float damage;

    public bool hitOnce = true;
    private List<GameObject> hits;

    public override void Cast() {
        hits = new List<GameObject>();
    }

    public override void HitHealth(Health health)
    {

        if (hitOnce && !hits.Contains(health.gameObject))
            health.TakeDamage(damage, spell.owner);

        hits.Add(health.gameObject);
    }
}
