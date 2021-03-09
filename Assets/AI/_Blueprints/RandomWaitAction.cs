using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RandomWait")]
public class RandomWaitAction : ActionBase
{
    public override void Act(StateController controller)
    {
        if (!controller.timer.isRunning)
        {
            controller.timer.waitInterval = Random.Range(controller.stats.randomWaitInterval.minValue, controller.stats.randomWaitInterval.maxValue);
            controller.timer.StartTimer();
        }
    }
}
