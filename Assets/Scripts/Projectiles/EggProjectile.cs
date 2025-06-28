using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggProjectile : Projectile
{
    protected GameObject chick = null;
    protected float orbitRadius = 5.0f;

    public Projectile InitChicks(GameObject a_chick, float a_orbit)
    {
        chick = a_chick;
        orbitRadius = a_orbit;

        return this;
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null)
    {
        // Attempt to spawn a chick
        if (SpecialsManager.instance.chicks.Count < SpecialsManager.instance.numChicks)
        {
            GameObject temp = Instantiate(chick, transform.position, transform.rotation);
            if (SpecialsManager.instance.chicks.Count < 1)
            {
                temp.GetComponent<Chick>().chickNum = 0;
                SpecialsManager.instance.chicks.Add(temp);
            }
            else
            {
                for (int i = 0; i < SpecialsManager.instance.chicks.Count + 1; i++)
                {
                    bool numCheck = false;
                    foreach (GameObject c in SpecialsManager.instance.chicks)
                    {
                        if (c.GetComponent<Chick>().chickNum == i)
                        {
                            numCheck = true;
                        }
                    }
                    if (!numCheck)
                    {
                        float tempNum = (SpecialsManager.instance.chicks[0].GetComponent<Chick>().chickNum - i) * -(Mathf.PI*2 / SpecialsManager.instance.numChicks);
                        temp.GetComponent<Chick>().chickNum = i;
                        temp.GetComponent<Chick>().chickTimer = SpecialsManager.instance.chicks[0].GetComponent<Chick>().chickTimer + tempNum;
                        SpecialsManager.instance.chicks.Add(temp);
                        break;
                    }
                }
            }
        }

        base.DestroyProjectile();
    }
}
