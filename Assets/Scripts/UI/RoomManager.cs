using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkRoomManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomManager.html

	See Also: NetworkManager
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

/// <summary>
/// This is a specialized NetworkManager that includes a networked room.
/// The room has slots that track the joined players, and a maximum player count that is enforced.
/// It requires that the NetworkRoomPlayer component be on the room player objects.
/// NetworkRoomManager is derived from NetworkManager, and so it implements many of the virtual functions provided by the NetworkManager class.
/// </summary>
public class RoomManager : NetworkRoomManager
{

    public List<RoomPlayer> RoomPlayers { get; } = new List<RoomPlayer>();
    public List<GamePlayer> GamePlayers = new List<GamePlayer>();

    private bool showStartButton;

    public override void OnRoomStopServer() {
        RoomPlayers.Clear();
        // // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // // going to the offline scene to avoid collision with the one that lives there.
        // if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
        //     SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        // base.OnRoomStopServer();
    }

    /// <summary>
    /// This is called on the server when a client disconnects.
    /// </summary>
    /// <param name="conn">The connection that disconnected.</param>
    public override void OnRoomServerDisconnect(NetworkConnection conn) { 
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<RoomPlayer>();
            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }
        base.OnRoomServerDisconnect(conn);
    }

    public void NotifyPlayersOfReadyState() {
        foreach (var player in RoomPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart() {
        if (numPlayers < minPlayers) { return false; }

        foreach(var player in RoomPlayers) {
            if (!player.IsReady) { return false; }
        }
        return true;
    }

    /// <summary>
    /// This allows customization of the creation of the room-player object on the server.
    /// <para>By default the roomPlayerPrefab is used to create the room-player, but this function allows that behaviour to be customized.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    /// <returns>The new room-player object.</returns>
    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
    {
        return base.OnRoomServerCreateRoomPlayer(conn);
    }

    /// <summary>
    /// This allows customization of the creation of the GamePlayer object on the server.
    /// <para>By default the gamePlayerPrefab is used to create the game-player, but this function allows that behaviour to be customized. The object returned from the function will be used to replace the room-player on the connection.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    /// <param name="roomPlayer">The room player object for this connection.</param>
    /// <returns>A new GamePlayer object.</returns>
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        return base.OnRoomServerCreateGamePlayer(conn, roomPlayer);
    }

    /// <summary>
    /// This allows customization of the creation of the GamePlayer object on the server.
    /// <para>This is only called for subsequent GamePlay scenes after the first one.</para>
    /// <para>See OnRoomServerCreateGamePlayer to customize the player object for the initial GamePlay scene.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    public override void OnRoomServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == RoomScene) {
            bool isLeader = RoomPlayers.Count == 0;

            RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab) as RoomPlayer;
            roomPlayer.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        }
    }

    /// <summary>
    /// This is called on the server when it is told that a client has finished switching from the room scene to a game player scene.
    /// <para>When switching from the room, the room-player is replaced with a game-player object. This callback function gives an opportunity to apply state from the room-player to the game-player object.</para>
    /// </summary>
    /// <param name="conn">The connection of the player</param>
    /// <param name="roomPlayer">The room player object.</param>
    /// <param name="gamePlayer">The game player object.</param>
    /// <returns>False to not allow this player to replace the room player.</returns>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        // var gamePlayerInstance = Instantiate(gamePlayer);
        // roomPlayer.GetComponent<RoomPlayer>().Replace(gamePlayer);
        // for (int i = 0; i < RoomPlayers.Count; i++) {
        //     var conn = RoomPlayers[i].connectionToClient;
        //     var roomPlayer = conn.identity.gameObject;
        //     NetworkServer.Destroy(roomPlayer);
        // }
        // PlayerScore score = gamePlayer.GetComponent<PlayerScore>();
        // var player = roomPlayer.GetComponent<RoomPlayer>();
        // score.index = player.index;
    }

    // public override void OnRoomClientSceneChanged(NetworkConnection conn) {
    //     // for (int i = 0; i < RoomPlayers.Count; i++) {
    //     //     var rpConn = RoomPlayers[i].connectionToClient;
    //     //     Debug.Log(rpConn);
    //     //     Debug.Log(rpConn.identity);
    //     //     Debug.Log(rpConn.identity.gameObject);

    //     //     var roomPlayer = rpConn.identity.gameObject;
    //     //     NetworkServer.Destroy(roomPlayer);
    //     // }
    //     if (SceneManager.GetActiveScene().path == GameplayScene) {
    //         NetworkServer.Destroy(conn.identity.gameObject);
    //     }
    //     base.OnRoomClientSceneChanged(conn);
    // }

    /// <summary>
    /// This is called on the server when all the players in the room are ready.
    /// <para>The default implementation of this function uses ServerChangeScene() to switch to the game player scene. By implementing this callback you can customize what happens when all the players in the room are ready, such as adding a countdown or a confirmation for a group leader.</para>
    /// </summary>
    public override void OnRoomServerPlayersReady()
    {
        if (isHeadless) {
            base.OnRoomServerPlayersReady();
        } else {
            showStartButton = true;
        }
    }


    #region Client Callbacks

    /// <summary>
    /// This is called on the client when the client stops.
    /// </summary>
    public override void OnRoomStopClient() { 
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopClient();
    }

    // public override void ServerChangeScene(string newSceneName) {
    //     var currentPath = SceneManager.GetActiveScene().path;
    //     base.ServerChangeScene(newSceneName);


    // }

    #endregion

    public void StartGame() {
        if (SceneManager.GetActiveScene().path == RoomScene) {
            if (!IsReadyToStart()) { return; }
            ServerChangeScene(GameplayScene);
        }
    }

    public override void OnGUI() { }
}
