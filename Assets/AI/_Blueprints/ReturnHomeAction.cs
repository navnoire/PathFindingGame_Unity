using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ReturnHome")]
public class ReturnHomeAction : ActionBase
{
    public override void Act(StateController controller)
    {
        switch (controller.type)
        {
            case EnemyType.robber:
                controller.animator.SetTrigger("DeactivateCone");
                break;
            case EnemyType.rotator:
                break;
        }
        controller.targetCoords = controller.GetComponent<Enemy>().m_spawnPoint.position;
        controller.targetItem = null;
        controller.aiActive = false;
    }
}
