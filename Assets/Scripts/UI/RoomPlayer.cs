using System.Reflection;
using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkRoomPlayer.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomPlayer.html
*/

/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
/// The RoomPrefab object of the NetworkRoomManager must have this component on it.
/// This component holds basic room player data required for the room to function.
/// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
/// </summary>
public class RoomPlayer : NetworkRoomPlayer
{
    [Header("Characters")]
    [SerializeField] private CharacterBuild[] availableCharacters;
    private CharacterBuild selectedCharacter;

    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Dropdown characterDropdown;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader {
        set {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private RoomManager room;
    public RoomManager Room {
        get {
           if (room != null) return room;
           return room = NetworkManager.singleton as RoomManager;
        }
    }

    public override void OnStartAuthority() {
        CmdSetDisplayName(Random.Range(0, 100).ToString());
        lobbyUI.SetActive(true);
        selectedCharacter = availableCharacters[0];
    }

    public void CloseLobbyUI() {
        lobbyUI.SetActive(false);
    }

    public override void OnStartClient() {
        Room.RoomPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnNetworkDestroy() {
        Room.RoomPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay() {
        if (!hasAuthority) {
            foreach (var player in Room.RoomPlayers) {
                if (player.hasAuthority) {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++) {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ? 
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }

        var options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < availableCharacters.Length; i++) {
            options.Add(new TMP_Dropdown.OptionData(availableCharacters[i].character.name));
        }
        characterDropdown.options = options;
    }

    public void SelectCharacterBuild(int index) {
        selectedCharacter = availableCharacters[index];
    }

    public CharacterBuild GetSelectedBuild() {
        return selectedCharacter;
    }

    public void HandleReadyToStart(bool readyToStart) {
        if (!isLeader) return;

        startGameButton.interactable = readyToStart;
    }

    public void Replace(GameObject gamePlayer) {
        gamePlayer.GetComponent<GamePlayer>().SetDisplayName(DisplayName);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, gamePlayer);
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSetDisplayName(string displayName) {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp() {
        IsReady = !IsReady;
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame() {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return;

        Room.StartGame();
    }

    public override void OnGUI() { }
}