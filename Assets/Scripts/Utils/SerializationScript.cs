using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SerializationScript : MonoBehaviour
{
    BinaryFormatter bf;
    string runtimePath;
    string gameStatePath;
    string tempSavePath;

    void Awake()
    {
        bf = new BinaryFormatter();
        runtimePath = Application.persistentDataPath + "/Runtime/";
        gameStatePath = Application.persistentDataPath + "/Data/";
        tempSavePath = runtimePath + "/temp/";
        Directory.CreateDirectory(runtimePath);
        Directory.CreateDirectory(gameStatePath);

    }


    public void SavePuzzleData(int roomNumber, int levelNumber, Tile[,] puzzle, List<Item> gems)
    {
        FileStream file = File.Open(runtimePath + "puzzleRoom_" + levelNumber + roomNumber + ".dat", FileMode.OpenOrCreate);


        PuzzleData room = new PuzzleData(roomNumber, puzzle, gems);
        bf.Serialize(file, room);
        file.Close();

    }

    public PuzzleData LoadPuzzleData(int roomNumber, int levelNumber)
    {
        PuzzleData room = null;
        if (File.Exists(runtimePath + "puzzleRoom_" + levelNumber + roomNumber + ".dat"))
        {
            FileStream file = File.Open(runtimePath + "puzzleRoom_" + levelNumber + roomNumber + ".dat", FileMode.Open);
            room = (PuzzleData)bf.Deserialize(file);
            file.Close();
        }

        return room;
    }

    // save state after station reached(in case of reborn)
    public void SaveTempPuzzleData(int roomNumber, int levelNumber, Tile[,] puzzle, List<Item> gems)
    {
        FileStream file = File.Open(tempSavePath + "puzzleRoom_" + levelNumber + roomNumber + ".dat", FileMode.OpenOrCreate);


        PuzzleData room = new PuzzleData(roomNumber, puzzle, gems);
        bf.Serialize(file, room);
        file.Close();
    }

    //load data to restart level after reborn from last station
    public PuzzleData LoadTempPuzzleData(int roomNumber, int levelNumber)
    {
        PuzzleData room = null;
        if (File.Exists(tempSavePath + "puzzleRoom_" + levelNumber + roomNumber + ".dat"))
        {
            FileStream file = File.Open(runtimePath + "puzzleRoom_" + levelNumber + roomNumber + ".dat", FileMode.Open);
            room = (PuzzleData)bf.Deserialize(file);
            file.Close();
        }

        return room;
    }


    public void ClearLevelData()
    {
        if (Directory.Exists(runtimePath))
        {
            Directory.Delete(runtimePath, true);
        }
    }

    public void SaveGameState(GameProps_SO state)
    {
        FileStream file = File.Open(gameStatePath + "game_" + state.playerID + ".dat", FileMode.OpenOrCreate);

        GameData data = new GameData(state);
        bf.Serialize(file, data);
        file.Close();
    }

    public GameProps_SO LoadGameState(string playerID)
    {
        GameProps_SO game = null;

        if (File.Exists(gameStatePath + "game_" + playerID + ".dat"))
        {
            FileStream file = File.Open(gameStatePath + "game_" + playerID + ".dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            game = GameData.ConvertToGameState_SO(data);
            file.Close();
        }

        return game;
    }
}

[Serializable]
public sealed class GameData
{
    public string checksum;
    public string playerID;

    public int currentLevelIndex;
    public int currencyAmount;
    public int gemsCollectedTotal;

    public GameData(GameProps_SO data)
    {
        checksum = data.checksum;
        playerID = data.playerID;

        currentLevelIndex = data.currentLevelIndex;
        currencyAmount = data.currencyAmount;
        gemsCollectedTotal = data.gemsCollectedTotal;
    }

    public static GameProps_SO ConvertToGameState_SO(GameData data)
    {
        GameProps_SO state = ScriptableObject.CreateInstance<GameProps_SO>();

        state.checksum = data.checksum;
        state.playerID = data.playerID;
        state.currentLevelIndex = data.currentLevelIndex;
        state.currencyAmount = data.currencyAmount;
        state.gemsCollectedTotal = data.gemsCollectedTotal;

        return state;
    }
}

[Serializable]
public class PuzzleData
{
    public int roomNumber;
    public int[,] tileIndexes;
    public float[,] tileZRotations;
    public bool[,,] tileValues;

    public SerializableVector2Int[] itemPositions;
    public int[] itemPoolIndexes;
    public bool[] itemVisibility;


    public PuzzleData(int _roomNumber, Tile[,] _puzzle, List<Item> _items)
    {
        roomNumber = _roomNumber;
        tileIndexes = new int[_puzzle.GetLength(0), _puzzle.GetLength(1)];
        tileZRotations = new float[_puzzle.GetLength(0), _puzzle.GetLength(1)];
        tileValues = new bool[_puzzle.GetLength(0), _puzzle.GetLength(1), 4];

        itemPositions = new SerializableVector2Int[_items.Count];
        itemPoolIndexes = new int[_items.Count];
        itemVisibility = new bool[_items.Count];

        for (int i = 0; i < tileIndexes.GetLength(0); i++)
        {
            for (int j = 0; j < tileIndexes.GetLength(1); j++)
            {
                tileIndexes[i, j] = _puzzle[i, j].prefabIndex;
                tileZRotations[i, j] = _puzzle[i, j].realRotation;

                for (int z = 0; z < 4; z++)
                {
                    tileValues[i, j, z] = _puzzle[i, j].values[z];
                }
            }
        }

        for (int j = 0; j < _items.Count; j++)
        {
            itemPositions[j] = _items[j].coords;
            itemPoolIndexes[j] = _items[j].PoolIndex;
            itemVisibility[j] = _items[j].isVisible;
        }
    }

}

[Serializable]
public struct SerializableVector2Int
{
    public int x;
    public int y;

    public SerializableVector2Int(int rX, int rY)
    {
        x = rX;
        y = rY;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}]", x, y);
    }

    public static implicit operator Vector2Int(SerializableVector2Int sValue)
    {
        return new Vector2Int(sValue.x, sValue.y);
    }

    public static implicit operator SerializableVector2Int(Vector2Int sValue)
    {
        return new SerializableVector2Int(sValue.x, sValue.y);
    }
}
//[Serializable]
//public struct SerializableQuaternion
//{
//    public float x;
//    public float y;
//    public float z;
//    public float w;

//    public SerializableQuaternion(float rX, float rY, float rZ, float rW)
//    {
//        x = rX;
//        y = rY;
//        z = rZ;
//        w = rW;
//    }


//    public override string ToString()
//    {
//        return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
//    }

//    public static implicit operator Quaternion(SerializableQuaternion sValue)
//    {
//        return new Quaternion(sValue.x, sValue.y, sValue.z, sValue.w);
//    }

//    public static implicit operator SerializableQuaternion(Quaternion sValue)
//    {
//        return new SerializableQuaternion(sValue.x, sValue.y, sValue.z, sValue.w);
//    }
//}

