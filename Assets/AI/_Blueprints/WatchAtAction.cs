using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/WatchAtTarget")]
public class WatchAtAction : ActionBase
{
    public override void Act(StateController controller)
    {
        if (controller.rotateInProcess == false)
        {

            controller.rotateInProcess = true;
            controller.StartCoroutine(controller.RotateSmoothly());
        }
    }
}
