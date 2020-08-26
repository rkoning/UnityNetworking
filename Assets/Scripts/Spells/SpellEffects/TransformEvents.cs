using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformEvents : SpellEffect
{
    public enum EventTransform {
        Cast, Hold, Release, HitAny, HitHealth
    }

    public EventTransform CastTo = EventTransform.Cast;
    public EventTransform HoldTo = EventTransform.Hold;
    public EventTransform ReleaseTo = EventTransform.Release;
    public EventTransform HitAnyTo = EventTransform.HitAny;
    public EventTransform HitHealthTo = EventTransform.HitHealth;

    private Dictionary<EventTransform, Action> mapping = new Dictionary<EventTransform, Action>();

    public override void Register(Spell spell) {
        mapping.Add(EventTransform.Cast, base.Cast);
        mapping.Add(EventTransform.Hold, base.Hold);
        mapping.Add(EventTransform.Release, base.Release);
        mapping.Add(EventTransform.HitAny, () => base.HitAny(null));
        mapping.Add(EventTransform.HitHealth, () => base.HitHealth(null));

        base.Register(spell);
    }

    public override void Cast() {
        mapping[CastTo]();
    }

    public override void Hold() {
        mapping[HoldTo]();
    }

    public override void Release() {
        mapping[ReleaseTo]();
    }

    public override void HitAny(GameObject other) {
        mapping[HitAnyTo]();
    }

    public override void HitHealth(Health other) {
        mapping[HitHealthTo]();
    }
}
