using UnityEngine;
using System.Collections.Generic;

public class ParticleCollision : MonoBehaviour {
    public Spell spell;

    public bool occurOnce;
    private List<GameObject> hits = new List<GameObject>();

    private void Start() {
        // Every time the spell is cast, set hits to new array
        spell.OnCast += () => hits = new List<GameObject>();
    }

    private void OnParticleCollision(GameObject other) {
        if (occurOnce) { // if the Hit effects are only supposed to occur once, check if we have already hit this
            if (hits.Contains(other))
                return;
            else
                hits.Add(other);
        }

        var target = other.GetComponent<Health>();
        if (target)
        {
            spell.HitHealth(target);
        }
        spell.HitAny(other);
    }
}