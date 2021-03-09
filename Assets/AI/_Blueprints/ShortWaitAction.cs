using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Wait")]
public class ShortWaitAction : ActionBase
{

    public override void Act(StateController controller)
    {
        if (!controller.timer.isRunning)
        {
            controller.timer.waitInterval = Random.Range(controller.stats.shortWaitInterval.minValue, controller.stats.shortWaitInterval.maxValue);
            controller.timer.StartTimer();
        }

    }
}
