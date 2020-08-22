using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformEvents : SpellEffect
{
    public enum EventTransform {
        Cast, Hold, Release
    }

    public EventTransform CastTo = EventTransform.Cast;
    public EventTransform HoldTo = EventTransform.Hold;
    public EventTransform ReleaseTo = EventTransform.Release;
    private Dictionary<EventTransform, Action> mapping = new Dictionary<EventTransform, Action>();

    public override void Register(Spell spell) {
        mapping.Add(EventTransform.Cast, base.Cast);
        mapping.Add(EventTransform.Hold, base.Hold);
        mapping.Add(EventTransform.Release, base.Release);
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
}
