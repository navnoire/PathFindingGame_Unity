using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGhost
{
    public int poolIndex;
    public SerializableVector2Int coords;
    public bool isVisible = true;

    public ItemGhost(int pool, SerializableVector2Int pos, bool visibility)
    {
        poolIndex = pool;
        coords = pos;
        isVisible = visibility;
    }
}
