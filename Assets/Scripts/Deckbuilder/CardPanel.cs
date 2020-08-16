using UnityEngine;
using System.Collections.Generic;

public class CardPanel : MonoBehaviour {

    private List<FullCardUI> displayedCards = new List<FullCardUI>();
    public GameObject cardPrefab;
    public Transform cardsContent;

    private Deck currentDeck;

    public void SetCurrentDeck(Deck deck) {
        this.currentDeck = deck;
        for (int i = 0; i < deck.cards.Count; i++) {
            var card = Instantiate(cardPrefab, cardsContent).GetComponent<FullCardUI>();
            card.SetCard(deck.cards[i]);
            displayedCards.Add(card);
        }
    }

    public void Save() {

    }

    public void Exit() {

    } 

    public void SelectCard() {

    }
}