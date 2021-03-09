using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

[RequireComponent(typeof(PathFinder))]
public class PathRequestManager : MonoBehaviour
{


    Queue<PathResult> results = new Queue<PathResult>();
    PathResult result;

    static PathRequestManager instance;
    PathFinder pathFinder;

    void Awake()
    {
        instance = this;
        pathFinder = GetComponent<PathFinder>();
    }

    void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    result = results.Dequeue();

                }
                result.callback(result.path, result.isCycle);
            }
        }
    }

    public static void RequestPath(PathRequest request)
    {

        ThreadStart threadStart = delegate
        {
            instance.pathFinder.FindPath(request, instance.FinishedProcessingPath);
        };

        threadStart.Invoke();


    }

    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    }

}


public struct PathRequest
{
    public Vector2Int playerCurrentCoord;
    public Vector2Int direction;
    public Vector2Int prevNodeCoord;
    public Action<List<Vector2Int>, bool> callback;

    public PathRequest(Vector2Int _start, Vector2Int _direction, Vector2Int _prevNode, Action<List<Vector2Int>, bool> _callback)
    {
        playerCurrentCoord = _start;
        direction = _direction;
        prevNodeCoord = _prevNode;
        callback = _callback;
    }

}

public struct PathResult
{
    public List<Vector2Int> path;
    public bool isCycle;
    public Action<List<Vector2Int>, bool> callback;

    public PathResult(List<Vector2Int> path, bool isCycle, Action<List<Vector2Int>, bool> callback)
    {
        this.path = path;
        this.isCycle = isCycle;
        this.callback = callback;
    }
}
