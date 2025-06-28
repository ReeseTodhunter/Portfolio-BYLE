using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TitleScript : MonoBehaviour
{
    [SerializeField]float openingDuration = 2, holdDuration = 2, closingDuration = 2;
    float timer = 0;
    [SerializeField] List<Image> images = new List<Image>();
    //[SerializeField] List<Image> images = new List<Image>();
    [SerializeField] List<TextMeshProUGUI> text = new List<TMPro.TextMeshProUGUI>();
    [SerializeField] List<Text> legacyText = new List<Text>();
    public GameObject UI;

    public TMPro.TextMeshProUGUI clearText;
    public TMPro.TextMeshProUGUI clearText2;

    public bool roomClear = false;
    private enum State
    {
        opening,
        active,
        closing,
        closed
    }
    private State currState;
    public void Start()
    {
        
        if (roomClear == false)
        {
            UI.SetActive(true);
            timer = 0;
            currState = State.opening;
        }
        else
        {
            currState = State.closed;
        }
    }
    public void Update()
    {
        timer += Time.deltaTime;
        switch(currState)
        {
            case State.opening:
                if(timer >= openingDuration)
                {
                    currState = State.active; 
                    timer = 0;
                    return;
                }
                OpeningUpdate();
                break;
            case State.active:
                if(timer >= holdDuration)
                {
                    currState = State.closing; 
                    timer = 0;
                    return;
                }
                break;
            case State.closing:
                if(timer >= closingDuration)
                {
                    currState = State.closed;
                    timer = 0;
                    return;
                }
                ClosingUpdate();
                break;
            case State.closed:
                DeactivateUI();
                break;
        }
    }
    void OpeningUpdate()
    {
        foreach(Image currImage in images)
        {
            Color col = currImage.color;
            col.a = Mathf.Lerp(0,1, timer / openingDuration);
            currImage.color = col;
        }
        foreach(TextMeshProUGUI currText in text)
        {   
            Color col = currText.color;
            col.a = Mathf.Lerp(0,1, timer / openingDuration);
            currText.color = col;
        }
        foreach (Text currText in legacyText)
        {
            Color col = currText.color;
            col.a = Mathf.Lerp(0, 1, timer / openingDuration);
            currText.color = col;
        }
    }
    void ClosingUpdate()
    {
        foreach(Image currImage in images)
        {
            Color col = currImage.color;
            col.a = Mathf.Lerp(1,0, timer / closingDuration);
            currImage.color = col;
        }
        foreach(TextMeshProUGUI currText in text)
        {
            Color col = currText.color;
            col.a = Mathf.Lerp(1,0, timer / closingDuration);
            currText.color = col;
        }
        foreach (Text currText in legacyText)
        {
            Color col = currText.color;
            col.a = Mathf.Lerp(0, 1, timer / openingDuration);
            currText.color = col;
        }
    }

    protected virtual void DeactivateUI()
    {
        UI.SetActive(false);
    }
}
