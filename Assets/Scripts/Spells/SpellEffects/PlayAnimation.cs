using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : SpellEffect
{
    public AnimationClip animationClip;
    public string overrideName = "Cast";
    public float performAtPercent = 1f;

    public override void Register(Spell spell) {
        duration = animationClip.averageDuration * performAtPercent;
        base.Register(spell);
    }
    
    public override void Cast() {
        spell.owner.SetCastAnimation(animationClip);
        spell.owner.animator.SetTrigger("isCasting");
        StartCoroutine(WaitThenDo(animationClip.averageDuration * performAtPercent, base.Cast));
    }
}
