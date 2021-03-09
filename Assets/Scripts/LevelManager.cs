using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using NavnoireCoding.Managers;

public class LevelManager : MonoBehaviour
{
    public Player playerPrefab;
    public Trail trailPrefab;

    Vector3 playerStartPosition;
    private Player playerInstance;


    PuzzleManager puzzleMgr;
    CollectManager collectMgr;
    public int currentLevelIndex;
    public int currentRoomIndex;
    public bool roomNeedsReload;

    private bool _isReborn;
    public bool IsReborn { get => _isReborn; set => _isReborn = value; }

    void Awake()
    {
        puzzleMgr = FindObjectOfType<PuzzleManager>();
        collectMgr = GetComponent<CollectManager>();
    }

    void OnEnable()
    {
        Transgressor.ReloadEvent += UpdateRoom;
        PortalZone.OnRoomExit += SavePuzzleDataOnExit;
    }

    void OnDisable()
    {
        Transgressor.ReloadEvent -= UpdateRoom;
        PortalZone.OnRoomExit -= SavePuzzleDataOnExit;
    }

    void Start()
    {
        StartCoroutine(GameLoop());

    }

    IEnumerator GameLoop()
    {
        yield return StartCoroutine(LevelStart());
        yield return StartCoroutine(LevelPlay());
        yield return StartCoroutine(LevelEnd());
    }

    IEnumerator LevelStart()
    {
        currentLevelIndex = GameManager.Instance.levelData.levelIndex;
        currentRoomIndex = 0;
        playerStartPosition = GameManager.Instance.levelData.rooms[currentRoomIndex].playerStartPosition;
        GeneralPooler.Instance.FillPool();
        puzzleMgr.GeneratePuzzleFromData(currentRoomIndex);
        AddPlayer(playerStartPosition, Quaternion.AngleAxis(90f, transform.up));
        playerInstance.SetupPlayer();

        // здесь нужно ждать окончания анимации 3... 2... 1...
        yield return null;
    }

    IEnumerator LevelPlay()
    {
        yield return new WaitForSeconds(1f);
        SavePuzzleDataOnEnter();



        while (true)
        {
            if (roomNeedsReload)
            {
                yield return new WaitForSeconds(1f);
                playerInstance.origin = new Vector2Int((int)puzzleMgr.station.x, (int)puzzleMgr.station.z);
                SavePuzzleDataOnEnter();
                roomNeedsReload = false;
            }

            if (_isReborn)
            {
                puzzleMgr.RebornGemsStatus();
                puzzleMgr.GeneratePuzzleFromData(currentRoomIndex);
                _isReborn = false;
            }

            yield return null;
        }

    }

    IEnumerator LevelEnd()
    {
        yield return null;
    }

    private void AddPlayer(Vector3 position, Quaternion rotation)
    {
        playerInstance = Instantiate(playerPrefab, position, rotation);
        playerInstance.PlayerCurrentCoord = new Vector2Int((int)position.x, (int)position.z);
        playerInstance.origin = new Vector2Int((int)puzzleMgr.station.x, (int)puzzleMgr.station.z);
        Trail trail = Instantiate(trailPrefab);
        trail.player = playerInstance;
        collectMgr.trail = trail;
        trail.maxNodesamount = Mathf.RoundToInt(GameManager.Instance.levelData.levelGoal * GameManager.Instance.levelData.GemMultiplier + 1);
        StartCoroutine(trail.QueueCoords());
    }

    //сохраняем исходное состояние паззла на входе в каждую комнату(на случай replay level) внутри игры в SO
    void SavePuzzleDataOnEnter()
    {
        if (!GameManager.Instance.levelData.puzzleDatas.Exists(d => d.roomNumber == currentRoomIndex))
        {
            GameManager.Instance.levelData.puzzleDatas.Add(new PuzzleData(currentRoomIndex, puzzleMgr.puzzle, puzzleMgr.currentRoomGems));
        }
    }

    // сохраняем данные на выходе из комнаты на диск (чтобы при возвращении картина была такая же)
    void SavePuzzleDataOnExit()
    {
        GameManager.Instance.serializator.SavePuzzleData(currentRoomIndex, currentLevelIndex, puzzleMgr.puzzle, puzzleMgr.currentRoomGems);
    }

    public void UpdateRoom(int index)
    {
        currentRoomIndex = index;
        roomNeedsReload = true;
    }

}
