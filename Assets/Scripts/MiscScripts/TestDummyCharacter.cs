using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDummyCharacter : Character
{
    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(debugButton != null){debugButton.text = GetHealth().ToString();}

        if (Input.GetKeyDown(KeyCode.P))
        {
            float testNumber = 2.1f;
            string testString = "Number: {number}, Percent: {percentage}";
            GameObject floatingText = Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.FLOATING_TEXT), PlayerController.instance.transform.position + Vector3.up * 1.5f + Vector3.one * Random.Range(-0.2f, 0.2f), Quaternion.Euler(Vector3.zero));
            floatingText.GetComponent<ItemPickupFade>().SetText(testString.Replace("{number}",((int)testNumber).ToString()).Replace("{percentage}",((int)(testNumber*100)).ToString()+"%"));
        }
    }

    protected override void OnDamage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false)
    {
        //GameObject floatingText = Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.FLOATING_TEXT), transform.position + Vector3.up * 1.5f + Vector3.one * Random.Range(-0.2f, 0.2f), Quaternion.Euler(Vector3.zero));
        //floatingText.GetComponent<ItemPickupFade>().SetText(dmg.ToString());
        Debug.Log(name + ": RECIEVED " + dmg.ToString() + " DAMAGE");
    }
}
