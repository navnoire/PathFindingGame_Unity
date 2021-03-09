using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/StopDecision")]
public class StopDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        if (!controller.navMeshAgent.pathPending)
        {
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.radius * 2)
            {
                switch (controller.type)
                {
                    case EnemyType.rotator:
                        controller.animator.SetBool("Rotated", false);
                        break;
                    case EnemyType.robber:
                        if (controller.targetItem.gameObject.activeInHierarchy && !controller.targetItem.transform.IsChildOf(controller.transform) && !controller.targetItem.isRobbered)
                        {
                            controller.targetItem.transform.parent = controller.transform.GetChild(0).GetChild(1);
                            controller.targetItem.transform.localPosition = Vector3.zero;
                            controller.targetItem.transform.localScale = new Vector3(1, 1, 1);
                            controller.targetItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                            controller.targetItem.isRobbered = true;
                            controller.animator.SetTrigger("ActivateCone");
                        }
                        break;
                }
            }

            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance)
            {
                if (!controller.navMeshAgent.hasPath || controller.navMeshAgent.velocity.sqrMagnitude == 0f)
                {

                    return true;
                }
            }
        }
        return false;
    }
}
