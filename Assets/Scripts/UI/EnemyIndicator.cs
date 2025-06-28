using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    public static EnemyIndicator instance;
    private List<GameObject> enemies = new List<GameObject>();
    public int maxIndicators = 3;
    public GameObject indicatorPrefab;
    private List<GameObject> indicators = new List<GameObject>();
    public Canvas parentCanvas;
    public Camera mainCam;
    private List<Vector3> screenPosisions = new List<Vector3>();
    public GameObject testObject;
    public float offset = 0;
    public bool test = false;
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
    private void Update()
    {
        if (mainCam == null) { return; }
        List<BTAgent> agents = EnemySpawningSystem.instance.GetEnemies();
        ClearIndicators();
        enemies.Clear();
        screenPosisions.Clear();
        if (test)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(testObject.transform.position);
            if (screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Screen.width || screenPos.y > Screen.height)
            {
                screenPosisions.Add(GetEdgePosition(screenPos));
                GameObject newIndicator = Instantiate(indicatorPrefab, transform);
                newIndicator.GetComponent<RectTransform>().position = GetEdgePosition(screenPosisions[0]);
                newIndicator.GetComponent<RectTransform>().LookAt(this.GetComponent<RectTransform>().position);
                indicators.Add(newIndicator);
            }
            return;
        }
        if (agents.Count == 0 || agents == null)
        {
            return;
        }
        foreach (BTAgent agent in agents)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(agent.transform.position);
            if (screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Screen.width|| screenPos.y > Screen.height) { screenPosisions.Add(GetEdgePosition(screenPos)); }
        }
        for (int i = 0; i < maxIndicators; i++)
        {
            if(i >= screenPosisions.Count) { break; }
            GameObject newIndicator = Instantiate(indicatorPrefab, transform);
            newIndicator.GetComponent<RectTransform>().position = GetEdgePosition(screenPosisions[i]);
            newIndicator.GetComponent<RectTransform>().LookAt(this.GetComponent<RectTransform>().position);
            indicators.Add(newIndicator);
        }
    }
    private void ClearIndicators()
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            Destroy(indicators[i]);
        }
        indicators.Clear();
    }
    private Vector3 GetEdgePosition(Vector3 screenTargetPos)
    {
        Vector3 centre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        bool inScreen = false;
        float x = 0;
        while (!inScreen)
        {
            x += 0.001f;
            Vector3 newPos = Vector3.Lerp(screenTargetPos, centre, x);
            if (newPos.x < Screen.width && newPos.x > 0 && newPos.y < Screen.height && newPos.y > 0)
            {
                return newPos - (screenTargetPos - centre) * offset;
            }
            else if (x >= 1)
            {
                inScreen = true;
            }
        }
        return centre;
    }
}

