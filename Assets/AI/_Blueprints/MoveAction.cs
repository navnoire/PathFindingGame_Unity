using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Move")]
public class MoveAction : ActionBase
{

    public override void Act(StateController controller)
    {
        Move(controller);
    }

    private void Move(StateController controller)
    {
        controller.navMeshAgent.destination = controller.targetCoords;
        controller.navMeshAgent.isStopped = false;

        switch (controller.type)
        {
            case (EnemyType.rotator):
                controller.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                controller.animator.SetBool("Rotated", true);
                break;
            case (EnemyType.robber):
                break;

        }
    }
}
