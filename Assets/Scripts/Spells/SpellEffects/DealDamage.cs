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
        if (hitOnce) {
            if (!hits.Contains(health.gameObject))
                spell.owner.gamePlayer.DealDamage(health, damage);
        } else {
            spell.owner.gamePlayer.DealDamage(health, damage);
        }
        hits.Add(health.gameObject);
    }
}
