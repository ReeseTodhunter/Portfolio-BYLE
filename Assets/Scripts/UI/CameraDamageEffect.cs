using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraDamageEffect : MonoBehaviour
{
    public static CameraDamageEffect instance;
    public Image damageFlash;
    [Range(0,1)]
    public float maxOpacity = 0.33f;
    public float decaySpeed = 1;
    public float decayTimer = 0;
    private float minOpacity = 0;
    private Color col = Color.red;
    public void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
        col.a = 0;
        damageFlash = this.GetComponent<Image>();
        damageFlash.color = col;
        decayTimer = 1;
    }
    void FixedUpdate()
    {
        decayTimer += Time.deltaTime * decaySpeed;
        if(decayTimer >= 1){decayTimer = 1;}
        col.a = Mathf.Lerp(maxOpacity, minOpacity, decayTimer);
        damageFlash.color = col;
        float percentage = 1 - (PlayerController.instance.GetHealthRatio());
        if(percentage < 0.3f){return;}
        minOpacity = percentage * maxOpacity;
    }
    public void ActivateFlash()
    {
        decayTimer = 0;
    }
    public void SetMinOpacity()
    {
        minOpacity = 0;
    }
}
