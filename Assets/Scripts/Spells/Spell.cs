using UnityEngine;

public class Spell : MonoBehaviour
{
    public Avatar owner;
    public delegate void CastAction();
    public event CastAction OnCast;
    public event CastAction OnHold;
    public event CastAction OnRelease;

    public delegate void HitAnyAction(GameObject other);
    public event HitAnyAction OnHitAny;

    public delegate void HitTargetAction(Target target);
    public event HitTargetAction OnHitTarget;

    private void Start()
    {
        // Register event defaults
        OnCast += () => { };

        OnHold += () => { };

        OnRelease += () => { };

        OnHitAny += (GameObject other) => { };
        OnHitTarget += (Target target) => { };

        // Register all spell effects to effect events
        foreach(var effect in GetComponents<SpellEffect>())
        {
            effect.Register(this);
        }
    }

    public void Cast()
    {
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

    public void HitTarget(Target target)
    {
        OnHitTarget(target);
    }
}
