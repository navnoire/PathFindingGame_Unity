using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/NewTargetSearch")]
public class SearchNewTargetAction : ActionBase
{
    public override void Act(StateController controller)
    {
        switch (controller.type)
        {
            case EnemyType.rotator:
                Tile[,] puzzle = controller.puzzleMgr.puzzle;
                puzzle[(int)controller.targetCoords.x, (int)controller.targetCoords.z].isOpen = true;

                Vector2Int coords = new Vector2Int(Random.Range(0, puzzle.GetLength(0)), Random.Range(0, puzzle.GetLength(1)));
                while (!puzzle[coords.x, coords.y].isOpen)
                {
                    coords = new Vector2Int(Random.Range(0, puzzle.GetLength(0)), Random.Range(0, puzzle.GetLength(1)));
                }
                controller.targetCoords = new Vector3(coords.x, controller.transform.position.y, coords.y);
                puzzle[coords.x, coords.y].isOpen = false;
                break;

            case EnemyType.robber:

                List<Item> gems = controller.puzzleMgr.currentRoomGems;
                if (gems.Count > 0)
                {
                    controller.animator.SetTrigger("DeactivateCone");
                    foreach (Item gem in gems)
                    {
                        if (!gem.isRobbered && gem.gameObject.activeInHierarchy)
                        {
                            controller.targetItem = gem;
                            break;
                        }
                    }

                    if (controller.targetItem != null)
                    {
                        controller.targetCoords = new Vector3(controller.targetItem.transform.position.x, controller.transform.position.y, controller.targetItem.transform.position.z);
                    }
                }
                break;
        }
    }

}
