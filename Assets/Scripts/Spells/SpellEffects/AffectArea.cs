using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectArea : SpellEffect
{
    public GameObject areaToEffect;

    private void Start() {
        areaToEffect.SetActive(false);
    }
    
    public override void Cast() {
        areaToEffect.gameObject.SetActive(true);    
    }

    public override void Release() {
        areaToEffect.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other) {
        var target = other.GetComponent<Target>();
        if (target)
        {
            //HitTarget(target);
            spell.HitTarget(target);
        }
        HitAny(other.gameObject);
        spell.HitAny(other.gameObject);
    }
}
