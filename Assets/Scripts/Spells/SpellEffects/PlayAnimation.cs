using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : SpellEffect
{
    public string trigger = "isCasting";


    public override void Register(Spell spell) {
        base.Register(spell);
    }
    
    public override void Cast() {
        spell.owner.animator.SetTrigger(trigger);
    }
}