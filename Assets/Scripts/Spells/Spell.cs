using UnityEngine;

public class Spell : MonoBehaviour, IPoolableObject
{
    public Avatar owner;
    public delegate void CastAction();
    public event CastAction OnCast;
    public event CastAction OnHold;
    public event CastAction OnRelease;

    public delegate void HitAnyAction(GameObject other);
    public event HitAnyAction OnHitAny;

    public delegate void HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;

    private float duration;
    public bool anchorForDuration;

    private float spellEnd;

    public bool parentToCaster;

    public void Init()
    {
        // Register event defaults
        OnCast += () => { };

        OnHold += () => { };

        OnRelease += () => { };

        OnHitAny += (GameObject other) => { };
        OnHitHealth += (Health target) => { };

        // Register all spell effects to effect events
        foreach(var effect in GetComponents<SpellEffect>())
        {
            effect.Register(this);
            duration += effect.duration;
        }
    }

    public void Cast()
    {
        spellEnd = Time.fixedTime + duration;
        if (parentToCaster) {
            transform.SetParent(owner.deck.castTransform);
        }
        OnCast();
    }

    public void Hold()
    {
        OnHold();
    }

    public void Release()
    {
        OnRelease();
    }

    public void HitAny(GameObject other)
    {
        OnHitAny(other);
    }

    public void HitHealth(Health target)
    {
        OnHitHealth(target);
    }

    public bool Done() {
        return Time.fixedTime > spellEnd;
    }
}
