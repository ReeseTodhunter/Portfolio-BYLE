using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChargeSlider : MonoBehaviour
{
    public static ChargeSlider instance;
    public Slider slider;
    public Image sliderImage;
    public Image endBar;
    private void Awake()
    {
        if(instance != null)
        {
            if(instance !=this)
            {
                Destroy(this);
            }
        }
        instance = this;
        SetSliderVisible(false);
    }
    public void SetSliderValue(float _val)
    {
        if(_val > 1){_val = 1;}
        else if(_val < 0){_val = 0;}
        slider.value = _val;
    }
    public void SetSliderColor(Color _col)
    {
        sliderImage.color = _col;
    }
    public void SetSliderVisible(bool _isVisible)
    {
        sliderImage.enabled = _isVisible;
        endBar.enabled = _isVisible;
    }
}
