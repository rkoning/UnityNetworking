using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsToDeck : SpellEffect
{
    public Card cardToAdd;
    public int numberToAdd;

    public override void Cast() {
        for (int i = 0; i < numberToAdd; i++)
            spell.owner.deck.AddCard(cardToAdd);
        spell.owner.deck.Shuffle();
    }
}
