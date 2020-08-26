using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ObjectPool : NetworkBehaviour
{
    public int maxPoolSize = 10;
    public Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

    public Dictionary<uint, GameObject> spawnedObjects = new Dictionary<uint, GameObject>();
    public string spawnablesDirectory = "Spawnables";
    public static ObjectPool singleton;

    private void Start() {
        singleton = this;
    }

    public void RegisterPrefab(string name, int count) {
        GameObject prefab = Resources.Load<GameObject>($"{spawnablesDirectory}/{name}");
        bool existing = pool.ContainsKey(name);

        Queue<GameObject> prefabPool;
        if (existing) {
            prefabPool = pool[name];
        } else {
            prefabPool = new Queue<GameObject>();
            pool.Add(name, prefabPool);
        }

        for (int i = 0; i < count && prefabPool.Count < maxPoolSize; i++) {
            var go = Instantiate(prefab);
            go.name = go.name.Replace("(Clone)", "") + i.ToString();
            prefabPool.Enqueue(go);
            go.SetActive(false);
            go.GetComponent<IPoolableObject>().Init();
        }
        
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

    // public GameObject SpawnObject(Vector3 position, System.Guid assetId) {
    //     return GetFromPool(assetId, position, Quaternion.identity);
    // }

    public void UnSpawnObject(GameObject spawned) {
        spawned.transform.SetParent(null);
        spawned.SetActive(false);
    }
}
