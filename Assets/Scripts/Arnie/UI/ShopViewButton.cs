using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopViewButton : MonoBehaviour, IPointerEnterHandler
{
    public ShopItem viewObject;


    public ShopInteractScript parentShop;

    public bool initialActivation = false;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        viewObject.shopKeeperText.text = viewObject.priceText();
        
    }

    public void OnPressTheButton()
    {
        viewObject.CurrentClickedGameObject(viewObject.gameObject);
    }
}
