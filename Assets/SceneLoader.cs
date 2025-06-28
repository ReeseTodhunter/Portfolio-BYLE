using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreenUI;
    public Slider loadingSlider;
    public float minWaitTime = 2;
    private float minWaitTimer = 0;
    public GameObject menuUI;
    public void Start()
    {
        loadingScreenUI.SetActive(false);
    }
    public void OnLoadScene(int _sceneId)
    {
        loadingScreenUI.SetActive(true);
        menuUI.SetActive(false);
        minWaitTimer = 0;
        StartCoroutine(LoadSceneAsync(_sceneId));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    IEnumerator LoadSceneAsync(int _sceneId)
    {
        while(minWaitTimer <= minWaitTime)
        {
            minWaitTimer += Time.deltaTime;
            float percentage = minWaitTimer / minWaitTime;
            float progressVal = Mathf.Clamp01(percentage / 2);
            loadingSlider.value = progressVal;
            yield return null;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneId);
        while(!operation.isDone)
        {
            float progressVal = Mathf.Clamp01(0.5f + operation.progress / 2);
            loadingSlider.value = progressVal;
            yield return null;
        }
    }
}
