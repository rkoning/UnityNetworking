using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// [System.Serializable]
public class SavedDeck
{
    public string name;
    private string fileName;
    public List<Card> cards = new List<Card>();
    public Character character;

    private static string deckDir = $"{Application.persistentDataPath}/Decks";

    public SavedDeck() {}
    public SavedDeck(string fileName) {
        this.fileName = fileName;
    }

    public void Save() {
        if (name == null) {
            return;
        }
        SaveToJSON();
    }

    public void Load() {
        if (fileName == null) {
            return;
        }
        LoadFromJSON();
    }

    
    public void Delete() {
        if (fileName == null) {
            return;
        }

        System.IO.File.Delete($"{deckDir}/{fileName}");
    }

    private void LoadFromJSON() {
        string deckJson = System.IO.File.ReadAllText($"{deckDir}/{fileName}");
        SavedDeck loaded = SavedDeck.LoadFromString(deckJson);
        this.name = loaded.name;
        this.character = loaded.character;
        this.cards = new List<Card>(loaded.cards);
    }

    private void SaveToJSON() {
        DeckData data = new DeckData(name, character, cards);
        string json = JsonUtility.ToJson(data, true);
        if (fileName == null) {
            fileName = $"{System.Guid.NewGuid()}.json";
        }
        System.IO.File.WriteAllText($"{deckDir}/{fileName}", json);
    }

    public static SavedDeck LoadFromString(string deckJson) {
        DeckData data = JsonUtility.FromJson<DeckData>(deckJson);
        SavedDeck deck = new SavedDeck();
        deck.name = data.name;
        deck.character = data.GetCharacter();
        deck.cards = data.GetCards();
        return deck;
    }

    public override string ToString() {
        DeckData data = new DeckData(name, character, cards);
        var str = JsonUtility.ToJson(data, false);
        return str;
    }

    /// <summary>
    /// Gets a list of the decks that are loaded in the user's LocalLow folder
    /// </summary>
    /// <returns>All decks that were found locally</returns>
    public static SavedDeck[] LoadLocalDecks() {
        if (!System.IO.Directory.Exists(deckDir)) {
            System.IO.Directory.CreateDirectory(deckDir);
        }
        
        string[] deckFiles = System.IO.Directory.GetFiles(deckDir);
        SavedDeck[] decks  = new SavedDeck[deckFiles.Length];

        for (int i = 0; i < deckFiles.Length; i++) {
            string fileName = deckFiles[i].Substring(deckFiles[i].LastIndexOf("\\") + 1);
            decks[i] = new SavedDeck(fileName);
            decks[i].Load();
        }
        return decks;
    }
}

[System.Serializable]
public class DeckData {
    public string name;
    public int characterId;
    public int[] cardIds;

    public DeckData(string name, Character character, List<Card> cards) {
        this.name = name;
        this.characterId = character.id;
        this.cardIds = cards.Select((card) => card.id).ToArray();
    }

    public Character GetCharacter() {
        var characters = Resources.LoadAll<Character>("Characters");
        return Array.Find(characters, (c) => c.id == characterId);
    }

    public List<Card> GetCards() {
        var cards = Resources.LoadAll<Card>("Cards");
        var deckCards = new List<Card>();
        for (int i = 0; i < cardIds.Length; i++) {
            var card = System.Array.Find(cards, (c) => c.id == cardIds[i]);
            if (card)
                deckCards.Add(card);
        }

        return deckCards;
    }
}
