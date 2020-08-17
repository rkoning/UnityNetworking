using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private Transform characterAnchor;

    public string charactersFolder = "Characters";

    private Character[] characters;
    private int currentIndex = -1;
    public Character currentCharacter;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text activeNameText;
    [SerializeField] private TMP_Text activeText;
    [SerializeField] private TMP_Text passiveNameText;
    [SerializeField] private TMP_Text passiveText;
    [SerializeField] private TMP_Text loreText;

    [SerializeField] private Deckbuilder deckBuilder;

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
        currentIndex = currentIndex % (characters.Length - 1);
        currentCharacter = characters[currentIndex];
        UpdateCharacterInfo(currentCharacter);    
    }
    

    public void ChooseCharacter() {
        var deck = new SavedDeck();
        deck.character = currentCharacter;
        deckBuilder.OpenCardPanel(deck, true);
    }
}
