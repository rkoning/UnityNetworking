using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGameObject : SpellEffect
{
    public GameObject toActivate;

    private float deactivateAfter = 5f;

    public override void Register(Spell spell) {
        toActivate.SetActive(false);
        base.Register(spell);
    }

    public override void Cast() {
        toActivate.SetActive(true);
        StartCoroutine(WaitThenDo(deactivateAfter, () => toActivate.SetActive(false)));
    }
}   
