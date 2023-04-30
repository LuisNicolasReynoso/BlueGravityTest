using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemIcon : MonoBehaviour
{
    public Image image;

    public Item CurrentItem;

    public bool Selected;

    public Slot CurrentSlot;

    public RectTransform rectTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void SetItem(Item item)
    {
        CurrentItem = item;

        image.sprite = Resources.Load<Sprite>("Icons/" + item.sprite);
    }

    public void Click()
    {
        if(!Selected)
        {
            Inventory.Instance.SelectItem(this);
        }
    }
}
