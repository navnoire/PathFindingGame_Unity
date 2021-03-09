using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public int index;
        public GameObject prefab;
        public int size;
        public bool selfExpanding;

        public List<GameObject> pooledObjects;
    }

    public static GeneralPooler Instance;
    public List<Pool> pools;
    public Transform poolHolder;

    Dictionary<int, Pool> poolDictionary;


    void Awake()
    {
        Instance = this;
    }

    public void FillPool()
    {
        poolDictionary = new Dictionary<int, Pool>();

        foreach (Pool p in pools)
        {

            p.pooledObjects = new List<GameObject>();
            for (int i = 0; i < p.size; i++)
            {
                GameObject obj = Instantiate(p.prefab, poolHolder);
                obj.SetActive(false);
                p.pooledObjects.Add(obj);
            }

            poolDictionary.Add(p.index, p);
        }

    }

    public GameObject GetFromPool(int index)
    {
        if (!poolDictionary.ContainsKey(index))
        {
            Debug.LogWarning("Index " + index + " doesn't exist in a pool");
            return null;
        }

        Pool current = poolDictionary[index];
        for (int i = 0; i < current.pooledObjects.Count; i++)
        {
            if (!current.pooledObjects[i].activeInHierarchy)
            {
                return current.pooledObjects[i];
            }
        }

        if (current.selfExpanding)
        {
            GameObject obj = Instantiate(current.prefab, poolHolder);
            obj.SetActive(false);
            current.pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    public void RefillPool()
    {
        foreach (Pool p in pools)
        {
            for (int i = 0; i < p.pooledObjects.Count; i++)
            {
                p.pooledObjects[i].SetActive(false);
            }
        }
    }
}
