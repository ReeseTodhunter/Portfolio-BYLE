using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOB_AI : MonoBehaviour
{
    // GOP STUFF, NOT USED // Tom
    protected Action currentAction = null;
    protected bool performingAction = false, Initialised = false;
    protected List<Action> actions = new List<Action>();

    public enum animState{running,idle,attack};
    public virtual void InitialiseAgent()
    {
    }
    public virtual void ChangeAnimState(animState newState)
    {
        
    }
    public virtual void UpdateAI()
    {
        if(!Initialised){return;}
        if(currentAction == null)
        {
            currentAction = GetBestAction(actions);
            currentAction.BeginAction();
        }
        if(performingAction)
        {
            currentAction.UpdateAction();
            return;
        }
        currentAction.ExitAction();
        currentAction = GetBestAction(actions);
        currentAction.BeginAction();
    }
    protected Action GetBestAction(List<Action> actionList)
    {
        float score = 0, highscore = 0;
        Action bestAction = null;
        foreach(Action currentAction in actionList)
        {
            if(!currentAction.IsActionPossible()){continue;}
            score = currentAction.GetActionScore();
            if(score > highscore)
            {
                highscore = score;
                bestAction = currentAction;
            }
        }
        return bestAction;
    }
    public void SetPerformingAction(bool _isPerformingAction)
    {
        performingAction = _isPerformingAction;
    }
}
