using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class PathFinder : MonoBehaviour
{
    PuzzleManager puzzleMgr;

    void Awake()
    {
        puzzleMgr = FindObjectOfType<PuzzleManager>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Tile[,] puzzle = puzzleMgr.puzzle;

        foreach (var item in puzzle)
        {
            item.SetEmission(false);
        }

        Vector2Int startPos = request.playerCurrentCoord;
        Vector2Int direction = request.direction;
        Vector2Int prevNodeCoords = request.prevNodeCoord;

        List<Vector2Int> path = new List<Vector2Int>();
        bool isCycle = false;
        bool isStart = true;

        Vector2Int aux = new Vector2Int(20, 10);

        Queue<Tile> candidates = new Queue<Tile>();
        candidates.Enqueue(puzzle[startPos.x, startPos.y]);

        while (candidates.Count > 0)
        {
            Tile currentTile = candidates.Dequeue();
            path.Add(currentTile.Coords);

            if (prevNodeCoords == startPos)
            {
                if (isStart)
                {
                    isStart = false;
                }
                else
                {
                    if (currentTile.Coords == path[1])
                    {
                        isCycle = true;
                        path.RemoveRange(path.Count - 2, 2);
                        break;
                    }
                }
            }

            //fourcorner or station
            if (currentTile.prefabIndex == 1 || currentTile.prefabIndex == 3)
            {
                if (!isStart)
                {
                    direction = currentTile.Coords - prevNodeCoords;
                }
                aux = currentTile.Coords + direction;
            }

            //line or portal tile
            if (currentTile.prefabIndex == 2 || currentTile.prefabIndex == 4)
            {
                direction = currentTile.Coords - prevNodeCoords;
                aux = currentTile.Coords + direction;
            }

            //crook
            if (currentTile.prefabIndex == 0)
            {
                if (currentTile.connectedWith.Count == 2)
                {
                    aux = currentTile.connectedWith.Single(n => n != prevNodeCoords);
                }

                if (currentTile.connectedWith.Count == 1)
                {
                    aux = currentTile.connectedWith.First();
                }
            }

            if (currentTile.connectedWith.Contains(aux) && aux != prevNodeCoords)
            {
                prevNodeCoords = currentTile.Coords;
                candidates.Enqueue(puzzle[aux.x, aux.y]);
            }

        }

        foreach (var item in path)
        {
            puzzle[item.x, item.y].SetEmission(true);
        }
        callback(new PathResult(path, isCycle, request.callback));
    }

}
