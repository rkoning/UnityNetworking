using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterPanel : MonoBehaviour
{
    public Transform characterAnchor;

    public string charactersFolder = "Characters";

    private Character[] characters;
    private int currentIndex = -1;
    public Character currentCharacter;

    public TMP_Text nameText;
    public TMP_Text activeNameText;
    public TMP_Text activeText;
    public TMP_Text passiveNameText;
    public TMP_Text passiveText;
    public TMP_Text loreText;

    public DeckBuilder deckBuilder;

    private void OnEnable() {
        LoadAllCharacters();
        NextCharacter();
    }

    public void LoadAllCharacters() {
        characters = Resources.LoadAll<Character>(charactersFolder);
    }

    private void UpdateCharacterInfo(Character character) {
        nameText.text = character.name;
        activeNameText.text = character.activeName;
        activeText.text = character.activeDescription;
        passiveNameText.text = character.passiveName;
        passiveText.text = character.passiveDescription;
        string lores = "";
        foreach (var lore in character.lores) {
            lores += $"{lore.ToString()}, ";
        }
        loreText.text = lores.Substring(0, lores.Length - 2);
    }

    public void NextCharacter() {
        currentIndex++;
        currentIndex = currentIndex % (characters.Length);
        currentCharacter = characters[currentIndex];
        UpdateCharacterInfo(currentCharacter);    
    }

    public void PreviousCharacter() {
        currentIndex--;
        currentIndex = currentIndex % (characters.Length);
        currentCharacter = characters[currentIndex];
        UpdateCharacterInfo(currentCharacter);    
    }
    

    public void ChooseCharacter() {
        var deck = new SavedDeck();
        deck.character = currentCharacter;
        deckBuilder.OpenCardPanel(deck, true);
    }
}
