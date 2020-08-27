using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    public Deck deck;

    public List<CardUI> cardsInHand = new List<CardUI>();
    public GameObject cardPrefab;

    public Transform handPanel;
    public TMP_Text cardsInDeckText;
    public TMP_Text currentManaText;
    public TMP_Text manaRegenText;
    public TMP_Text shufflingText;
    public TMP_Text selectedText;
    public TMP_Text castingText;

    private bool initialized;


    public void Init() {
        initialized = true;
        for (int i = 0; i < deck.handSize; i++) {
            var card = Instantiate(cardPrefab, handPanel).GetComponent<CardUI>();
            card.gameObject.SetActive(false);
            cardsInHand.Add(card);
        } 
    }

    private void Update() {
        if (!initialized)
            return;

        if (deck.IsCasting()) {
            castingText.gameObject.SetActive(true);
            castingText.text = deck.castingCard.name;
        } else {
            castingText.gameObject.SetActive(false);
        }
        cardsInDeckText.text = deck.RemainingCards.ToString() + " " + deck.DeckSize.ToString();
        currentManaText.text = deck.currentMana.ToString("0.0");
        manaRegenText.text = deck.manaRegen.ToString("0.0") + "/sec";

        shufflingText.text = deck.IsShuffling ? "Shuffling..." + deck.RemainingShuffleTime.ToString("0.0") + " sec" : String.Empty;

        // TODO: Super janky way to do this
        for (int i = 0; i < cardsInHand.Count; i++) {
            if (deck.hand.Count > i) {
                cardsInHand[i].gameObject.SetActive(true);
                cardsInHand[i].SetCard(deck.hand[i], deck.hand[i].manaCost < deck.currentMana);
                if (deck.selectedIndex == i) {
                    selectedText.gameObject.SetActive(true);
                    selectedText.text = deck.hand[i].name;
                    cardsInHand[i].Selected(true);
                }
            } else {
                cardsInHand[i].gameObject.SetActive(false);
            }
        }
        if (deck.selectedIndex < 0) {
            selectedText.gameObject.SetActive(false);
        }
    }
}