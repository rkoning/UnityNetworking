using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deckbuilder : MonoBehaviour
{

    [SerializeField] private CardPanel cardPanel;
    [SerializeField] private DeckPanel deckPanel;

    private void Start() {
        OpenDeckPanel();
    }

    public void OpenCardPanel(SavedDeck deck) {
        cardPanel.gameObject.SetActive(true);
        deckPanel.gameObject.SetActive(false);

        cardPanel.SetCurrentDeck(deck);
    }

    public void OpenDeckPanel() {
        cardPanel.gameObject.SetActive(false);
        deckPanel.gameObject.SetActive(true);
    }
}
