using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : SpellEffect
{
    public AnimationClip animationClip;
    public string overrideName = "Cast";
    public float performAtPercent = 1f;

    public override void Cast() {
        // animator = spell.owner.animator;
        // var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        
        // var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        // foreach (var a in aoc.animationClips) {
        //     Debug.Log(a.name);
        //     if (a.name == overrideName)
        //         anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, animationClip));
        // }
        // aoc.ApplyOverrides(anims);
        // animator.runtimeAnimatorController = aoc;
        spell.owner.SetCastAnimation(animationClip);
        spell.owner.animator.SetTrigger("isCasting");
        StartCoroutine(WaitThenDo(animationClip.averageDuration * performAtPercent, base.Cast));
    }

    protected IEnumerator WaitThenDo(float duration, System.Action action) {
        Debug.Log(duration);
        yield return new WaitForSeconds(duration);
        action();
    }
}
