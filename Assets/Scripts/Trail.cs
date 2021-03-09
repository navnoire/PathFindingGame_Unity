using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Trail : MonoBehaviour
{
    public Player player;
    public Carriage carriagePrefab; // экземпляр тележки
    public float speedCoeff = 1.2f;

    Carriage lastInChain; //последняя в списке тележка
    public List<Carriage> currentTrail; // список всех сейчас существующих тележек
    List<Vector3> pathNodes = new List<Vector3>(); //нитка с узелками, по которой движется весь хвост
    [HideInInspector] public int maxNodesamount; //сколько максимально может быть длина пути

    Transform parent;

    float moveSpeed;
    float rotationSpeed;

    void Start()
    {
        currentTrail = new List<Carriage>();
        parent = transform;

        Carriage.moveSpeed = player.moveSpeed * speedCoeff;
        Carriage.rotationSpeed = player.rotationSpeed;
        Carriage.currentPath = pathNodes;
    }

    private void OnEnable()
    {
        Item.OnItemCollected += AddCarriage;
        PlayerCollisionSensor.CollidedWithTail += StopChain;
        Transgressor.TransgressAction += TransgressTrail;

    }

    private void OnDisable()
    {
        Item.OnItemCollected -= AddCarriage;
        PlayerCollisionSensor.CollidedWithTail -= StopChain;
        Transgressor.TransgressAction -= TransgressTrail;

    }



    void AddCarriage(Vector2Int pos, int poolIndex)
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        Carriage newCarriage;

        if (pathNodes.Count >= currentTrail.Count + 1)
        {

            spawnPosition = pathNodes[currentTrail.Count + 1];
        }
        else
        {
            spawnPosition = pathNodes[currentTrail.Count];
        }

        if (currentTrail.Count == 0)
        {
            spawnRotation = player.transform.localRotation;
            newCarriage = Instantiate(carriagePrefab, spawnPosition, spawnRotation, parent);
            newCarriage.target = player.transform.Find("Crook");
        }
        else
        {
            spawnRotation = lastInChain.transform.localRotation;
            newCarriage = Instantiate(carriagePrefab, spawnPosition, spawnRotation, parent);
            newCarriage.target = lastInChain.transform;

        }
        newCarriage.pocket.GetChild(poolIndex - 5).gameObject.SetActive(true);
        lastInChain = newCarriage;
        currentTrail.Add(newCarriage);
        newCarriage.index = currentTrail.Count - 1;
        newCarriage.MoveToTarget();

    }

    public IEnumerator QueueCoords()
    {
        // опрашивает свою цель, складывает результаты в список, следит за длиной списка
        Vector3 prevPos = new Vector3(10, 20);

        while (true)
        {
            Vector3 newCoord = player.checkPointForChaser + new Vector3(0f, .23f, 0f);

            if (newCoord != prevPos)
            {
                pathNodes.Insert(0, newCoord);

                while (pathNodes.Count > maxNodesamount)
                {
                    pathNodes.RemoveAt(pathNodes.Count - 1);
                }

                prevPos = newCoord;
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    void StopChain()
    {
        foreach (var item in currentTrail)
        {
            item.IsReachedOrigin(true);

        }
    }

    public void CollectItem()
    {
        Carriage c = currentTrail.FindLast((Carriage obj) => obj != null);
        c.CarriageCollect();
        currentTrail.Remove(c);
    }

    public void TransgressTrail(Vector3 pos, float rot)
    {
        Vector3 newPos = pos - player.transform.forward + new Vector3(0f, 0.23f, 0);
        pathNodes.Clear();

        foreach (Carriage c in currentTrail)
        {

            c.transform.position = newPos;
            c.transform.localRotation = Quaternion.AngleAxis(rot, transform.up);
            pathNodes.Add(c.transform.position);
            c.currentTargetPosition = pathNodes[c.index];


        }


    }

}