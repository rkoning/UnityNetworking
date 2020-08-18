using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string deckBuilderScene = "DeckBuilder";

    public void LoadDeckBuilder() {
        SceneManager.LoadScene(deckBuilderScene);
    }
}
