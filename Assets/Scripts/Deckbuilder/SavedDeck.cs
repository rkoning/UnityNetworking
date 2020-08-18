using System.Collections.Generic;
using UnityEngine;

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
        DeckData data = JsonUtility.FromJson<DeckData>(deckJson);
        this.name = data.name;
        this.character = data.character;
        this.cards = new List<Card>(data.cards);
    }

    private void SaveToJSON() {
        DeckData data = new DeckData(name, character, cards.ToArray());
        string json = JsonUtility.ToJson(data, true);
        if (fileName == null) {
            fileName = $"{System.Guid.NewGuid()}.json";
        }
        System.IO.File.WriteAllText($"{deckDir}/{fileName}", json);
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
    public Character character;
    public Card[] cards;

    public DeckData(string name, Character character, Card[] cards) {
        this.name = name;
        this.character = character;
        this.cards = cards;
    }
}
