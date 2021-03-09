using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/New State")]
public class State : ScriptableObject
{
    public ActionBase[] actions;
    public Transition[] transitions;
    public Color stateColor = Color.gray;

    public void UpdateState(StateController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    private void DoActions(StateController controller)
    {
        foreach (ActionBase action in actions)
        {
            action.Act(controller);
        }
    }

    private void CheckTransitions(StateController controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceded = transitions[i].decision.Decide(controller);

            controller.TransitionTostate(decisionSucceded ? transitions[i].trueState : transitions[i].falseState);
        }
    }

}
