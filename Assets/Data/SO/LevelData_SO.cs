using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataAssets/New Level Data", fileName = "Level_")]
public class LevelData_SO : ScriptableObject
{
    public int levelIndex;
    public int levelGoal;
    [Range(1f, 3f)]
    public float GemMultiplier = 1;

    public RoomData_SO[] rooms;
    public ItemDrawer_SO itemDrawer;
    public List<PuzzleData> puzzleDatas;


    public void ClearPuzzleStartData()
    {
        puzzleDatas.Clear();
    }


}
