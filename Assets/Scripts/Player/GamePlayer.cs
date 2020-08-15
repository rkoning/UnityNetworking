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

    public CharacterBuild build;

    private Avatar avatar;

    #region Start & Stop Callbacks

    public override void OnStartClient() {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
    }

    public override void OnNetworkDestroy() {
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
        NetworkServer.Spawn(avatarGameObject, connectionToClient); // Give player authority over avatar object
        RpcSpawnAvatar();
    }

    [ClientRpc]
    public void RpcSpawnAvatar() {
        var healths = FindObjectsOfType<Health>();
        foreach (var health in healths) {
            if (health.GetComponent<NetworkIdentity>().hasAuthority) {
                health.gamePlayer = this;
                avatar = health.GetComponent<Avatar>();
            }
        }
    }

    [Command]
    public void CmdStartRespawn() {
        NetworkServer.Destroy(avatar.gameObject);
        StartCoroutine(WaitThenRespawn(3f));
    }

    private IEnumerator WaitThenRespawn(float duration) {
        yield return new WaitForSeconds(duration);
        CmdSpawnAvatar();
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() { 
        // CmdSpawnAvatar();
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() {
        CmdSpawnAvatar();
    }

    private void Start() {
    }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}