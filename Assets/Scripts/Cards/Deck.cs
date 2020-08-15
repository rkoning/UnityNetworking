﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour {

    [Header("Deck")]
    public List<Card> cards;
    public int currentCard;
    private int deckSize;

    [Header("Hand")]
    public List<Card> hand = new List<Card>();
    public int handSize = 5;
    public int initialHandSize = 3;
    public float drawDelay;
    private float nextDraw;

    private Avatar avatar;

    [Header("Shuffle")]
    public float shuffleTime;
    private Coroutine shuffling;

    public bool IsShuffling { get; private set; }

    private void Start() {

        avatar = GetComponent<Avatar>();
        avatar.deck = this;
        deckSize = cards.Count;
        for (int i = 0; i < deckSize; i++) {
            ObjectPool.singleton.RegisterPrefab(cards[i].spellPrefab.gameObject, 10);
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
        if (IsShuffling) {
            return;
        }

        if (hand.Count < handSize) {
            nextDraw += Time.deltaTime;
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
        Debug.Log("Casting at: " + handIndex);
        if (hand.Count <= handIndex) {
            return;
            // TODO: play fizzle sound
        }
        System.Guid spellAssetId = hand[handIndex].spellPrefab.GetComponent<NetworkIdentity>().assetId;
        hand.RemoveAt(handIndex);
        CmdCast(spellAssetId);
    }

    [Command]
    private void CmdCast(System.Guid spellAssetId) {
        // if (handIndex >= hand.Count)
        //     return;
        // System.Guid spellAssetId = hand[handIndex].spellPrefab.GetComponent<NetworkIdentity>().assetId;
        RpcCast(spellAssetId); 
    }

    [ClientRpc]
    private void RpcCast(System.Guid assetId)
    {
        var spell = ObjectPool.singleton.GetFromPool(assetId, transform.position + transform.forward, transform.rotation).GetComponent<Spell>();
        spell.owner = avatar;
        spell.Cast();
        StartCoroutine(Destroy(spell.gameObject, 2.0f));
    }

    private IEnumerator Destroy(GameObject go, float after) {
        yield return new WaitForSeconds(after);
        ObjectPool.singleton.UnSpawnObject(go);
        NetworkServer.UnSpawn(go);
    }
}