using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectSphere : SpellEffect
{
    public float radius = 5f;
    
    public override void Cast() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < hitColliders.Length; i++) {
            var target = hitColliders[i].GetComponent<Health>();
            if (target) {
                spell.HitHealth(target);
            }
            HitAny(hitColliders[i].gameObject);
            spell.HitAny(hitColliders[i].gameObject);
        }
    }
}
