using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Reese

public class AchievementPopup : MonoBehaviour
{
    public static AchievementPopup instance;
    public GameObject UI;
    public float descendDuration = 1, WaitDuration = 2, RiseDuration = 2;
    private float timer = 0;
    public TextMeshProUGUI title,description;
    public Image Icon;

    private Vector2 resolution;
    private float offsetAmount;
    private Vector2 startPos;
    private Vector2 endPos;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }
    private enum ScriptState
    {
        Hidden,
        Lowering,
        visible,
        rising
    }
    private ScriptState currState;

    private void Start()
    {
        //Get the current screen resolution
        //resolution = new Vector2(PlayerPrefs.GetInt("chosenResolutionX"), PlayerPrefs.GetInt("chosenResolutionY"));
        resolution = new Vector2(Screen.width, Screen.height);

        //update the offset amount based on the screen resolution
        offsetAmount = (float)resolution.y / 5.0f;

        //Setup the startPos
        startPos.x = (float)resolution.x - offsetAmount;
        startPos.y = (float)resolution.y + (offsetAmount / 2);

        SetUIYPos(startPos.y);

        UI.SetActive(false);

        //Set the start position offscreen
        //startPos = UI.GetComponent<RectTransform>().position;
        
        currState = ScriptState.Hidden;
    }
    private void Update()
    {
        //Get the current screen resolution
        //resolution = new Vector2(PlayerPrefs.GetInt("chosenResolutionX"), PlayerPrefs.GetInt("chosenResolutionY"));
        //Debug.Log(resolution);

        resolution = new Vector2(Screen.width, Screen.height);
        //Debug.Log(resolution);

        //update the offset amount based on the screen resolution
        offsetAmount = (float)resolution.y / 5.0f;

        //Update starting position of Achievement
        startPos.x = (float)resolution.x - offsetAmount;
        startPos.y = (float)resolution.y + (offsetAmount / 2);

        //Update ending position of Achievement
        endPos.x = startPos.x;
        endPos.y = startPos.y - offsetAmount;

        //if (Input.GetKeyDown(KeyCode.Delete)) { StartPopUp(); return; }
        switch (currState)
        {
            case ScriptState.Hidden:
                break;
            case ScriptState.Lowering:
                gameObject.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
                gameObject.GetComponent<AudioSource>().Play();
                timer += Time.unscaledDeltaTime;
                SetUIYPos(Mathf.Lerp(startPos.y, endPos.y, timer / descendDuration));
                if(timer < descendDuration) { break; }
                SetUIYPos(endPos.y);
                currState = ScriptState.visible;
                timer = 0;
                break;
            case ScriptState.visible:
                timer += Time.unscaledDeltaTime;
                if (timer / WaitDuration >= 0.5f)
                {
                    title.gameObject.SetActive(false);
                    description.gameObject.SetActive(true);
                }
                else
                {
                    title.gameObject.SetActive(true);
                    description.gameObject.SetActive(false);
                }
                if (timer < WaitDuration) { break; }
                currState = ScriptState.rising;
                timer = 0;
                break;
            case ScriptState.rising:
                timer += Time.unscaledDeltaTime;
                SetUIYPos(Mathf.Lerp(endPos.y, startPos.y, timer / RiseDuration));
                if (timer < RiseDuration) { break; }
                SetUIYPos(startPos.y);
                currState = ScriptState.Hidden;
                timer = 0;
                UI.SetActive(false);
                break;
        }
    }
    //Set popup to start position
    public void StartPopUp()
    {
        timer = 0;
        currState = ScriptState.Lowering;
        UI.SetActive(true);
        SetUIYPos(startPos.y);
        title.gameObject.SetActive(true);
        description.gameObject.SetActive(false);
    }
    //Update the Y position of the popup
    private void SetUIYPos(float _yPos)
    {
        Vector3 pos = UI.GetComponent<RectTransform>().position;
        pos.y = _yPos;
        UI.GetComponent<RectTransform>().position = pos;
    }
    public void SetUIText(string _title = "", string _description = "", Sprite _icon = null)
    {
        title.text = _title;
        description.text = _description;
        Icon.sprite = _icon;
    }
}
