using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    public List<FullDeckUI> displayedDecks = new List<FullDeckUI>();
    private DeckbuilderUI deckbuilder;

    [SerializeField] private Transform decksContent;

    public void LoadDecks() {

    }

    public void NewDeck() {
        var deckGameObject = new GameObject("New Deck");
        deckGameObject.AddComponent<Deck>();
    }
}
