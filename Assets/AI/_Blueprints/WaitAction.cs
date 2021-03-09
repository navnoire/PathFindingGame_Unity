using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ShortWait")]
public class WaitAction : ActionBase
{

    public override void Act(StateController controller)
    {
        if (!controller.timer.isRunning)
        {
            //controller.animator.SetBool("Rotated", false);
            controller.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            controller.timer.waitInterval = Random.Range(controller.stats.longWaitInterval.minValue, controller.stats.longWaitInterval.maxValue);
            controller.timer.StartTimer();
        }

    }
}
