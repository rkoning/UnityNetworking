using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class PlayerScore : NetworkBehaviour
{

    [SyncVar]
    public int index;

    [SyncVar]
    public uint score;

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() {
        index = connectionToClient.connectionId;
    }

    private void OnGUI() {
        GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score.ToString("0000000")}");
    }
}
