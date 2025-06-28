using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponSwapper
{
    //Weapon swapping interface, created to see if the sniper enemy can swap weapons
    // -Tom
    public void SwapWeapon(int index);
    public float GetCurrWeapon();
}
