using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// STATUS EFFECTS
/*
 * Each status effect has 2 types, a "type" and an "effect"
 * The "type" is the type of effect it will have on the character (like damaging them, slowing them, etc.)
 * The "effect" is the actual in-game effect the character will experience (like burning, poisoning, etc.)
 * 
 * ------- mucho texto warning: futher improvements that could be made to the status effect system
 * 
 * The management for status effects is handled by the character, not the status effect itself (intentional)
 * 
 * There are a lot of for loops and nested if statements involved with this
 * This is mainly done when checking if there are 2 status effects and one needs to be overwritten
 * At some point this should probably be optimised
 * But since there probably won't be more than 3 effects at once anyway it should be fine
 * Makes the code look awful too
 * 
 * Could maybe utilise pointers/references of the characters isBurning or isPoisoned flags
 * Then check to see if 2 statuses have the same reference
 * Therefore they would know if there are 2 of the same status "effect"
 * 
 * TL;DR we should use pointers, but they dont exist
 * 
 * */

public class StatusEffect
{
    private Character character; // The character that has the status applied to it
    private StatusType type; // Type of status effect
    private EffectType effect; // Effect of status effect
    private float power; // How much power the effect has (i.e. damage per tick, speed modifier, etc.)

    private float maxTime; // Total length of the status effect
    private float timer; // How long the status effect has left
    private float tickTime; // Time between each event tick (i.e. damage)
    private float nextTickTime; // When the next tick event should happen

    private bool isOver = false; // True when the status effect is considered over

    public StatusEffect(StatusType a_type, EffectType a_effect, Character a_char, float a_power, float a_maxTime, float a_tickTime)
    {
        character = a_char;
        type = a_type;
        effect = a_effect;
        power = a_power;

        maxTime = a_maxTime;
        timer = a_maxTime;
        tickTime = a_tickTime;
        nextTickTime = a_maxTime;
    }

    public float Update()
    {
        // Default return value, used if any status effect needs to return something back to the character script
        float returnValue = 0.0f;

        // Check if status effect is over
        if (timer <= 0.0f) isOver = true;

        // Determining which update function to run based off type
        switch (type) {
            case StatusType.Damage:
                DamageUpdate();
                break;
            case StatusType.Speed:
                SpeedUpdate();
                break;
            case StatusType.inputLocked:

            default:
                break;
        }

        // Decrease timer
        timer -= Time.deltaTime;

        // Return
        return returnValue;
    }

    // Update code specific for damage type status effects
    private void DamageUpdate()
    {
        // If it is time for damage, then do damage
        if (nextTickTime >= timer)
        {
            character.Damage(power,false,true,false,effect);
            nextTickTime = timer - tickTime;
        }
    }
    private void SpeedUpdate()
    {
        // Add speed modifier if status effect has just started
        if (timer == maxTime)
        {
            character.AddModifier(ModifierType.Speed, power);
        }
        // Remove modifier if status effect has ended
        if (isOver)
        {
            character.RemoveModifier(ModifierType.Speed, power);
        }
    }
    // Returns true when effect is over, so character can remove status from self
    public bool IsOver()
    {
        return isOver;
    }
    // Returns amount of time remaining left on this status effect
    public float TimeRemaining()
    {
        return timer;
    }
    // Returns this status' type (i.e. damage, speed)
    public StatusType GetStatusType()
    {
        return type;
    }
    // Returns this status' effect (i.e. burn, poison)
    public EffectType GetEffectType()
    {
        return effect;
    }
    // Returns true if provided status effect and type are the same
    public bool MatchingStatus(StatusEffect newEffect)
    {
        return newEffect.GetStatusType() == type && newEffect.GetEffectType() == effect;
    }
    public bool MatchingStatus(StatusType a_type, EffectType a_effect)
    {
        return a_type == type && a_effect == effect;
    }
}

public enum StatusType // What type of effect the status would have on the character
{
    Damage,
    Speed,
    inputLocked,
}
public enum EffectType // The type of source of the status effect
{
    None, // useful for a generic effect (temp. speed boost or whatever)
    Burn,
    Poison,
    Stun
}