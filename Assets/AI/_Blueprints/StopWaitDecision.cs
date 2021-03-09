using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/StopWait Decision")]
public class StopWaitDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        if (controller.timer.CheckTimer())
        {
            controller.timer.ResetTimer();
            return true;
        }
        return false;


    }

}
