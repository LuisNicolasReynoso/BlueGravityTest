using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class Tooltip : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image itemIcon;
    public TextMeshProUGUI itemCost;

    public TextMeshProUGUI itemDamage;
    public TextMeshProUGUI itemDefense;

    public GameObject panel;

    [HideInInspector]
    public bool Showing;
    [HideInInspector]
    public bool FromUI;

    [SerializeField]
    float offsetX = 120;
    [SerializeField]
    float offsetY = 50;
    

    public void SetTooltip(Item item, Vector3 pos, bool fromUI)
    {
        LoadValues(item);
        FromUI = fromUI;
        Show();

        float resolutionMultiplerX = Screen.width / 1280f;
        float resolutionMultiplerY = Screen.height / 720f;

        this.transform.position = new Vector3(pos.x - (offsetX * resolutionMultiplerX), pos.y + (offsetY * resolutionMultiplerY), pos.z);
    }

    void LoadValues(Item item)
    {
        itemName.text = item.name;
        itemDescription.text = item.description;

        itemIcon.sprite = Resources.Load<Sprite>("Icons/" + item.sprite);

        itemCost.text = "Cost: " + item.cost.ToString();

        itemDamage.text = "Damage: " + item.damage.ToString();

        itemDefense.text = "Defense: " + item.defense.ToString();
    }


    void Show()
    {
        if(!Showing)
        {
            Showing = true;
            panel.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if(Showing)
        {
            Showing = false;
            panel.gameObject.SetActive(false);
        }
    }
}
