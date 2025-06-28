using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralAgent : Character
{
    /*
     * Neutral agent, useful for things like barrels or destructable objects
     * -Tom
     */
    public EnemyHealthBar heatlhbar;
    public List<BTModifier> modifiers = new List<BTModifier>(); 
    void Start()
    {
        foreach(BTModifier modifier in modifiers)
        {
            modifier.Initialise(this);
        }
    }
    protected override void Die()
    {
        EvaluateModifers(BTModifier.modifierType.onDeath);
        base.Die();
    }
    void Update()
    {
        CharacterUpdate();
    }
    protected override void OnDamage(float dmg, bool ignoreImmunity = false, bool grantImmunity = true, bool ignoreResistance = false, EffectType type = EffectType.None, bool _isCrit = false)
    {
        if(heatlhbar != null){heatlhbar.ApplyDamage();}
        EvaluateModifers(BTModifier.modifierType.onDamage);
        base.OnDamage(dmg, ignoreImmunity, grantImmunity, ignoreResistance);
    }
    private void EvaluateModifers(BTModifier.modifierType type)
    {
        foreach(BTModifier modifier in modifiers)
        {
            if(modifier.type == type){modifier.ActivateModifier(this);}
        }
    }
}
