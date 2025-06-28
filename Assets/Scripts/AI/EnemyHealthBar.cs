using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthBar : MonoBehaviour
{
    /*
     * Script that controls the health bars of enemies. This script has multiple visual elements. Firstly, there
     * is a secondary bar that follows the primary bar in delay, making it more obvious how much damage the 
     * player has done to the enemy. As well as this, the bar increases in size when the agent is damaged, increasing
     * feedback for the player
     * - Tom
     */
    public Slider primaryHealthBar; //The slider for enemies health
    public Slider secondaryHealthBar;// a secondary slider that follows the first with a delay, purely visual
    public Slider primaryOvershieldbar; //Slider for enemy overshield (not used in the end, but still a fun visual)
    public Slider secondaryOvershieldbar; //Functions the same for the overshield bar as the secondaryhealth bar does
    public GameObject uiElements; //All ui elements
    private static float decayDelay = 0.75f; //delay before the secondary healthbar catches up to the primary
    private float delayTimer = 0; 
    private static float decaySpeed = .5f; //speed at which the secondary bar moves
    public Vector3 defaultScale = new Vector3(0.03f,0.03f,0.03f); // Default scale of the bar
    public Vector3 maxScale = new Vector3(0.04f,0.04f,0.04f); //max scale of the bar
    private float scaleDecayTimer = 0; 
    private static float scaleDecaySpeed = 4; //speed at which the scale decays back to normal
    public Character agent; //character attached to this healthbar
    private BTAgent btAgent; //btagent attached to this healthbar
    public bool alwaysVisible = false; //used for bosses, minibosses and elites
    private bool hasOvershield = false; //whether to display overshield
    void Awake() //reset stats and attach agent
    {
        primaryHealthBar.value = 1;
        secondaryHealthBar.value = 1;
        uiElements.SetActive(alwaysVisible);
        primaryOvershieldbar.gameObject.SetActive(false);
        secondaryOvershieldbar.gameObject.SetActive(false);
        if(agent.TryGetComponent<BTAgent>(out BTAgent _agent))
        {
            btAgent = _agent;
        }
        else
        {
            return;
        }
        hasOvershield = btAgent.maxOvershield > 0;
        primaryOvershieldbar.gameObject.SetActive(hasOvershield);
        secondaryOvershieldbar.gameObject.SetActive(hasOvershield);
    }
    void Update()
    {
        //Reset rotation
        transform.rotation = Quaternion.identity;
        //Decay scale
        DecayScale();
        //Update values
        primaryHealthBar.value = agent.GetHealth() / agent.GetMaxHealth();
        if(hasOvershield)
        {
            primaryOvershieldbar.value = btAgent.currOvershield / btAgent.maxOvershield;
        }
        //Delay decay
        if(primaryHealthBar.value == secondaryHealthBar.value)
        {
            if(!hasOvershield)
            {
                uiElements.SetActive(false);
            }
            else if(primaryOvershieldbar.value == secondaryOvershieldbar.value)
            {
                uiElements.SetActive(false);
            }
        }
        delayTimer += Time.deltaTime;
        if(delayTimer < decayDelay)
        {
            return;
        }
        UpdateHealthBar(primaryHealthBar,secondaryHealthBar); //update healthbars
        if(btAgent == null)
        {
            primaryOvershieldbar.gameObject.SetActive(false);
            secondaryOvershieldbar.gameObject.SetActive(false);
            return;
        }
        else if(btAgent.currOvershield <= 0)
        {
            primaryOvershieldbar.gameObject.SetActive(false);
            secondaryOvershieldbar.gameObject.SetActive(false);
            return;
        }
        if(hasOvershield) //update overshields
        {
            UpdateHealthBar(primaryOvershieldbar,secondaryOvershieldbar);
        }
    }
    private void UpdateHealthBar(Slider _primaryBar, Slider _secondaryBar) //update bar slider values
    {
        if(_secondaryBar.value <= _primaryBar.value)
        {
            _secondaryBar.value = _primaryBar.value;
            return;
        }
        _secondaryBar.value -= Time.deltaTime * decaySpeed;
    }
    private void DecayScale() //decay scale 
    {
        scaleDecayTimer += Time.deltaTime * scaleDecaySpeed;
        if(scaleDecayTimer >= 1){scaleDecayTimer = 1;}
        transform.localScale = Vector3.Lerp(maxScale,defaultScale,scaleDecayTimer);
    }
    public void ApplyDamage() //Allows attached agent to refresh this script and show damage
    {
        transform.localScale = maxScale;
        uiElements.SetActive(true);
        delayTimer = 0;
        scaleDecayTimer = 0;
    }
    
}
