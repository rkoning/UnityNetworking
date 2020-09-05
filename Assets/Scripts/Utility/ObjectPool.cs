using System.Runtime.InteropServices.ComTypes;
using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ObjectPool : NetworkBehaviour
{
    public int maxPoolSize = 10;
    public Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
    public Dictionary<System.Guid, string> poolNames = new Dictionary<System.Guid, string>();
    public Dictionary<uint, GameObject> spawnedObjects = new Dictionary<uint, GameObject>();
    public static string spawnablesDirectory = "Spawnables";
    public static ObjectPool singleton;

    private void Awake() {
        singleton = this;
    }

    public static void RegisterPrefab(string name, int count) {
        GameObject prefab = Resources.Load<GameObject>($"{spawnablesDirectory}/{name}");
        bool existing = singleton.pool.ContainsKey(name);

        Queue<GameObject> prefabPool;
        if (existing) {
            prefabPool = singleton.pool[name];
        } else {
            prefabPool = new Queue<GameObject>();
            singleton.pool.Add(name, prefabPool);
        }
        singleton.poolNames[prefab.GetComponent<NetworkIdentity>().assetId] = name;
        for (int i = 0; i < count && prefabPool.Count < singleton.maxPoolSize; i++) {
            var go = Instantiate(prefab);
            go.name = go.name.Replace("(Clone)", "") + i.ToString();
            prefabPool.Enqueue(go);
            go.SetActive(false);
            go.GetComponent<IPoolableObject>().Init();
        }
    }

    public static void Register(uint netId, GameObject obj) {
        singleton.spawnedObjects.Add(netId, obj);
    }

    public GameObject GetFromPool(System.Guid assetId, Vector3 position, Quaternion rotation) {
        return GetFromPool(poolNames[assetId], position, rotation);
    }

    public GameObject GetFromPool(string name, Vector3 position, Quaternion rotation) {
        // Get the oldest gameobject on the queue
        var go = pool[name].Dequeue();
        // set up the object
        go.SetActive(true);
        go.transform.position = position;
        go.transform.rotation = rotation;
        // requeue it as the youngest queue object
        pool[name].Enqueue(go);
        NetworkServer.Spawn(go);
        return go;
    }

    IEnumerator Despawn(GameObject gameObject, float delay) {
        yield return new WaitForSeconds(delay);
        NetworkServer.UnSpawn(gameObject);
    }

    public static GameObject SpawnHandler(SpawnMessage msg) {
        return singleton.GetFromPool(msg.assetId, msg.position, msg.rotation);
    }

    public static void UnspawnHandler(GameObject spawned) {
        Debug.Log("Unspawning");
        if (spawned == null)
            return;
        spawned.transform.SetParent(null);
        spawned.SetActive(false);
    }
}
