using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineUIController : MonoBehaviour
{
    public static MagazineUIController instance;
    private float magazineSize;
    private float currentMagSize;
    public GameObject magPrefab;
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    private List<GameObject> UIElements = new List<GameObject>();
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
    void Start()
    {
    }
    public void RefreshMagazineUI(float maxMag, float currMag)
    {
        for(int i = 0; i < UIElements.Count; i++)
        {
            Destroy(UIElements[i]);
        }
        if(currMag == 0 || maxMag == 0){return;}
        magazineSize = maxMag;
        currentMagSize = currMag;
        if(currentMagSize >= 45){currentMagSize = 45;}
        float height = EndPosition.y - StartPosition.y;
        float spriteHeight = 25;
        for(int i = 0; i < currentMagSize; i++)
        {
            Vector3 position = this.GetComponent<RectTransform>().position + Vector3.Lerp(StartPosition,EndPosition, i / magazineSize);
            GameObject prefab = Instantiate(magPrefab, Vector3.zero, Quaternion.identity);
            prefab.GetComponent<RectTransform>().anchoredPosition = position;
            prefab.transform.SetParent(transform);

            Vector2 sizeDelta = prefab.GetComponent<RectTransform>().sizeDelta;
            sizeDelta.y = spriteHeight;
            prefab.GetComponent<RectTransform>().sizeDelta = sizeDelta;

            UIElements.Add(prefab);
        }
    }
}
