using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/RandomChoiceDecision")]
public class RandomChoiceDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        float rand = Random.value;
        return rand > .7f;
    }
}
