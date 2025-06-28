using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode
{
    /*
        Base node of the Behaviour tree AI pattern.
        -Tom
    */
    public enum NodeState //node states
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
    protected BTAgent agent; //agent that the tree is connected to
    protected NodeState state; //cyrrebt state
    public BTNode parentNode; 
    protected List<BTNode> childNodes = new List<BTNode>(); //any child nodes
    //Blank constructor
    public BTNode()
    {
        parentNode = null;
    }
    public BTNode(BTAgent _agent)
    {
        agent = _agent;
    }
    //Constructor that passes child nodes
    public BTNode(BTAgent _agent ,List<BTNode> _childNodes)
    {
        agent = _agent;
        foreach(BTNode childNode in _childNodes)
        {
            childNodes.Add(childNode);
        }
    }
    public virtual void DeInitialiseNode(BTAgent agent) { }
    public virtual NodeState Evaluate(BTAgent agent){return NodeState.FAILURE;} //evaluate node
    public virtual void UpdateNode(BTAgent agent){}
}
