using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    public static event Action<Vector2Int, int> OnItemCollected;

    public int PoolIndex;
    public ItemRarity itemRarity;
    public SerializableVector2Int coords;

    public bool isRobbered;
    public bool isVisible = true;

    public int cost;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isRobbered)
            {
                isRobbered = false;
                transform.parent = GeneralPooler.Instance.poolHolder;
            }
            gameObject.SetActive(false);
            OnItemCollected?.Invoke(coords, PoolIndex);
        }
    }
}


public enum ItemRarity
{
    common,
    uncommon,
    rare
}

//[Serializable]
//public struct ItemStatus
//{
//    public SerializableVector2Int position;
//    public bool isVisible;
//    public int poolIndex;

//    public ItemStatus(Vector2Int pos, int pool, bool visibility)
//    {
//        position = pos;
//        poolIndex = pool;
//        isVisible = visibility;
//    }

//    public void SetStatus(bool b)
//    {
//        isVisible = b;
//    }
//}
