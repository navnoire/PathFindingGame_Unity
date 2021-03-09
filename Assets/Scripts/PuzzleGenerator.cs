using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleGenerator : MonoBehaviour
{

    public Vector2Int puzzleSize = new Vector2Int(9, 6);

    public Material[] tileMaterials;
    public TileCoordsPack tileTexCoords;

    public AnimationCurve generationCurve;

    public Tile[,] GeneratePuzzle(Vector3 startPos, List<RoomData_SO.PortalInfo> portals, List<int> items, Action<List<Item>> callback)
    {
        if (puzzleSize.x == 0 || puzzleSize.y == 0)
        {
            Debug.LogError("Please set puzzle dimensions");
            Debug.Break();
        }

        GeneralPooler pool = GeneralPooler.Instance;
        pool.RefillPool();

        Tile[,] newPuzzle = new Tile[puzzleSize.x, puzzleSize.y];

        Tile startTile = pool.GetFromPool(3).GetComponent<Tile>();
        startTile.transform.position = startPos;
        startTile.transform.rotation = Quaternion.Euler(90, 0, 0);
        startTile.Coords = new Vector2Int((int)startPos.x, (int)startPos.z);
        newPuzzle[startTile.Coords.x, startTile.Coords.y] = startTile;
        startTile.gameObject.SetActive(true);

        for (int i = 0; i < portals.Count; i++)
        {
            Tile portal = pool.GetFromPool(4).GetComponent<Tile>();
            portal.transform.position = portals[i].portalPosition;
            portal.transform.rotation = Quaternion.Euler(90, 0, 0);
            portal.Coords = new Vector2Int((int)portals[i].portalPosition.x, (int)portals[i].portalPosition.z);

            Transgressor transgressor = portal.GetComponentInChildren<Transgressor>();
            if (transgressor != null)
            {
                transgressor.GoToRoomIndex = portals[i].targetRoomIndex;
                transgressor.targetPlayerDirection = portals[i].playerRotationAfterTransgress;
                transgressor.transgressTarget = portals[i].transgressToPosition;
            }

            newPuzzle[portal.Coords.x, portal.Coords.y] = portal;
            portal.gameObject.SetActive(true);

        }

        for (int x = 0; x < newPuzzle.GetLength(0); x++)
        {
            for (int y = 0; y < newPuzzle.GetLength(1); y++)
            {
                if (newPuzzle[x, y] == null)
                {
                    Tile tile = pool.GetFromPool(CurveWeightedRandom()).GetComponent<Tile>();
                    tile.transform.position = new Vector3(x, .11f, y);
                    tile.transform.rotation = Quaternion.Euler(90, 0, 0);
                    tile.Coords = new Vector2Int(x, y);
                    tile.gameObject.SetActive(true);
                    if (!tile.isUntouchable)
                    {
                        tile.isOpen = true;
                        tile.isRotatable = true;
                    }
                    newPuzzle[x, y] = tile;
                }
            }
        }

        foreach (var tile in newPuzzle)
        {
            if (tile._mesh == null)
            {
                tile.InitializeSingleUVRect(tileTexCoords.coordArray[(int)tile.tileType]);
                tile.AssignMaterials(tileMaterials[0], tileMaterials[1]);
                tile.SetEmission(false);
            }

        }
        PlaceGems(newPuzzle, items, callback);
        return newPuzzle;
    }

    public void PlaceGems(Tile[,] puzzle, List<int> gems, Action<List<Item>> callback)
    {
        GeneralPooler pool = GeneralPooler.Instance;
        List<Item> newGems = new List<Item>();
        foreach (int item in gems)
        {
            Vector2Int currentCoords = new Vector2Int(UnityEngine.Random.Range(0, puzzle.GetLength(0)), UnityEngine.Random.Range(0, puzzle.GetLength(1)));
            while (puzzle[currentCoords.x, currentCoords.y].isOpen == false)
            {
                currentCoords = new Vector2Int(UnityEngine.Random.Range(0, puzzle.GetLength(0)), UnityEngine.Random.Range(0, puzzle.GetLength(1)));
            }

            Item i = pool.GetFromPool(item).GetComponent<Item>();
            i.transform.position = new Vector3(currentCoords.x, 0.15f, currentCoords.y);
            i.transform.rotation = Quaternion.identity;
            i.coords = currentCoords;
            puzzle[currentCoords.x, currentCoords.y].isOpen = false;
            i.isVisible = true;
            i.gameObject.SetActive(true);
            newGems.Add(i);
        }

        callback(newGems);
    }

    int CurveWeightedRandom()
    {
        return Mathf.RoundToInt(generationCurve.Evaluate(UnityEngine.Random.value));
    }



    public Tile[,] ReloadPuzzle(PuzzleData data, List<RoomData_SO.PortalInfo> portals)
    {
        Tile[,] puzzle = new Tile[data.tileIndexes.GetLength(0), data.tileIndexes.GetLength(1)];

        GeneralPooler pool = GeneralPooler.Instance;
        pool.RefillPool();

        //instantiate portals
        for (int i = 0; i < portals.Count; i++)
        {
            Tile portal = pool.GetFromPool(4).GetComponent<Tile>();
            portal.transform.position = portals[i].portalPosition;
            portal.transform.rotation = Quaternion.Euler(90, data.tileZRotations[(int)portals[i].portalPosition.x, (int)portals[i].portalPosition.z], 0);
            portal.Coords = new Vector2Int((int)portals[i].portalPosition.x, (int)portals[i].portalPosition.z);
            portal.realRotation = data.tileZRotations[portal.Coords.x, portal.Coords.y];
            for (int v = 0; v < 4; v++)
            {
                portal.values[v] = data.tileValues[portal.Coords.x, portal.Coords.y, v];
            }

            Transgressor transgressor = portal.GetComponentInChildren<Transgressor>();
            transgressor.GoToRoomIndex = portals[i].targetRoomIndex;
            transgressor.targetPlayerDirection = portals[i].playerRotationAfterTransgress;
            transgressor.transgressTarget = portals[i].transgressToPosition;
            if (portal._mesh == null)
            {
                portal.InitializeSingleUVRect(tileTexCoords.coordArray[(int)portal.tileType]);
                portal.AssignMaterials(tileMaterials[0], tileMaterials[1]);
            }

            portal.gameObject.SetActive(true);
            puzzle[portal.Coords.x, portal.Coords.y] = portal;
        }

        Tile currentTile;
        // instantiate regular tiles
        for (int x = 0; x < data.tileIndexes.GetLength(0); x++)
        {
            for (int y = 0; y < data.tileIndexes.GetLength(1); y++)
            {
                float rot = data.tileZRotations[x, y];
                if (rot > 275) rot = 0;

                if (puzzle[x, y] == null)
                {
                    currentTile = pool.GetFromPool(data.tileIndexes[x, y]).GetComponent<Tile>();
                    currentTile.transform.position = new Vector3(x, .11f, y);
                    currentTile.transform.rotation = Quaternion.Euler(90, rot, 0);
                    currentTile.realRotation = rot;
                    currentTile.Coords = new Vector2Int(x, y);
                    for (int v = 0; v < 4; v++)
                    {
                        currentTile.values[v] = data.tileValues[x, y, v];
                    }

                    if (!currentTile.isUntouchable)
                    {
                        currentTile.isOpen = true;
                        currentTile.isRotatable = true;
                    }

                    if (currentTile._mesh == null)
                    {
                        currentTile.InitializeSingleUVRect(tileTexCoords.coordArray[(int)currentTile.tileType]);
                        currentTile.AssignMaterials(tileMaterials[0], tileMaterials[1]);
                    }

                    currentTile.SetEmission(false);
                    currentTile.gameObject.SetActive(true);
                    puzzle[x, y] = currentTile;
                }
            }
        }
        return puzzle;
    }
}
