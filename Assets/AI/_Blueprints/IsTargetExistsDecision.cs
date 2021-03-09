using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetCheckDecision")]
public class IsTargetExistsDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool result = false;
        switch (controller.type)
        {
            case EnemyType.rotator:
                result = Vector3.Distance(controller.transform.position, controller.targetCoords) > 1f;
                break;
            case EnemyType.robber:
                if (controller.targetItem != null)
                {
                    if (controller.targetItem.isRobbered)
                    {
                        result = controller.targetItem.transform.IsChildOf(controller.transform);
                    }
                    else
                    {
                        result = controller.targetItem.gameObject.activeInHierarchy;
                    }
                }
                break;
        }
        return result;
    }
}
