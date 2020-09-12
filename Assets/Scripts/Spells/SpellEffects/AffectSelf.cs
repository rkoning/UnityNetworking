using UnityEngine;

public class AffectSelf : SpellEffect {
    public override void Cast()
    {
        HitAny(spell.owner.gameObject);
        HitHealth(spell.owner.health);
    }
}