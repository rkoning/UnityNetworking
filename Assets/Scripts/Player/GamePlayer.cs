using UnityEngine;
using Mirror;
using System.Collections;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class GamePlayer : NetworkBehaviour
{
    [SyncVar]
    public string displayName = "Loading...";

    private RoomManager room;
    public RoomManager Room {
        get {
           if (room != null) return room;
           return room = NetworkManager.singleton as RoomManager;
        }
    }

    [Header("Character")]
    public CharacterBuild build;

    private Avatar avatar;

    [Header("UI")]
    public GameObject hudUI;
    [SerializeField] private DeckUI deckUI;

    #region Start & Stop Callbacks

    public override void OnStartClient() {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
    }

    public override void OnStartAuthority() {
        CmdSpawnAvatar();
        hudUI.SetActive(true);
    }

    public override void OnStopClient() {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName) {
        this.displayName = displayName;
    }

    [Command]
    public void CmdSpawnAvatar() {
        var spawnPoint = NetworkManager.singleton.GetStartPosition();
        var avatarGameObject = GameObject.Instantiate(build.character.avatarPrefab, spawnPoint.position, spawnPoint.rotation);
        avatarGameObject.GetComponent<Health>().gamePlayer = this;
        avatarGameObject.GetComponent<Health>().connectionId = connectionToClient.connectionId;
        NetworkServer.Spawn(avatarGameObject, connectionToClient); // Give player authority over avatar object
        RpcSpawnAvatar(avatarGameObject.GetComponent<NetworkIdentity>().netId, connectionToClient.connectionId);
    }

    [ClientRpc]
    public void RpcSpawnAvatar(uint avatarId, int connectionId) {
        var healths = FindObjectsOfType<Health>();
        foreach (var health in healths) {
            if (health.GetComponent<NetworkIdentity>().netId == avatarId) {
                health.gamePlayer = this;
                avatar = health.GetComponent<Avatar>();
                deckUI.deck = health.GetComponent<Deck>();
                deckUI.Init();
            }
        }
    }

    #endregion
}