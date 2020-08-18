using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBuilder : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    [SerializeField] private CardPanel cardPanel;
    [SerializeField] private DeckPanel deckPanel;
    [SerializeField] private CharacterPanel characterPanel;


    private void Start() {
        OpenDeckPanel();
    }

    public void OpenCardPanel(SavedDeck deck, bool isNew) {
        cardPanel.gameObject.SetActive(true);
        deckPanel.gameObject.SetActive(false);
        characterPanel.gameObject.SetActive(false);
        cardPanel.SetCurrentDeck(deck, isNew);
    }

    public void OpenDeckPanel() {
        cardPanel.gameObject.SetActive(false);
        deckPanel.gameObject.SetActive(true);
        characterPanel.gameObject.SetActive(false);
    }

    public void OpenCharacterPanel() {
        cardPanel.gameObject.SetActive(false);
        deckPanel.gameObject.SetActive(false);
        characterPanel.gameObject.SetActive(true);
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(mainMenuScene);
    }
}
