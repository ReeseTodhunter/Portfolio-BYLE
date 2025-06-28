using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public enum WEAPONS
    {
        UNNARMED,
        PISTOL,
        SNIPER,
        SHOTGUN,
        MINIGUN,
        BOOMERANG_GUN,
        SHATTER_CANNON,
        LASER,
        FLAME_THROWER,
        OTHER
    }
    public enum STATE
    {
        IDLE,
        RUNNING,
        SHOOTING,
        RELOADING,
        ROLLING
    }
    private WEAPONS currWeapon;
    private STATE currState;
    public void ShootWeapon()
    {

    }
    public void SwapWeapon(WEAPONS _weapon)
    {
        currWeapon = _weapon;
        //this.GetComponent<Animator>().Play("PistolSwap");
    }
}
