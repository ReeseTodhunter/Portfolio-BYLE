using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    /*
     * This script controls the weather in the game. It has times of day and different weathers.
     * These can be set randomly at run-time or can be manually set using methods.
     * - Tom
    */
    [Header("Weather")] //All the different particle effect weathers
    public ParticleSystem Rain;
    public ParticleSystem acidRain;
    public ParticleSystem leaves;
    public ParticleSystem wind;
    public ParticleSystem cherryBlossom;
    public ParticleSystem Fog;
    public ParticleSystem embers;
    public ParticleSystem byleDust;
    public ParticleSystem Snow;

    public GameObject weatherRoot; //Root location of weathers
    public Vector3 offset = new Vector3(0,10,0); //offset of root
    

    //Light settings
    //----------------------------------------//
    [Space(6)]
    public Light daylight;
    public lightningFlash lightningFlash;
    [Space(6), Header("Light source angles")]
    public Vector3 dawn;
    public Color dawnCol;
    public float dawnIntensity = 1;

    public Vector3 day;
    public Color dayCol;
    public float dayIntensity = 3;

    public Vector3 dusk;
    public Color duskCol;
    public float duskIntensity = 1;

    public Vector3 nightTime;
    public Color nightCol;
    public float nightIntensity = .5f;

    public Vector3 thunderStorm;
    public Color thunderStormColor;
    public float thunderStormIntensity = .3f;

    public Vector3 byleStorm;
    public Color byleStormColor;
    public float byleStormIntensity = .3f;
    //----------------------------------------//

    private float time = 0;
    public bool fixedWeather = false; //allows manual setting of weather in inspector
    public WeatherType fixedWeatherType = WeatherType.Clear; //default weather
    public static WeatherSystem instance; //Singleton instance
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
    public bool randomDayTime = true; //random day time
    public bool randomWeather = true; //random weather time
    public enum WeatherType //Weather types
    {
        Clear,
        Rain,
        AcidRain,
        Wind,
        Leaves,
        CherryBlossom,
        Fog,
        embers,
        byleDust,
        Snow,
        Lightning,
        ByleLightning
    }
    public enum DayTime //Day time types
    {
        Dawn,
        Day,
        Dusk,
        Nighttime,
        thunderStorm,
        byleStorm
    }
    private void Start()
    {
        lightningFlash.SetLightningEnabled(false);
        if(fixedWeather)
        {
            SetWeather(fixedWeatherType);
            return;
        }
        if (randomWeather)
        {
            RandomiseWeather();
        }
        if (randomDayTime)
        {
            RandomiseTime();
        }
    }
    public void RandomiseWeather() //Randomise weather
    {
        float rnd = Random.Range(0, 100);
        if (rnd < 15) //15% chance for clear skies
        {
            SetWeather(WeatherType.Clear);
            Debug.Log("clear weather");
            return;
        }
        WeatherType randWeather = (WeatherType)Random.Range(1, 12);
        SetWeather(randWeather);
    }
    public void RandomiseTime()
    {
        DayTime time = (DayTime)Random.Range(0, 4);
        SetTimeOfDay(time);
    }
    public void SetWeather(WeatherType _targetWeather) //Set weather by enabling the correct particle system
    {
        ClearWeather();
        switch(_targetWeather)
        {
            case WeatherType.Rain:
                Rain.Play();
                break;
            case WeatherType.AcidRain:
                acidRain.Play();
                break;
            case WeatherType.Leaves:
                leaves.Play();
                break;
            case WeatherType.CherryBlossom:
                cherryBlossom.Play();
                break;
            case WeatherType.Wind:
                wind.Play();
                break;
            case WeatherType.Fog:
                Fog.Play();
                break;
            case WeatherType.embers:
                embers.Play();
                break;
            case WeatherType.byleDust:
                byleDust.Play();
                break;
            case WeatherType.Snow:
                Snow.Play();
                break;
            case WeatherType.Lightning:
                Rain.Play();
                lightningFlash.SetLightningEnabled(true);
                SetTimeOfDay(DayTime.Nighttime);
                break;
            case WeatherType.ByleLightning:
                acidRain.Play();
                lightningFlash.SetLightningEnabled(true,true);
                SetTimeOfDay(DayTime.byleStorm);
                break;
            default:
                break;
        }
    }
    public void SetTimeOfDay(DayTime time) //Set time of day by adjusting the color, angle and intensity of directional light
    {
        float intensity = 1;
        Vector3 angle = Vector3.zero;
        Color col = Color.white;
        switch (time)
        {
            case DayTime.Dawn:
                intensity = dawnIntensity;
                angle = dawn;
                col = dawnCol;
                break;
            case DayTime.Day:
                intensity = dayIntensity;
                angle = day;
                col = dayCol;
                break;
            case DayTime.Dusk:
                intensity = duskIntensity;
                angle = dusk;
                col = duskCol;
                break;
            case DayTime.Nighttime:
                intensity = nightIntensity;
                angle = nightTime;
                col = nightCol;
                break;
            case DayTime.thunderStorm:
                intensity = thunderStormIntensity;
                angle = thunderStorm;
                col = thunderStormColor;
                break;
            case DayTime.byleStorm:
                intensity = byleStormIntensity;
                angle = byleStorm;
                col = byleStormColor;
                break;
            default:
                intensity = dayIntensity;
                angle = day;
                col = dayCol;
                break;
        }
        daylight.transform.eulerAngles = angle;
        daylight.color = col;
        daylight.intensity = intensity;
    }
    void Update() //If O button pressed, change weather
    {
        if(PlayerController.instance == null){return;}
        weatherRoot.transform.position = PlayerController.instance.transform.position + offset;
    }
    public void ClearWeather() //clear all weather, by disabling all particle systems
    {
        Rain.Stop();
        Rain.Clear(); 
        acidRain.Stop();
        acidRain.Clear();
        leaves.Stop();
        leaves.Clear();
        cherryBlossom.Stop();
        cherryBlossom.Clear();
        wind.Stop();
        wind.Clear();
        Fog.Stop();
        Fog.Clear();
        embers.Stop();
        embers.Clear();
        byleDust.Stop();
        byleDust.Clear();
        lightningFlash.SetLightningEnabled(false);
	    Snow.Clear();
	    Snow.Stop();
    }
}
