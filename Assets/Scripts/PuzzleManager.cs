using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using NavnoireCoding.Managers;

[RequireComponent(typeof(PuzzleGenerator))]
public class PuzzleManager : MonoBehaviour
{
    public Vector3 station;
    public Tile[,] puzzle;
    public Transform[] spawnpointsForEnemies;
    public List<Item> currentRoomGems;

    List<Enemy> enemies;
    List<int> indexesBag;
    Dictionary<int, List<ItemGhost>> allGemsStatus;
    int roomCounter;
    int currentRoomIndex;


    GeneralPooler pool;
    PuzzleGenerator pg;
    ItemGenerator ig;

    void Awake()
    {
        pg = GetComponent<PuzzleGenerator>();
        ig = GetComponent<ItemGenerator>();
        pool = GeneralPooler.Instance;
        enemies = new List<Enemy>();
        indexesBag = ig.GenerateBag(GameManager.Instance.levelData.levelGoal, GameManager.Instance.levelData.GemMultiplier);
        allGemsStatus = new Dictionary<int, List<ItemGhost>>();
    }

    private void OnEnable()
    {
        Transgressor.ReloadEvent += GeneratePuzzleFromData;
        Tile.OnTileRotated += SweepOneTileConnections;
        Item.OnItemCollected += UpdateGemsList;
        Player.StationReached += ClearGlobalGemstorage;
    }

    private void OnDisable()
    {
        Transgressor.ReloadEvent -= GeneratePuzzleFromData;
        Tile.OnTileRotated -= SweepOneTileConnections;
        Item.OnItemCollected -= UpdateGemsList;
        Player.StationReached -= ClearGlobalGemstorage;
    }

    public void GeneratePuzzleFromData(int roomIndex)
    {
        int currentLevelIndex = GameManager.Instance.levelData.levelIndex;
        currentRoomIndex = roomIndex;
        RoomData_SO currentRoomData = GameManager.Instance.levelData.rooms[currentRoomIndex];
        station = currentRoomData.station;

        //сначала ищем в сохраненных на диске(может в этой комнате мы уже были?)
        PuzzleData savedData = GameManager.Instance.serializator.LoadPuzzleData(currentRoomIndex, currentLevelIndex);

        //// если назначен реплей то ищем в сохраненных внутри игры (вне игры их не будет)
        //if (GameManager.Instance.IsReplay || savedData == null)
        //{
        //    savedData = GameManager.Instance.levelData.puzzleDatas.Find(d => d.roomNumber == currentRoomIndex);
        //}

        //если нашли - подгружаем
        if (savedData != null)
        {
            puzzle = pg.ReloadPuzzle(savedData, currentRoomData.portals);
            RecreateGems(roomIndex, savedData);
        }

        // если не нашли - генерируем заново
        else
        {
            print("Generating Room From Scratch");
            roomCounter++;
            int gemsInCurrentRoom = indexesBag.Count / (GameManager.Instance.levelData.rooms.Length - (roomCounter - 1));
            List<int> gemsToPlace = roomCounter == GameManager.Instance.levelData.rooms.Length ? indexesBag : indexesBag.GetRange(0, gemsInCurrentRoom);

            puzzle = pg.GeneratePuzzle(station, currentRoomData.portals, gemsToPlace, PopulateCurrentRoomGemsList);
            PopulateGlobalGemStorage(currentRoomGems, roomIndex); // Заносим в глобальное хранилище гемов
            indexesBag.RemoveRange(0, gemsToPlace.Count);

            SetRandomRotationOnTiles();
        }

        SpawnEnemies(currentRoomIndex);
        SweepAllConnections(puzzle);
    }

    //--------------------GEMS SECTION-------------------------//
    void PopulateCurrentRoomGemsList(List<Item> list)
    {
        currentRoomGems = list;
    }

    void PopulateGlobalGemStorage(List<Item> list, int room)
    {
        if (allGemsStatus.ContainsKey(room))
        {
            for (int i = 0; i < list.Count; i++)
            {
                ItemGhost item = new ItemGhost(list[i].PoolIndex, list[i].coords, list[i].isVisible);
                allGemsStatus[room][i] = item;
            }
        }
        else
        {
            List<ItemGhost> statuses = new List<ItemGhost>();
            foreach (var i in list)
            {
                ItemGhost item = new ItemGhost(i.PoolIndex, i.coords, i.isVisible);
                statuses.Add(item);
            }
            allGemsStatus.Add(room, statuses);
        }
    }

    // убираемна станции те итемы, которые уже точно собраны и обратно при реборне не вернутся во всех комнатах
    void ClearGlobalGemstorage(Action callback)
    {
        for (int i = 0; i < allGemsStatus.Keys.Count; i++)
        {
            currentRoomGems.RemoveAll(g => g.isVisible == false);
            allGemsStatus[i].RemoveAll(g => g.isVisible == false);
        }
    }

    void UpdateGemsList(Vector2Int pos, int poolIndex)
    {
        int index = allGemsStatus[currentRoomIndex].FindIndex(i => i.coords == pos);
        //currentRoomGems[index].isVisible = false;
        allGemsStatus[currentRoomIndex][index].isVisible = false;
    }

    void RecreateGems(int roomIndex, PuzzleData data)
    {
        List<Item> items;

        //if (!GameManager.Instance.IsReplay || allGemsStatus.ContainsKey(roomIndex))
        //{
        //    // получаем итемы, которые хранятся в общем котле и используем
        //    items = new List<Item>();
        //    for (int i = 0; i < allGemsStatus[roomIndex].Count; i++)
        //    {
        //        Item item = pool.GetFromPool(allGemsStatus[roomIndex][i].poolIndex).GetComponent<Item>();
        //        item.transform.position = new Vector3(allGemsStatus[roomIndex][i].coords.x, .15f, allGemsStatus[roomIndex][i].coords.y);
        //        item.transform.rotation = Quaternion.identity;
        //        item.transform.localScale = Vector3.one;
        //        item.coords = allGemsStatus[roomIndex][i].coords;
        //        item.isVisible = allGemsStatus[roomIndex][i].isVisible;
        //        item.gameObject.SetActive(allGemsStatus[roomIndex][i].isVisible);
        //        items.Add(item);
        //    }
        //    print("Gems from global storage");
        //}
        //else
        //{
        // берем сохраненные ранее данные с генерации уровня, достаем из пула объекты и связываем вместе
        items = new List<Item>();
        for (int p = 0; p < data.itemPositions.Length; p++)
        {
            Item i = pool.GetFromPool(data.itemPoolIndexes[p]).GetComponent<Item>();
            i.transform.position = new Vector3(data.itemPositions[p].x, 0.15f, data.itemPositions[p].y);
            i.transform.rotation = Quaternion.identity;
            i.transform.localScale = Vector3.one;
            i.coords = data.itemPositions[p];
            i.isVisible = true;
            i.gameObject.SetActive(true);
            items.Add(i);
        }
        // вносим в глобальное хранилище, чтобы можно было манипулировать всеми итемами вместе
        PopulateGlobalGemStorage(items, currentRoomIndex);
        print("Gems from save");
        //}
        PopulateCurrentRoomGemsList(items);
    }

    // при реборне восстанавливаем статус тех итемов, которые собраны, но не сброшены на станции
    // потом эти данные будут использованы в RecreateGems()
    public void RebornGemsStatus()
    {
        for (int i = 0; i < allGemsStatus.Keys.Count; i++)
        {
            for (int index = 0; index < allGemsStatus[i].Count; index++)
            {
                allGemsStatus[i][index].isVisible = true;
            }
        }
        //print("Reborn to " + string.Join(", ", allGemsStatus[currentRoomIndex].Select(i => i.isVisible).ToList()));

    }

    // Перемешиваем существующие на поле сокровища и размещаем по новым случайным координатам после нажатия кнопки на экране
    public void ShuffleGems()
    {
        List<Item> robberedItems = currentRoomGems.FindAll(i => i.isRobbered == true);
        currentRoomGems.RemoveAll((Item obj) => obj.isRobbered == true);

        if (currentRoomGems.Count == 0)
        {
            print("There are no gems to shuffle");
            return;
        }

        ig.ShuffleItems(currentRoomGems);

        foreach (var item in currentRoomGems)
        {
            puzzle[item.coords.x, item.coords.y].isOpen = true;
        }

        currentRoomGems.AddRange(robberedItems);

    }

    //----------------------- TILE SECTION-------------------------//
    // метод для обновления связей между всеми тайлами в пазле(применять на входе в комнату сразу после генерации или релоада)
    public void SweepAllConnections(Tile[,] puzzle)
    {
        if (puzzle == null)
        {
            return;
        }

        foreach (var item in puzzle)
        {
            item.connectedWith.Clear();
        }

        for (int w = 0; w < puzzle.GetLength(0); w++)
        {
            for (int h = 0; h < puzzle.GetLength(1); h++)
            {
                // check left 
                if (w != 0)
                {
                    if (puzzle[w, h].values[2] && puzzle[w - 1, h].values[0])
                    {
                        puzzle[w, h].connectedWith.Add(puzzle[w - 1, h].Coords);
                        puzzle[w - 1, h].connectedWith.Add(puzzle[w, h].Coords);
                    }
                }

                //check bottom
                if (h != 0)
                {
                    if (puzzle[w, h].values[3] && puzzle[w, h - 1].values[1])
                    {
                        puzzle[w, h].connectedWith.Add(puzzle[w, h - 1].Coords);
                        puzzle[w, h - 1].connectedWith.Add(puzzle[w, h].Coords);
                    }
                }
            }
        }
    }

    // обновление связей одного тайла и его соседей (применять при каждом повороте тайла)
    public void SweepOneTileConnections(Vector2Int coords)
    {
        puzzle[coords.x, coords.y].connectedWith.Clear();

        //left border
        if (coords.x != 0)
        {
            puzzle[coords.x - 1, coords.y].connectedWith.Remove(coords);
            if (puzzle[coords.x, coords.y].values[2] && puzzle[coords.x - 1, coords.y].values[0])
            {
                puzzle[coords.x - 1, coords.y].connectedWith.Add(coords);
                puzzle[coords.x, coords.y].connectedWith.Add(puzzle[coords.x - 1, coords.y].Coords);
            }
        }

        //bottom
        if (coords.y != 0)
        {
            puzzle[coords.x, coords.y - 1].connectedWith.Remove(coords);
            if (puzzle[coords.x, coords.y].values[3] && puzzle[coords.x, coords.y - 1].values[1])
            {
                puzzle[coords.x, coords.y - 1].connectedWith.Add(coords);
                puzzle[coords.x, coords.y].connectedWith.Add(puzzle[coords.x, coords.y - 1].Coords);
            }
        }

        //right border
        if (coords.x != puzzle.GetLength(0) - 1)
        {
            puzzle[coords.x + 1, coords.y].connectedWith.Remove(coords);
            if (puzzle[coords.x, coords.y].values[0] && puzzle[coords.x + 1, coords.y].values[2])
            {
                puzzle[coords.x + 1, coords.y].connectedWith.Add(coords);
                puzzle[coords.x, coords.y].connectedWith.Add(puzzle[coords.x + 1, coords.y].Coords);
            }
        }

        //top
        if (coords.y != puzzle.GetLength(1) - 1)
        {
            puzzle[coords.x, coords.y + 1].connectedWith.Remove(coords);
            if (puzzle[coords.x, coords.y].values[1] && puzzle[coords.x, coords.y + 1].values[3])
            {
                puzzle[coords.x, coords.y + 1].connectedWith.Add(coords);
                puzzle[coords.x, coords.y].connectedWith.Add(puzzle[coords.x, coords.y + 1].Coords);
            }
        }
    }

    private void SpawnEnemies(int currRoomIndex)
    {
        enemies.Clear();
        RoomData_SO.EnemiesInfo[] enemyList = GameManager.Instance.levelData.rooms[currRoomIndex].enemiesInTheRoom;

        foreach (RoomData_SO.EnemiesInfo item in enemyList)
        {
            for (int i = item.amount; i > 0; i--)
            {
                Transform spawnPoint = spawnpointsForEnemies[UnityEngine.Random.Range(0, spawnpointsForEnemies.Length)];
                Enemy newEnemy = pool.GetFromPool((int)item.type).GetComponent<Enemy>();
                newEnemy.transform.position = spawnPoint.position;
                newEnemy.transform.rotation = spawnPoint.rotation;
                newEnemy.m_instance = newEnemy.gameObject;
                newEnemy.m_spawnPoint = spawnPoint;
                newEnemy.gameObject.SetActive(true);
                enemies.Add(newEnemy);

            }

        }
    }


    void SetRandomRotationOnTiles()
    {
        foreach (var item in puzzle)
        {
            if (!item.isUntouchable)
            {
                int rand = UnityEngine.Random.Range(0, 4);
                for (int i = 0; i <= rand; i++)
                {
                    item.RotateTile();
                }
            }
            else
            {
                if (item.Coords.y == puzzle.GetLength(1) - 1)
                {
                    item.RotateTile();
                    item.RotateTile();
                }

                if (item.Coords.x == 0)
                {
                    item.RotateTile();
                }

                if (item.Coords.x == puzzle.GetLength(0) - 1)
                {
                    item.RotateTile();
                    item.RotateTile();
                    item.RotateTile();
                }
            }
        }
    }

    public void DisableTileRotation()
    {
        foreach (Tile item in puzzle)
        {
            item.isRotatable = false;
        }
    }

    public void EnableTileRotation()
    {
        foreach (Tile item in puzzle)
        {
            item.isRotatable = true;
        }
    }
}
