using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorEntity : Character
{
    /*
     * Class that controls the riot shield enemies shield. A very stripped down version of the BTAgent class
     */
    public EnemyHealthBar healthBar;
    private ShieldModifier modifier = null;
    public DamageFlash flash;
    public GameObject deathParticles = null;
    public bool DestroyOnDeath = true;
    void Start()
    {
        CharacterAwake();
    }
    void Update()
    {
        CharacterUpdate();
    }
    protected override void Die()
    {
        if(modifier != null)
        {
            modifier.ShieldDestroyed();
        }
        if(deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }
        if(!DestroyOnDeath)
        {
            this.GetComponent<Collider>().enabled = false;
            this.gameObject.SetActive(false);
            return;
        }
        Destroy(this.gameObject);
    }
    protected override void OnDamage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false)
    {
        healthBar.ApplyDamage();
        flash.StartFlash();
        base.OnDamage(dmg, ignoreImmunity, grantImmunity);
    }
    public void SetListeningModifier(ShieldModifier _modifier)
    {
        modifier = _modifier;
    }
}
