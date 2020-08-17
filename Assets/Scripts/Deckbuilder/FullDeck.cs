using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FullDeck : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button editButton;
    [SerializeField] private Button deleteButton;
    
    public void SetDeck(SavedDeck deck, DeckPanel deckPanel) {
        nameText.text = deck.name;

        editButton.onClick.AddListener(() => deckPanel.EditDeck(deck));
        editButton.onClick.AddListener(() => deckPanel.DeleteDeck(gameObject, deck));
    }
}
