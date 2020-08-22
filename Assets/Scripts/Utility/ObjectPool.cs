using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ObjectPool : MonoBehaviour
{
    public int maxPoolSize = 10;
    public Dictionary<System.Guid, Queue<GameObject>> pool = new Dictionary<System.Guid, Queue<GameObject>>();

    public static ObjectPool singleton;

    private void Start() {
        singleton = this;
    }

    public System.Guid RegisterPrefab(GameObject prefab, int count) {
        var assetId = prefab.GetComponent<NetworkIdentity>().assetId;
        bool existing = pool.ContainsKey(assetId);

        Queue<GameObject> prefabPool;
        if (existing) {
            prefabPool = pool[assetId];
        } else {
            prefabPool = new Queue<GameObject>();
            pool.Add(assetId, prefabPool);
        }

        for (int i = 0; i < count && prefabPool.Count < maxPoolSize; i++) {
            var go = Instantiate(prefab);
            go.name = go.name.Replace("(Clone)", "") + i.ToString();
            prefabPool.Enqueue(go);
            go.SetActive(false);
            go.GetComponent<Spell>().Init();
        }
        
        if (!ClientScene.prefabs.ContainsKey(assetId))
            ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
        return assetId;
    }

    public GameObject GetFromPool(System.Guid assetId, Vector3 position, Quaternion rotation) {
        // Get the oldest gameobject on the queue
        var go = pool[assetId].Dequeue();
        // set up the object
        go.SetActive(true);
        go.transform.position = position;
        go.transform.rotation = rotation;
        // requeue it as the youngest queue object
        pool[assetId].Enqueue(go);
        return go;
    }

    public GameObject SpawnObject(Vector3 position, System.Guid assetId) {
        return GetFromPool(assetId, position, Quaternion.identity);
    }

    public void UnSpawnObject(GameObject spawned) {
        spawned.SetActive(false);
    }
}
