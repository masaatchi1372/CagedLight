using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : SingletoneMonoBehaviour<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start()
    {

        // initiating our pool dictionary
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // we'll loop through all the pools and create the initial objects
        foreach (Pool pool in pools)
        {
            // we create a new Queue
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // creating objects according to the size of each pool
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity);

                // we deactivate it so it's not shown in the screen
                obj.SetActive(false);

                // append a number to the name of the obj
                obj.name = obj.name + i;

                // we push it into the objectPool
                objectPool.Enqueue(obj);
            }

            // we push the objectPool into our dictionary
            poolDictionary.Add(pool.tag, objectPool);
        }

    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        // we get an object from the pool
        if (poolDictionary[tag].Count < 1)
        {
            Debug.Log($"count:{poolDictionary[tag].Count} Queue is empty, cannot spawn an object");
            return null;
        }

        GameObject objectToSpawn;
        if (!poolDictionary[tag].TryDequeue(out objectToSpawn) )
        {
            Debug.Log($"couldn't dequeue object from the pool");
            return null;
        }

        Debug.Log($"name: {objectToSpawn.name}");

        // setting the transforms
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // calling the OnObjectSpawn method
        IObjectPooled objPoolInterface = objectToSpawn.GetComponent<Line>();
        if (objPoolInterface != null)
        {
            objPoolInterface.OnSpawnObjectPooled();
        }

        // we activate it
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public bool PoolObject(string tag, GameObject objToPool)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return false;
        }

        // deactivate the object
        objToPool.SetActive(false);

        // reset the position and rotation
        objToPool.transform.position = Vector3.zero;
        objToPool.transform.rotation = Quaternion.identity;

        // calling the OnPoolingObject method
        IObjectPooled objPoolInterface = objToPool.GetComponent<Line>();
        if (objPoolInterface != null)
        {
            objPoolInterface.OnPoolingObject();
        }

        // put back in the poolDictionary
        poolDictionary[tag].Enqueue(objToPool);

        return true;
    }

}