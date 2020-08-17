using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour {

    [Header("Deck")]
    private List<DeckCard> cardsInDeck = new List<DeckCard>();
    public GameObject deckCardPrefab;
    public Transform deckContent;

    private Card[] allCards;

    private Card[] filteredCards;

    private List<FullCard> displayedCards = new List<FullCard>();

    [Header("Cards")]
    public GameObject fullCardPrefab;
    public Transform cardsContent;

    private SavedDeck currentDeck;

    public string cardsFolder = "Cards";

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField deckName;

    private void OnEnable() {
        LoadAllCards();
    }

    private void OnDisable() {
        for (int i = 0; i < displayedCards.Count; i++) {
            Destroy(displayedCards[i].gameObject);
        }
        displayedCards.Clear();
        for (int i = 0; i < cardsInDeck.Count; i++) {
            Destroy(cardsInDeck[i].gameObject);
        }
        cardsInDeck.Clear();
    }

    public void SetCurrentDeck(SavedDeck deck, bool isNew) {

        if (!isNew) {
            deckName.interactable = false;
        }
        this.currentDeck = deck;
        deckName.text = deck.name;
        PopulateCards(currentDeck.character.lores);
        for (int i = 0; i < deck.cards.Count; i++) {
            var card = Instantiate(deckCardPrefab, deckContent).GetComponent<DeckCard>();
            card.SetCard(deck.cards[i], this);
            cardsInDeck.Add(card);
        }
    }

    public void PopulateCards(Lore[] lores) {
        filteredCards = allCards.Where((Card c) => lores.Contains(c.lore)).ToArray();

        displayedCards = new List<FullCard>();
        for (int i = 0; i < filteredCards.Length; i++) {
            var dc = Instantiate(fullCardPrefab, cardsContent).GetComponent<FullCard>();
            dc.SetCard(filteredCards[i], this);
            displayedCards.Add(dc);
        }
    }

    public void LoadAllCards() {
        allCards = Resources.LoadAll<Card>(cardsFolder);
    }

    public void AddCardToDeck(Card card) {
        var deckCard = Instantiate(deckCardPrefab, deckContent).GetComponent<DeckCard>();
        deckCard.SetCard(card, this);
        cardsInDeck.Add(deckCard);

        // TODO: Check if the card that was clicked should be locked
    }

    public void RemoveCardFromDeck(DeckCard deckCard) {
        cardsInDeck.Remove(deckCard);
        Destroy(deckCard.gameObject);
    }

    public void SetDeckName(string name) {
        currentDeck.name = name;
    }

    public void Save() {
        currentDeck.cards = new List<Card>();
        foreach(var deckCard in cardsInDeck) {
            currentDeck.cards.Add(deckCard.GetCard());
        }
        currentDeck.Save();
    }
}