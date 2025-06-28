using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPopupController : MonoBehaviour
{
    public static ItemPopupController instance; // The instance of the popup manager
    private GameObject popupObject; // The popup object in game
    private List<ItemText> items = new List<ItemText>(); // List containing all items that need to show a popup
    private int activeItemIndex = -1; // Current index of the item in the list that has a popup

    public bool active = true;

    private void Start()
    {
        // Creating instance
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
        // Spawn in popup object ready for use
        if (popupObject == null)
        {
            popupObject = Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.ITEM_DESC_BOX), 1000.0f * Vector3.down, Quaternion.Euler(45.0f, 0.0f, 0.0f));
        }
    }

    private void Update()
    {
        if(active == true)
        {
            // If there is more than 1 item that needs a popup displaying, we must repeatedly check which item is closest to the player and display that box
            if (items.Count >= 2)
            {
                UpdatePopup();
            }
        }
        
    }

    private void UpdatePopup()
    {
        //Fixeroni
        if(popupObject == null)
        {
            popupObject = Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.ITEM_DESC_BOX), 1000.0f * Vector3.down, Quaternion.Euler(45.0f, 0.0f, 0.0f));
        }
        // There are no items, teleport the popup underground and set item index to -1 (no item)
        if (items.Count <= 0)
        {
            popupObject.transform.position = 1000.0f * Vector3.down;
            activeItemIndex = -1;
            return;
        }

        // There is 1 item, set item index to 0
        else if (items.Count == 1)
        {
            activeItemIndex = 0;
        }

        // There is more than 1 item but only one item popup can be displayed, so the one which is the closest to the player will be shown
        else if (items.Count >= 2)
        {
            int closestItemIndex = -1;
            float closestDistance = 9999999.0f;

            // Scan through every item in list and find the closest one
            for (int i = 0; i < items.Count; ++i)
            {
                if(PlayerController.instance == null)
                {
                    return;
                }
                float distance = Vector3.Distance(PlayerController.instance.transform.position, items[i].position); // Distance from item to player
                if (distance < closestDistance) // Closer item found, marking it as the closest so far
                {
                    closestItemIndex = i;
                    closestDistance = distance;
                }
            }

            activeItemIndex = closestItemIndex;
        }
        // Update popup box position and text based off current item index
        popupObject.GetComponent<ItemDescriptionBox>().SetText(items[activeItemIndex].title, items[activeItemIndex].desc);
        popupObject.transform.position = items[activeItemIndex].position + (7.5f * Vector3.up);
    }

    public void AddBox(string a_title, string a_desc, Vector3 a_pos)
    {
        // Check if the item is already in the list ignore it
        // The powerups move up and down, so comparing the 2 itemText items wont work as the positions will be different
        // Instead descriptions are compared to find the item
        bool itemFound = false;
        foreach (ItemText item in items)
        {
            if (item.desc == a_desc)
            {
                itemFound = true;
                break;
            }
        }

        // Not a duplicate, add item to list
        if (!itemFound)
        {
            items.Add(new ItemText(a_title, a_desc, a_pos));
            UpdatePopup();
        }
    }

    public void RemoveBox(string a_title, string a_desc, Vector3 a_pos)
    {
        // Find item from list to remove it
        // The powerups move up and down, so comparing the 2 itemText items wont work as the positions will be different
        // Instead descriptions are compared to find the item
        bool itemRemoved = false;
        foreach (ItemText item in items)
        {
            if (item.desc == a_desc)
            {
                itemRemoved = true;
                items.Remove(item);
                break;
            }
        }
        // Failed to find item to remove, nuke the whole list as a precaution
        if (!itemRemoved)
        {
            Debug.Log("Item popup list nuked, by a popup with the title: "+a_title);
            items = new List<ItemText>();
        }

        // Update the popup to see if it needs to move again
        UpdatePopup();
    }

    public void ClearBox()
    {
        items = new List<ItemText>();
        /*foreach (ItemText item in items)
        {
            items.Remove(item);
        }*/

        UpdatePopup();
    }

    struct ItemText
    {
        public ItemText(string a_title, string a_desc, Vector3 a_pos)
        {
            this.title = a_title;
            this.desc = a_desc;
            this.position = a_pos;
        }
        public string title;
        public string desc;
        public Vector3 position;
    }
}
