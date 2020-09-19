using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideAnimation : SpellEffect
{
    public AnimationClip animationClip;
    public string clipName = "Cast";
    public float performAtPercent = 1f;

    private float clipEnd;

    public override void Register(Spell spell) {
        base.Register(spell);
    }
    
    public override void Cast() {
        spell.owner.OverrideAnimation(clipName, animationClip);
        clipEnd = Time.fixedTime + animationClip.averageDuration * performAtPercent;
        StartCoroutine(WaitThenDo(animationClip.averageDuration * performAtPercent, base.Cast));
    }

    public override bool Done() {
        return Time.fixedTime > clipEnd;
    }
}
