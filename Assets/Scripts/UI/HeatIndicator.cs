using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeatIndicator : MonoBehaviour
{
    public Slider heatSlider;
    public Color coldCol,hotCol;
    public Image heatSliderBar;
    public static HeatIndicator instance;
    public GameObject UI;
    private bool coolingDown = false;
    private float coolDownDuration = 1;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
    }
    public void SetHeat(float _heat, float _maxHeat)
    {
        float fraction =  _heat/ _maxHeat;
        heatSlider.value = fraction;
        heatSliderBar.color = Color.Lerp(coldCol,hotCol,fraction);
    }
    public void SetVisible(bool _visible)
    {
        UI.SetActive(_visible);
    }
    public void SetCoolingDown(float _duration)
    {
        coolingDown = true;
        coolDownDuration = _duration;
        UI.SetActive(true);
    }
    void Update()
    {
        if(!coolingDown){return;}
        float fill = heatSlider.value;
        fill -= Time.deltaTime * 1 / coolDownDuration;
        heatSlider.value = fill;
        if(fill <= 0)
        {
            fill = 0;
            coolingDown = false;
            UI.SetActive(false);
        }
    }
}
