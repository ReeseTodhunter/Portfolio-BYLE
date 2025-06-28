using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReloadAnimation : MonoBehaviour
{
    public bool isEnabled = true;
    public bool isRunning = false;
    public GameObject UI;
    public Color defaultBarColor, defaultZoneColor, failedColor;
    public RectTransform innerLeftBar, innerRightBar, reloadZone, reloadBar;
    private float duration, perfectTime, forgiveness;
    public Vector3 startPos, endPos;
    private Vector3 currPos;
    private float timer = 0;
    public static ReloadAnimation instance;
    public Color perfectCol, normalCol;
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
        UI.SetActive(false);
        reloadBar.GetComponent<Image>().color = defaultBarColor;
        reloadZone.GetComponent<Image>().color = defaultZoneColor;
    }
    public void BeginAnimation(float _duration, float _perfectTime, float _forgiveness, bool _hasForgivenessTime = false)
    {
        UI.SetActive(true);
        reloadZone.gameObject.SetActive(_hasForgivenessTime);
        innerLeftBar.gameObject.SetActive(_hasForgivenessTime);
        innerRightBar.gameObject.SetActive(_hasForgivenessTime);
        reloadBar.gameObject.GetComponent<Image>().color = _hasForgivenessTime ? perfectCol : normalCol;
        duration = _duration;
        perfectTime = _perfectTime;
        forgiveness = _forgiveness;
        float radius = forgiveness /2 ;
        innerLeftBar.anchoredPosition = Vector3.Lerp(startPos, endPos, (perfectTime - radius) / duration);
        innerRightBar.anchoredPosition = Vector3.Lerp(startPos,endPos, (perfectTime + radius) / duration);
        reloadZone.anchoredPosition = Vector3.Lerp(startPos,endPos, perfectTime / duration);
        isRunning = true;
        isEnabled = true;
        timer = 0;
        Vector3 scale = reloadZone.localScale;
        scale.x = forgiveness / duration;
        reloadZone.localScale = scale;
        reloadZone.GetComponent<Image>().color = defaultZoneColor;
    }
    public void StopAnimation()
    {
        UI.SetActive(false);
        isRunning = false;
        isEnabled = false;
    }
    void Update()
    {
        if(!isRunning)
        {
            return;
        }
        if(timer <= duration)
        {
            timer += Time.unscaledDeltaTime;
            currPos = Vector3.Lerp(startPos,endPos, timer / duration);
            reloadBar.anchoredPosition = currPos;
            reloadBar.gameObject.SetActive(true);
        }
        else
        {
            reloadBar.gameObject.SetActive(false);
            StopAnimation();
        }
    }
    public void FailFastReload()
    {
        reloadBar.GetComponent<Image>().color = failedColor;
        reloadZone.GetComponent<Image>().color = failedColor;
    }
}
