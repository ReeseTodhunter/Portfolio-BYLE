using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
//Base class for all weapons 
public class BaseWeapon : MonoBehaviour
{
    public int weaponID; // ID of the weapon
    public bool weaponIsHeldInPistolPos;
    #region WeaponIDs
    /*
     * 0 Basic Gun
     * 1 Ak-47
     * 2 Shotgun
     * 3 Sniper
     * 4 Flamethrower
     * 5 Minigun
     * 6 RPG
     * 7 Laser
     * 8 Boomerang Launcher
     * 9 Bounce Gun
     * 10 Shatter Cannon
     * 11 Goose Gun
     * 12 Katana
     */
    #endregion
    public List<AudioClip> fireAudioClips = null;
    public List<AudioClip> reloadAudioClips = null;
    public Texture2D weaponCursor;
    protected int audioCounter = 0;

    //Modifiers
    private float damageModifier = 1.0f;
    private float rofModifier = 1.0f;
    private float speedModifier = 1.0f;
    private float reloadSpeedModifier = 1.0f;
    private float magSizeModifier = 1.0f;

    // Description
    public string weaponName = ""; // Name of the powerup in item desc box

    [TextArea(15, 20)]
    public string weaponDescription = ""; // Description of the powerup in item desc box
    public Sprite weaponSprite = null; // Weapon icon
    protected GameObject descriptionBox;

    //State
    public enum WeaponState
    {
        EQUIPPED,
        STOWED,
        DROPPED
    }
    public WeaponState currState = WeaponState.DROPPED;

    //Virtual functions
    public virtual void OnFireOneHeld() {}
    public virtual void OnFireOneDown() {}
    public virtual void OnFireOneUp() { }
    public virtual void OnFireTwoHeld() { }
    public virtual void OnFireTwoDown() {}
    public virtual void OnFireTwoUp() { }
    public virtual void OnReloadHeld() { }
    public virtual void OnReloadDown() {}
    public virtual void OnReloadUp() { }
    public virtual void OnReloadCancel() { }
    public virtual void SetModifiers() { }
    public virtual string GetSwapAnimName(){return "";}
    public virtual string GetIdleAnimName(){return "";}
    public virtual void OnStow()
    {
        currState = WeaponState.STOWED;
        if (gameObject.TryGetComponent(out AudioSource audioSource))
        {
            audioSource.Stop();
        }
    }
    public virtual void OnEquip()
    {
        currState = WeaponState.EQUIPPED;
        //CursorController.instance.SetCursor(weaponCursor);
        GameManager.GMinstance.GetComponent<Crosshair>().SetCrosshair();
    }
    public virtual void OnPickup()
    {
        this.GetComponent<Collider>().enabled = false;
        currState = WeaponState.EQUIPPED;
        RemoveDescBox();
        if (GameManager.GMinstance.weaponsPickedUp.Length - 1 >= weaponID)
        {
            GameManager.GMinstance.weaponsPickedUp[weaponID] = true;
        }
        //CursorController.instance.SetCursor(weaponCursor);
        GameManager.GMinstance.GetComponent<Crosshair>().SetCrosshair();
    }
    public virtual void OnDrop()
    {
        currState = WeaponState.DROPPED;
        this.GetComponent<Collider>().enabled = true;
        transform.parent = null;
        Vector3 pos = transform.position;
        pos.y = 0.5f;
        transform.position = pos;
        SpawnDescBox();
    }

    public virtual void PlayAudio(List<AudioClip> audioClips)
    {
        if (audioClips.Count > 0)
        {
            //get random audio 
            int randAudio = Random.Range(0, audioClips.Count);
            //Play Reload Audio clip on reload
            if (gameObject.TryGetComponent(out AudioSource audioSource))
            {
                audioSource.clip = audioClips[randAudio];
                //audioSource.time = 0.3f;
                audioSource.volume = GameManager.GMinstance.FXVolume;
                audioSource.Play();
            }
        }
    }

    public virtual void Idle()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + (50 * Time.unscaledDeltaTime), 0);
        transform.position += (Vector3.up * Mathf.Sin(Time.time * 2) / 2) * Time.deltaTime;
    }

    public virtual bool isEquipped()
    {
        if(currState == WeaponState.EQUIPPED)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SpawnDescBox()
    {
        if(ItemPopupController.instance == null){Debug.Log("null");}
        ItemPopupController.instance.AddBox(weaponName, weaponDescription, transform.position);
    }
    public void RemoveDescBox()
    {
        if(ItemPopupController.instance == null){Debug.Log("null");}
        ItemPopupController.instance.RemoveBox(weaponName, weaponDescription, transform.position);
    }

    public string GetDescription()
    {
        return weaponDescription;
    }
}
