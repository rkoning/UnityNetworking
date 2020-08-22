using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FullDeck : MonoBehaviour
{
    public TMP_Text nameText;
    public Button editButton;
    public Button deleteButton;
    
    public void SetDeck(SavedDeck deck, DeckPanel deckPanel) {
        nameText.text = deck.name;

        editButton.onClick.AddListener(() => deckPanel.EditDeck(deck));
        deleteButton.onClick.AddListener(() => deckPanel.DeleteDeck(gameObject, deck));
    }
}
