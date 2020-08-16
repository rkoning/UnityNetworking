using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderUI : MonoBehaviour
{


    public List<FullDeckUI> displayedDecks = new List<FullDeckUI>();

    [SerializeField] private GameObject cardPanel;
    [SerializeField] private GameObject deckPanel;

    public void OpenCardPanel(Deck deck) {
        cardPanel.SetActive(true);
        deckPanel.SetActive(false);
    }

    public void OpenDeckPanel() {
        cardPanel.SetActive(false);
        deckPanel.SetActive(true);
    }
}
