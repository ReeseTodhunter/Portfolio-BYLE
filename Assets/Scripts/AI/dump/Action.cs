using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    // GOB STUFF, NOT USED // Tom
    protected GOB_AI agent;
    public virtual bool IsActionPossible(){return false;}
    public virtual float GetActionScore(){return 0;}
    public virtual void BeginAction(){}
    public virtual void UpdateAction(){}
    public virtual void ExitAction(){}
}
