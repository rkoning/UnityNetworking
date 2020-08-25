using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour {

    public Transform castTransform;
    
    [Header("Mana")]
    public float maxMana = 5f;
    public float currentMana;
    public float manaRegen = 0.5f;

    [Header("Deck")]
    public List<Card> cards;
    public int currentCard;
    private int deckSize;

    public int DeckSize {
        get {
            return deckSize;
        }
    }

    public int RemainingCards {
        get {
            return deckSize - currentCard;
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

    private Spell currentSpell;

    public void LoadCards(List<Card> cards) {
        avatar = GetComponent<Avatar>();
        avatar.deck = this;
        this.cards = cards;
        deckSize = this.cards.Count;
        for (int i = 0; i < deckSize; i++) {
            avatar.gamePlayer.RegisterPrefab(this.cards[i].spellPrefab, 10);
        }
        if (!hasAuthority) {
            return;
        }
        Shuffle();
        // Draw up to handSize initially
        Draw(initialHandSize);
    }

    private void Update() {
        if (!hasAuthority) {
            return;
        }

        if (currentSpell && currentSpell.Done()) {
            Debug.Log("Spell Done");
            currentSpell = null;
            IsAnchored = false;
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
        if (currentCard >= deckSize) {
            if (hand.Count < 1)  // Auto Shuffle if the deck and hand are empty
                StartShuffle();
            return;
        }
        Card c = cards[currentCard];
        hand.Add(c);
        currentCard++;
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
        Shuffle();
        currentCard = 0;
        shuffling = null;
        IsShuffling = false;
        Draw(initialHandSize);
    }

    public void Shuffle() {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = cards.Count;
        while (n > 1) {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = box[0] % n;
            n--;
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public void Cast(int handIndex) {
        if (currentSpell) {
            return;
        }

        if (hand.Count <= handIndex || currentMana < hand[handIndex].manaCost) {
            return;
            // TODO: play fizzle sound
        }

        currentMana -= hand[handIndex].manaCost;
        string name = hand[handIndex].spellPrefab.GetComponent<NetworkIdentity>().name;
        hand.RemoveAt(handIndex);
        CmdCast(name);
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

    [Command]
    private void CmdCast(string name) {
        var go = ObjectPool.singleton.GetFromPool(name, castTransform.position, castTransform.rotation);
        var netId = go.GetComponent<NetworkIdentity>();
        netId.AssignClientAuthority(connectionToClient);

        RpcCast(netId.netId); 
    }

    [ClientRpc]
    private void RpcCast(uint netId)
    {
        Spell spell = ObjectPool.singleton.spawnedObjects[netId].GetComponent<Spell>();
        spell.owner = avatar;
        spell.Cast();
        currentSpell = spell;
        IsAnchored = spell.anchorForDuration;
        StartCoroutine(Destroy(spell.gameObject, 10.0f));
    }

    private IEnumerator Destroy(GameObject go, float after) {
        yield return new WaitForSeconds(after);
        ObjectPool.singleton.UnSpawnObject(go);
        NetworkServer.UnSpawn(go);
    }
}
