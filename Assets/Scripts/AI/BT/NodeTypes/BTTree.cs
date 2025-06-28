using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tree : MonoBehaviour
{
    /*
        Tree class for the Behaviour tree pattern. Will evaluate the tree
        to find the action to be performed. Works hand in hand with the BTAgent
        script, which controls this script. 
        -Tom
    */
    protected BTNode _rootNode = null;
    protected BTNode _runningNode = null;
    protected BTAgent Agent;
    public bool hasRunningNode = false;
    public void InitialiseTree(BTAgent _agent)
    {
        Agent = _agent;
        _rootNode = SetupTree(_agent);
    }
    public virtual void UpdateTree()
    {
        if(_rootNode == null){return;}

        _rootNode.Evaluate(Agent);
    }
    public virtual void UpdateRunningNode()
    {
        if(_runningNode == null){return;}
        _runningNode.UpdateNode(Agent);
        return;
    }
    public virtual void DeInitialiseTree()
    {
        if(_runningNode == null) { return; }
        _runningNode.DeInitialiseNode(Agent);
    }
    public void lockActiveNode(BTNode activeNode){_runningNode = activeNode; hasRunningNode = true;}
    public void unlockActiveNode(){_runningNode = null; hasRunningNode = false;}
    protected abstract BTNode SetupTree(BTAgent agent);
}
