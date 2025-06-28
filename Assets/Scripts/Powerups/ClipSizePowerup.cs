using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipSizePowerup : BasePowerup
{
    protected override bool OnPickup()
    {
        //PlayerController.instance.UpdateWeaponsModifiers();
        //MagazineUIController.instance.RefreshMagazineUI();
        return base.OnPickup();
    }
}
