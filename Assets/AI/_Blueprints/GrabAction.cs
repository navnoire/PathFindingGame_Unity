using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/GrabItem")]
public class GrabAction : ActionBase
{
    public override void Act(StateController controller)
    {
        if (controller.targetItem.gameObject.activeInHierarchy && !controller.targetItem.transform.IsChildOf(controller.transform))
        {
            controller.puzzleMgr.puzzle[controller.targetItem.coords.x, controller.targetItem.coords.y].isOpen = true;
        }
    }
}
