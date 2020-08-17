using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    public List<FullDeck> displayedDecks = new List<FullDeck>();
    public GameObject deckPrefab;

    [SerializeField] private Deckbuilder deckbuilder;
    [SerializeField] private Transform decksContent;

    private SavedDeck[] decks;

    private void OnEnable() {
        LoadDecks();
    }

    public void LoadDecks() {
        try {
            string deckDir = $"{Application.persistentDataPath}/Decks";
            if (!System.IO.Directory.Exists(deckDir)) {
                System.IO.Directory.CreateDirectory(deckDir);
            }
            
            string[] deckFiles = System.IO.Directory.GetFiles(deckDir);
            decks  = new SavedDeck[deckFiles.Length];
            for (int i = 0; i < deckFiles.Length; i++) {
                Debug.Log(deckFiles[i]);
                string fileName = deckFiles[i].Substring(deckFiles[i].LastIndexOf("\\") + 1);
                Debug.Log(fileName);
                decks[i] = new SavedDeck(fileName);
                decks[i].Load();

                var deck = Instantiate(deckPrefab, decksContent).GetComponent<FullDeck>();
                deck.SetDeck(decks[i], this);
                displayedDecks.Add(deck);
            }


        } catch (Exception err) {
            Debug.Log(err);
            // TODO: send some feedback to the client
        }
    }

    public void NewDeck() {
        deckbuilder.OpenCardPanel(new SavedDeck());
    }

    public void DeleteDeck(GameObject deckUI, SavedDeck deck) {
        deck.Delete();
        Destroy(deckUI);      
    }

    public void EditDeck(SavedDeck deck) {
        deckbuilder.OpenCardPanel(deck);
    }
}
