using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/SearchNeighbourAction")]
public class SearchNeighboursAction : ActionBase
{
    public override void Act(StateController controller)
    {

        Tile[,] puzzle = controller.puzzleMgr.puzzle;
        List<Vector3> candidates = new List<Vector3>();

        //если мы не у левого края
        if ((int)controller.targetCoords.x != 0)
        {
            Vector3 c = new Vector3(controller.targetCoords.x - 1, controller.targetCoords.y, controller.targetCoords.z);
            if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
        }

        // если мы не на нижней кромке
        if ((int)controller.targetCoords.z != 0)
        {
            Vector3 c = new Vector3(controller.targetCoords.x, controller.targetCoords.y, controller.targetCoords.z - 1);
            if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
        }

        // если мы не у правого края
        if ((int)controller.targetCoords.x < puzzle.GetLength(0) - 1)
        {
            Vector3 c = new Vector3(controller.targetCoords.x + 1, controller.targetCoords.y, controller.targetCoords.z);
            if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
        }

        // если мы не у верхней кромки
        if ((int)controller.targetCoords.z < puzzle.GetLength(1) - 1)
        {
            Vector3 c = new Vector3(controller.targetCoords.x, controller.targetCoords.y, controller.targetCoords.z + 1);
            if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
        }

        controller.prevCoords = controller.targetCoords;
        if (candidates.Count > 0)
        {
            controller.targetCoords = candidates[Random.Range(0, candidates.Count)];
        }
        else
        {
            // проверяем диагонали слева
            if ((int)controller.targetCoords.x != 0)
            {
                if ((int)controller.targetCoords.z != 0)
                {
                    Vector3 c = new Vector3(controller.targetCoords.x - 1, controller.targetCoords.y, controller.targetCoords.z - 1);
                    if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
                }

                if ((int)controller.targetCoords.z < puzzle.GetLength(1) - 1)
                {
                    Vector3 c = new Vector3(controller.targetCoords.x - 1, controller.targetCoords.y, controller.targetCoords.z + 1);
                    if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
                }
            }

            // проверяем диагонали справа
            if ((int)controller.targetCoords.x < puzzle.GetLength(0) - 1)
            {
                if ((int)controller.targetCoords.z != 0)
                {
                    Vector3 c = new Vector3(controller.targetCoords.x + 1, controller.targetCoords.y, controller.targetCoords.z - 1);
                    if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
                }

                if ((int)controller.targetCoords.z < puzzle.GetLength(1) - 1)
                {
                    Vector3 c = new Vector3(controller.targetCoords.x + 1, controller.targetCoords.y, controller.targetCoords.z + 1);
                    if (c != controller.prevCoords && puzzle[(int)c.x, (int)c.z].isOpen) candidates.Add(c);
                }
            }



            if (candidates.Count > 0)
            {
                controller.targetCoords = candidates[Random.Range(0, candidates.Count)];
            }
            else
            {
                // выбираем произвольную свободную ячейку 
                Vector2Int randomCoords = puzzle[Random.Range(0, puzzle.GetLength(0) - 1), Random.Range(0, puzzle.GetLength(1) - 1)].Coords;
                while (!puzzle[randomCoords.x, randomCoords.y].isOpen)
                {
                    randomCoords = puzzle[Random.Range(0, puzzle.GetLength(0) - 1), Random.Range(0, puzzle.GetLength(1) - 1)].Coords;
                }
                controller.targetCoords = new Vector3(randomCoords.x, controller.targetCoords.y, randomCoords.y);
            }
        }


    }
}
