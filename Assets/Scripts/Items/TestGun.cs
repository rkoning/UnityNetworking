using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGun : Usable
{
    public ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public override void Use()
    {
        ps.Emit(1);
    }

    private void OnParticleCollision(GameObject other) {
        var h = other.GetComponent<Health>();
        h.TakeDamage(1f);
    }

}
