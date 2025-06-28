using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSwapWeapon : BTNode
{
    /*
     * This experimental node was added to see if agents can swap weapons. It allows the sniper enemy to use either a pistol or sniper rifle
     * -Tom
     */
    private IWeaponSwapper swapper; //weapon swapper interface
    private int index; //weapon index
    private float timer, duration;
    public BTSwapWeapon(BTAgent _agent, IWeaponSwapper _swapper, int _index, float _actionLockDuration = 1) //Swap weapon
    {
        agent = _agent;
        swapper = _swapper;
        index = _index;
        duration = _actionLockDuration;
    }
    public override NodeState Evaluate(BTAgent agent) //evaluate node
    {
        timer = 0;
        swapper.SwapWeapon(index);
        return NodeState.SUCCESS;
    }
}
