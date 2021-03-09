using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public delegate void PathEndedHandler();
    public delegate void StationReachedHandler(Action callback);

    public static event PathEndedHandler PathEnded;
    public static event StationReachedHandler StationReached;

    public float moveSpeed;
    public float rotationSpeed;
    public float timerThreshold;

    public Vector2Int origin;
    Vector2Int playerCoord;
    Vector2Int prevNode;
    Vector2Int oldPrevNode;
    public Vector2Int PlayerCurrentCoord
    {
        set
        {
            oldPrevNode = prevNode;
            prevNode = playerCoord;
            playerCoord = value;
        }
        get
        {
            return playerCoord;

        }
    }

    IEnumerator StartFollowPath;

    [HideInInspector] public bool transferringLoot;
    public bool followingPath = true;
    bool pathIsCycle;
    bool pathNeedsUpdate;
    bool isLeavingRoom;

    Animator playerAnimator;
    int animatorSpeedID;

    public List<Vector2Int> CurrentPath { set; get; }
    public Vector3 checkPointForChaser;


    public void SetupPlayer()
    {
        transferringLoot = false;
        prevNode = PlayerCurrentCoord - new Vector2Int((int)Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad), (int)Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
        CurrentPath = new List<Vector2Int>(1);
        checkPointForChaser = transform.position;
        playerAnimator = GetComponentInChildren<Animator>();
        animatorSpeedID = Animator.StringToHash("Speed");

        StartCoroutine(UpdatePath());
    }

    void OnEnable()
    {
        PortalZone.OnRoomExit += IsLeavingRoom;
        Tile.OnTileRotated += PathNeedsUpdate;
        Transgressor.TransgressAction += Transgress;
        PlayerCollisionSensor.CollidedWithTail += OnCollidedWithTail;
    }

    void OnDisable()
    {
        PortalZone.OnRoomExit -= IsLeavingRoom;
        Tile.OnTileRotated -= PathNeedsUpdate;
        Transgressor.TransgressAction -= Transgress;
        PlayerCollisionSensor.CollidedWithTail -= OnCollidedWithTail;
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 2f)
        {
            yield return new WaitForSeconds(2f);
        }

        Vector2Int direction = PlayerCurrentCoord - prevNode;
        PathRequestManager.RequestPath(new PathRequest(PlayerCurrentCoord, direction, prevNode, OnPathFound));

        while (true)
        {
            if (pathNeedsUpdate)
            {
                pathNeedsUpdate = false;
                direction = PlayerCurrentCoord - prevNode;
                if (direction == Vector2Int.zero)
                {
                    prevNode = oldPrevNode;
                    direction = PlayerCurrentCoord - prevNode;
                }

                float sqrDstBtwNodes = (prevNode - PlayerCurrentCoord).sqrMagnitude;

                //print("PathUpdate coroutine requestingPath from " + PlayerCurrentCoord + " with direction + " + direction + " and prevNode is " + prevNode + " difference = " + sqrDstBtwNodes);
                PathRequestManager.RequestPath(new PathRequest(PlayerCurrentCoord, direction, prevNode, OnPathFound));

            }
            yield return null;
        }
    }

    void OnPathFound(List<Vector2Int> path, bool isCycle)
    {
        if (IsSimilarPath(CurrentPath, path))
        {
            pathIsCycle = isCycle;
            return;
        }

        CurrentPath = path;
        pathIsCycle = isCycle;
        followingPath = true;
        if (StartFollowPath != null)
        {
            StopCoroutine(StartFollowPath);
        }

        StartFollowPath = FollowPath();
        StartCoroutine(StartFollowPath);

    }

    IEnumerator FollowPath()
    {
        int index = 0;
        while (followingPath)
        {
            if (index == CurrentPath.Count - 1 && !pathIsCycle)
            {
                playerAnimator.SetFloat(animatorSpeedID, 0);
                followingPath = false;
                break;
            }

            index = (index + 1) % CurrentPath.Count;

            Vector2Int currentWaypoint = CurrentPath[index];
            Vector3 currTarget = new Vector3(currentWaypoint.x, .12f, currentWaypoint.y);
            checkPointForChaser = currTarget;
            playerAnimator.SetFloat(animatorSpeedID, 1f);

            if (currentWaypoint == origin)
            {
                transferringLoot = true;
            }

            while (Vector3.Distance(transform.position, currTarget) > .05f)
            {

                if (followingPath && !pathNeedsUpdate)
                {
                    Vector3 targetDir = (currTarget - transform.position).normalized;
                    var targetAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
                    Vector3 rotation = targetAngle * Vector3.up;

                    transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), rotationSpeed * Time.deltaTime);
                    transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.Self);
                    yield return new WaitForEndOfFrame();
                }

                yield return null;
            }
            PlayerCurrentCoord = currentWaypoint;

            yield return null;

            if (playerCoord == origin)
            {
                if (StationReached != null)
                {
                    playerAnimator.SetFloat(animatorSpeedID, 0);
                    StationReached(ContinueMovement);

                }
                yield return new WaitWhile(() => transferringLoot);
            }

        }

        yield return new WaitForSeconds(timerThreshold);
        if (!followingPath)
        {
            if (PathEnded != null)
            {
                print("Path Ended");
                PathEnded();
            }
        }
        yield return null;
    }

    void ContinueMovement()
    {
        transferringLoot = false;
        pathNeedsUpdate = true;
    }

    void PrintPath(List<Vector2Int> path)
    {
        String result = "";
        foreach (Vector2Int v in path)
        {
            result += v + ", ";
        }
        print("Received Path: " + result);
    }

    public void PathNeedsUpdate(Vector2Int changedCoords)
    {
        if (!transferringLoot && !isLeavingRoom)
            pathNeedsUpdate = true;

    }

    void IsLeavingRoom()
    {
        isLeavingRoom = true;
    }

    bool IsSimilarPath(List<Vector2Int> p1, List<Vector2Int> p2)
    {
        if (p1.Count != p2.Count)
        {
            return false;
        }

        for (int i = 0; i < p1.Count; i++)
        {
            float sqrtDistance = (p2[i] - p1[i]).sqrMagnitude;
            if (sqrtDistance > .05f)
            {
                return false;
            }
        }
        return true;
    }

    void OnCollidedWithTail()
    {
        if (StartFollowPath != null)
        {
            playerAnimator.SetFloat(animatorSpeedID, 0f);
            StopCoroutine(StartFollowPath);
        }
        if (PathEnded != null)
        {
            PathEnded();
        }
    }

    void StartAfterTransgression()
    {
        isLeavingRoom = false;
        StartCoroutine(UpdatePath());
    }

    public void Transgress(Vector3 position, float yAxis)
    {
        if (StartFollowPath != null)
        {
            StopCoroutine(StartFollowPath);
        }

        StopCoroutine(UpdatePath());

        transform.position = position;
        transform.rotation = Quaternion.AngleAxis(yAxis, transform.up);
        CurrentPath.Clear();

        PlayerCurrentCoord = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        Vector2Int firstNode = PlayerCurrentCoord + new Vector2Int(Mathf.RoundToInt(transform.forward.x), Mathf.RoundToInt(transform.forward.z));
        PlayerCurrentCoord = firstNode;
        checkPointForChaser = transform.position;

        // Эту штуку запустить, когда сыграны все анимации трансгрессии, счетчики итп.
        Invoke("StartAfterTransgression", 1f);

    }

}
