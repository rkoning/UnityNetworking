using UnityEngine;
using UnityEngine.Events;

public abstract class SpellEffect : MonoBehaviour {
    public Spell spell;
    public SpellEffect parent;

    public delegate void CastAction();
    public event CastAction OnCast;
    public event CastAction OnHold;
    public event CastAction OnRelease;
    public delegate void HitAnyAction(GameObject other);
    public event HitAnyAction OnHitAny;

    public delegate void HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;

    /// <summary>
    /// Called by the spell that this effect is on when the spell is instantiated,
    /// Registers callbacks for all spell events.
    /// If this effect has a parent effect, events are registered to the parent instead.
    /// </summary>
    /// <param name="spell"></param>
    public void Register(Spell spell)
    {
        this.spell = spell;
        OnCast += () => { };
        OnHold += () => { };
        OnRelease += () => { };
        OnHitAny += (GameObject other) => { };
        OnHitHealth += (Health t) => { };

        if (parent)
        {
            parent.OnCast += Cast;
            parent.OnHold += Hold;
            parent.OnRelease += Release;
            parent.OnHitAny += HitAny;
            parent.OnHitHealth += HitHealth;
        }
        else
        {
            spell.OnCast += Cast;
            spell.OnHold += Hold;
            spell.OnRelease += Release;
            spell.OnHitAny += HitAny;
            spell.OnHitHealth += HitHealth;
        }
    }

    public virtual void Cast()
    {
        OnCast();
    }
    public virtual void Hold()
    {
        OnHold();
    }
    public virtual void Release()
    {
        OnRelease();
    }
    public virtual void HitAny(GameObject other)
    {
        OnHitAny(other);
    }
    public virtual void HitHealth(Health target)
    {
        OnHitHealth(target);
    }
}
