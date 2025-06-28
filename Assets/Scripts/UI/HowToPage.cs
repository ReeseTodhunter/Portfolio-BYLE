using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class HowToPage : MonoBehaviour
{
    public List<Sprite> pages = new List<Sprite>();
    public List<string> descriptions = new List<string>();
    public Image page;
    public TextMeshProUGUI text;
    private int currPage = 0;
    public GameObject prevButton, nextButton;
    void Awake()
    {
        UpdatePage();
    }
    private void UpdatePage()
    {
        currPage = Mathf.Clamp(currPage, 0, pages.Count);
        if(pages.Count > currPage)
        {
            page.sprite = pages[currPage];
        }
        else
        {
            page.sprite = null;
        }
        if(descriptions.Count >= currPage)
        {
            text.text = descriptions[currPage];
        }
        else
        {
            text.text = "";
        }

        //Update buttons
        prevButton.SetActive(currPage <= 0 ? false : true);
        nextButton.SetActive(currPage >= pages.Count - 1 ? false : true);
    }
    public void NextPage()
    {
        currPage++;
        UpdatePage();
    }
    public void PrevPage()
    {
        currPage--;
        UpdatePage();
    }

    
}
