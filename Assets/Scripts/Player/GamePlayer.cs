using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;
using System.Linq;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class GamePlayer : NetworkBehaviour
{

    [SyncVar]
    public string displayName = "Loading...";

    [SyncVar]
    public string buildJson;

    [SyncVar]
    public string initBuild;

    private RoomManager room;
    public RoomManager Room {
        get {
           if (room != null) return room;
           return room = NetworkManager.singleton as RoomManager;
        }
    }

    [Header("Character")]
    public SavedDeck build;
    private Avatar avatar;

    [Header("UI")]
    public GameObject hudUI;
    public DeckUI deckUI;
    public HealthUI healthUI;

    #region Start & Stop Callbacks

    public override void OnStartClient() {
        foreach (var roomPlayer in Room.RoomPlayers) {
            roomPlayer.gameObject.SetActive(false);
        }
        if (Room)
            Room.GamePlayers.Add(this);
    }

    public override void OnStartServer() {
        // Get the chosen buildJson and deserialize it.
        build = SavedDeck.LoadFromString(buildJson);
        // Spawn the build.character.avatarPrefab with client authority
        var spawnPoint = NetworkManager.singleton.GetStartPosition();
        build = SavedDeck.LoadFromString(buildJson);
        var avatarGameObject = GameObject.Instantiate(build.character.avatarPrefab, spawnPoint.position, spawnPoint.rotation);
        avatarGameObject.GetComponent<Avatar>().playerNetId = netId;
        NetworkServer.Spawn(avatarGameObject, connectionToClient);

        // // setup the avatar
        // avatar = avatarGameObject.GetComponent<Avatar>();
        // avatar.Init(this);
    }

    public override void OnStartAuthority() { 
        build = SavedDeck.LoadFromString(buildJson);
        var byType = build.cards.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        foreach(var kvp in byType) {
            RegisterPrefab(kvp.Key.spellPrefab, kvp.Value);
        }
        this.buildJson = build.ToString();
        CmdAvatarSpawned();
    }

    [Command]
    public void CmdAvatarSpawned() {
        Debug.Log("CmdAvatarSpawned");
        RpcAvatarSpawned();
    }

    [ClientRpc]
    public void RpcAvatarSpawned() {
        Debug.Log("RpcAvatarSpawned");
        var avatars = FindObjectsOfType<Avatar>();
        var players = new List<GamePlayer>(FindObjectsOfType<GamePlayer>());

        var playerDictionary = players.ToDictionary(x => x.netId, x => x);
        foreach(Avatar a in avatars) {
            if (playerDictionary.ContainsKey(a.playerNetId)) {
                // map the other players to their respective avatars
                if (a.initialized == true)
                    continue;
                a.Init(playerDictionary[a.playerNetId]);
                playerDictionary[a.playerNetId].avatar = a;

                // get the buildJson of this gamePlayer and call LoadCards() to initialize the deck
                a.deck.LoadCards(SavedDeck.LoadFromString(playerDictionary[a.playerNetId].buildJson).cards);
                if (a.hasAuthority) {
                    // This will only happen on the local player
                    hudUI.SetActive(true);
                    // link healthUI to health
                    healthUI.health = avatar.health;
                    // link deckUI to deck
                    deckUI.deck = avatar.deck;
                    deckUI.Init();
                }
                continue;
            }
            // we should never get here, so spit out a warning
            Debug.LogWarning($"Warning, avatar with connectionId: {netIdentity.connectionToClient} does not have a gamePlayer");
        }
        // look up all avatars in the scene and match them with their gameplayers
    }
    
    public override void OnStopClient() {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName) {
        this.displayName = displayName;
    }

    #endregion

    public void RegisterPrefab(GameObject prefab, int count) {
        CmdRegisterPrefab(prefab.name, count);
    }

    [Command]
    public void CmdRegisterPrefab(string name, int count) {
        ObjectPool.RegisterPrefab(name, count);
    }

    public void GetFromPool(string name, Vector3 position, Quaternion rotation) {
        CmdGetFromPool(name, position, rotation);
    }

    [Command]
    public void CmdGetFromPool(string name, Vector3 position, Quaternion rotation) {
        var go = ObjectPool.singleton.GetFromPool(name, position, rotation);
        go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    public void DealDamage(Health other, float amount) {
        if (hasAuthority)
            CmdDealDamage(other.GetComponent<NetworkIdentity>().netId, amount);
    }

    [Command]
    public void CmdDealDamage(uint netId, float amount) {
        ObjectPool.singleton.spawnedObjects[netId].GetComponent<Health>().TakeDamage(amount, avatar);
    }

    public void ApplyStatus(Health health, StatusFactory factory) {
        if (hasAuthority)
            CmdApplyStatus(health.GetComponent<NetworkIdentity>().netId, factory.name);
    }

    [Command]
    public void CmdApplyStatus(uint netId, string factoryName) {
        StatusFactory fact = Resources.Load<StatusFactory>($"StatusEffects/{factoryName}");
        ObjectPool.singleton.spawnedObjects[netId].GetComponent<Health>().ApplyStatus(fact, avatar);
    }

    public void Cast(string name) {
        Debug.Log($"CmdCast called from: {avatar.playerNetId}");
        CmdCast(name);
    }

    [Command]
    private void CmdCast(string name) {
        var go = ObjectPool.singleton.GetFromPool(name, avatar.deck.castTransform.position, avatar.deck.castTransform.rotation);
        var netId = go.GetComponent<NetworkIdentity>();
        Debug.Log($"Client Authority assigned to: {avatar.playerNetId}");
        netId.AssignClientAuthority(avatar.gamePlayer.connectionToClient);

        RpcCast(netId.netId); 
    }

    [ClientRpc]
    private void RpcCast(uint netId)
    {
        Spell spell = ObjectPool.singleton.spawnedObjects[netId].GetComponent<Spell>();
        avatar.deck.AfterCast(spell);
    }

    public void OnCurrentHealthChanged(float oldValue, float newValue) {
        avatar.health.currentHealth = newValue;
    }
    public void OnMaxHealthChanged(float oldValue, float newValue) {
        avatar.health.maxHealth = newValue;
    }
    public void OnIsDeadChanged(bool oldValue, bool newValue) {
        avatar.health.IsDead = newValue;
    }

    public void PlayerDead() {
        if (hasAuthority)
            CmdPlayerDead();
    }

    [Command]
    public void CmdPlayerDead() {
        Debug.Log("Player dying on server" + connectionToClient);
        // var clips = animator.GetCurrentAnimatorClipInfo(0);
        StartCoroutine(WaitThenRespawn(5f));
    }

    private IEnumerator WaitThenRespawn(float duration) {
        yield return new WaitForSeconds(duration);
        avatar.health.currentHealth = avatar.health.maxHealth;
        var spawnPoint = NetworkManager.singleton.GetStartPosition();
        avatar.transform.position = spawnPoint.position;
        avatar.transform.rotation = spawnPoint.rotation;
        RpcPlayerAlive();
    }

    [ClientRpc]
    private void RpcPlayerAlive() {
        avatar.health.Respawn();
    }
}