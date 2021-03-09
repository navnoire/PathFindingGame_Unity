using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(menuName = "DataAssets/New Game Data Asset", fileName = "Game", order = 52)]
public class GameProps_SO : ScriptableObject
{
    public string checksum;
    public string playerID;

    // может понадобиться
    public int currentLevelIndex;
    public int currencyAmount;
    public int gemsCollectedTotal;

    public void Assign(GameProps_SO instance)
    {
        checksum = instance.checksum;
        playerID = instance.playerID;
        currentLevelIndex = instance.currentLevelIndex;
        currencyAmount = instance.currencyAmount;
        gemsCollectedTotal = instance.gemsCollectedTotal;
    }
}
