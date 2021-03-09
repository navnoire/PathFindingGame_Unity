using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NavnoireCoding.Managers;
using NavnoireCoding.Utils;

public class CollectManager : MonoBehaviour
{

    public Text scoreText;
    [HideInInspector] public Trail trail;

    int collectedOnThisLevel = 0;
    int pickedOnThisLevel = 0;




    void OnEnable()
    {
        Item.OnItemCollected += PickItem;
        Player.StationReached += CollectItems;
    }

    void OnDisable()
    {
        Item.OnItemCollected -= PickItem;
        Player.StationReached -= CollectItems;
    }

    void PickItem(Vector2Int pos, int poolIndex)
    {
        pickedOnThisLevel++;
    }

    void CollectItems(Action callback)
    {
        StartCoroutine(Collect(callback));
    }

    public void SaveScore(int amount)
    {
        GameManager.Instance.currentGameProps.gemsCollectedTotal += amount;
    }


    IEnumerator Collect(Action callback)
    {
        if (pickedOnThisLevel > 0)
        {
            yield return new WaitForSeconds(1f);
        }
        SaveScore(pickedOnThisLevel);
        while (pickedOnThisLevel > 0)
        {
            pickedOnThisLevel--;
            collectedOnThisLevel++;
            trail.CollectItem();
            yield return new WaitForSeconds(1f);
            Events.Instance.OnItemCollected?.Invoke(collectedOnThisLevel);

        }

        callback();
        yield return null;
    }
}
