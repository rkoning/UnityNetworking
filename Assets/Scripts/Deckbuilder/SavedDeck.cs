using System.Collections.Generic;
using UnityEngine;

public class SavedDeck
{
    public string name;
    private string fileName;
    public List<Card> cards = new List<Card>();
    public Character character;

    public SavedDeck() {}
    public SavedDeck(string fileName) {
        this.fileName = fileName;
    }

    public void Save() {
        if (name == null) {
            return;
        }
        fileName = $"{name}.json";
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

        System.IO.File.Delete($"{Application.persistentDataPath}/Decks/{fileName}");
    }

    private void LoadFromJSON() {
        string deckJson = System.IO.File.ReadAllText($"{Application.persistentDataPath}/Decks/{fileName}");
        DeckData data = JsonUtility.FromJson<DeckData>(deckJson);
        this.name = data.name;
        this.character = data.character;
        this.cards = new List<Card>(data.cards);
    }

    private void SaveToJSON() {
        DeckData data = new DeckData(name, character, cards.ToArray());
        string json = JsonUtility.ToJson(data, true);
        // TODO: Parse out any ../ and catch execptions
        System.IO.File.WriteAllText($"{Application.persistentDataPath}/Decks/{fileName}", json);
    }
}

[System.Serializable]
public class DeckData {
    public string name;
    public Character character;
    public Card[] cards;

    public DeckData(string name, Character character, Card[] cards) {
        this.name = name;
        this.character = character;
        this.cards = cards;
    }
}
