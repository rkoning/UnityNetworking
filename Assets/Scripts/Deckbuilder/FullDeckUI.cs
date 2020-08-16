using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FullDeckUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    private SavedDeck deck;

    public void SetDeck(SavedDeck deck) {
        this.deck = deck;
        nameText.text = deck.name;
    }

    public void Edit() {

    }

    public void Delete() {
        
    }
}
