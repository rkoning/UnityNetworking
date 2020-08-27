using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAtCasterFeet : SpellEffect
{
    public override void Cast() {
        transform.position = spell.owner.transform.position - Vector3.up;
        transform.rotation = Quaternion.identity;
        base.Cast();
    }
}
