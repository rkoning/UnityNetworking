using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;

public class Deck : MonoBehaviour {

    public Transform castTransform;
    
    [Header("Mana")]
    public float maxMana = 5f;
    public float currentMana;
    public float manaRegen = 0.5f;

    [Header("Deck")]
    public List<Card> cards;
    public int currentCard;

    public int DeckSize {
        get {
            return cards.Count;
        }
    }

    public int RemainingCards {
        get {
            return currentCard < DeckSize ? DeckSize - currentCard : 0;
        }
    }

    [Header("Hand")]
    public List<Card> hand = new List<Card>();
    public int handSize = 5;
    public int initialHandSize = 3;
    public float drawDelay;
    private float nextDraw;

    private Avatar avatar;

    [Header("Shuffle")]
    public float shuffleTime;
    public float RemainingShuffleTime { get; private set; }

    private Coroutine shuffling;

    public bool IsShuffling { get; private set; }

    public bool IsAnchored { get; private set; }
    public bool IsBlocked { get; private set; }

    private Spell currentSpell;
    public Card castingCard;

    public Card selectedCard;
    public int selectedIndex = -1;

    private void Start() {
        avatar = GetComponent<Avatar>();
    }
    
    public void LoadCards(List<Card> cards) {
        avatar = GetComponent<Avatar>();
        avatar.deck = this;
        this.cards = cards;

        if (!avatar.gamePlayer.hasAuthority) {
            return;
        }
        FullShuffle();
        // Draw up to handSize initially
        Draw(initialHandSize);
    }

    private void Update() {
        if (!avatar.initialized || !avatar.gamePlayer.hasAuthority) {
            return;
        }

        if (currentSpell && currentSpell.Done()) {
            currentSpell.Despawn();
            castingCard = null;
            currentSpell = null;
            IsAnchored = false;
            IsBlocked = false;
        }

        float dT = Time.deltaTime;
        currentMana += manaRegen * dT;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        if (IsShuffling) {
            RemainingShuffleTime -= dT;
            return;
        }

        if (hand.Count < handSize) {
            nextDraw += dT;
            if (nextDraw > drawDelay) {
                nextDraw = 0;
                Draw();
            }
        }
    }

    public void Draw(int numCards) { 
        for (int i = 0; i < numCards; i++) {
            Draw();
        }
    }

    public void Draw() {
        if (IsShuffling || hand.Count > handSize) {
            return;
        }
        if (currentCard >= DeckSize) {
            if (hand.Count < 1)  // Auto Shuffle if the deck and hand are empty
                StartShuffle();
            return;
        }
        Card c = cards[currentCard];
        hand.Add(c);
        currentCard++;
    }

    public void AddCard(Card card) {
        cards.Add(card);
        avatar.gamePlayer.RegisterPrefab(card.spellPrefab, 1);
    }

    public void AddCard(Card card, int index) {
        cards.Insert(index, card);
        avatar.gamePlayer.RegisterPrefab(card.spellPrefab, 1);
    }

    public void StartShuffle() {
        if (!IsShuffling) {
            IsShuffling = true;
            hand.Clear();
            RemainingShuffleTime = shuffleTime;
            shuffling = StartCoroutine(WaitThenShuffle(shuffleTime));
        }
    }

    private IEnumerator WaitThenShuffle(float delay) {
        yield return new WaitForSeconds(delay);
        FullShuffle();
        currentCard = 0;
        shuffling = null;
        IsShuffling = false;
        Draw(initialHandSize);
    }

    public void Shuffle(int current) {
        int n = current + 1;
        RNGCryptoServiceProvider rngesus = new RNGCryptoServiceProvider();
        while (n < cards.Count - 1) {
            byte[] box = new byte[1];
            do rngesus.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = box[0] % n;
            n++;
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }
    
    /// <summary>
    /// Shuffles the remaining cards in the deck
    /// </summary>
    public void Shuffle() => Shuffle(currentCard);
    
    /// <summary>
    /// Shuffles all cards in the deck
    /// </summary>
    public void FullShuffle() => Shuffle(0);

    public void Cast() {
        // Blocking spells prevent additional casts during their duration
        if (selectedCard == null || (currentSpell && IsBlocked)) {
            return;
        }

        if (currentMana < selectedCard.manaCost) {
            return;
            // TODO: play fizzle sound
        }

        currentMana -= selectedCard.manaCost;
        string name = selectedCard.spellPrefab.GetComponent<NetworkIdentity>().name;
        hand.Remove(selectedCard);
        castingCard = selectedCard;
        avatar.gamePlayer.Cast(name);

        if (selectedCard.consume) {
            cards.Remove(selectedCard);
        }
        selectedIndex = -1;
        selectedCard = null;
    }

    public void AfterCast(Spell spell) {
        spell.owner = avatar;
        spell.Init();
        spell.Cast();
        currentSpell = spell;
        IsAnchored = spell.anchorForDuration;
        IsBlocked = spell.blockForDuration;
    }

    public void Hold() {
        if (currentSpell) {
            currentSpell.Hold();
        }
    }

    public void Release() {
        if (currentSpell) {
            currentSpell.Release();
        }
    }

    public void Cancel() {
        selectedIndex = -1;
        selectedCard = null;
    }

    public bool IsCasting() {
        return currentSpell != null;
    }


    public void SelectSpell(int index) {
        if (hand.Count <= index)
            return;
        selectedIndex = index;
        selectedCard = hand[index];
    }


}
