using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class MainMenuController : MonoBehaviour
{
    public string deckBuilderScene = "DeckBuilder";

    public TMP_InputField serverConnection;

    public void LoadDeckBuilder() {
        SceneManager.LoadScene(deckBuilderScene);
    }

    public void TryConnect() {
        NetworkManager.singleton.networkAddress = serverConnection.text;
        NetworkManager.singleton.StartClient();       
    }
}
