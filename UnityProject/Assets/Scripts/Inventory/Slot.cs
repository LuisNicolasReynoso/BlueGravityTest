using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int positionX;
    public int positionY;
    public bool Used;
    public bool EquipmentSlot;

    public Item.Type type;

    public Image image;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color mouseOverColor;
    [SerializeField]
    private Color UsedColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void mouseOver()
    {
        image.color = mouseOverColor;
    }

    public void SetUsed()
    {
        image.color = UsedColor;
    }

    public void ClearColors()
    {
        image.color = normalColor;
    }
   
}
