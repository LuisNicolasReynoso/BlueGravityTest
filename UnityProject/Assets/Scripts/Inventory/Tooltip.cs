using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class Tooltip : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public Image ItemIcon;
    public TextMeshProUGUI ItemCost;

    public GameObject Panel;

    public bool Showing;
    public bool FromUI;

    [SerializeField]
    float offsetX = 120;
    [SerializeField]
    float offsetY = 50;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTooltip(Item item, Vector3 pos, bool fromUI)
    {
        LoadValues(item);
        FromUI = fromUI;
        Show();

        this.transform.position = new Vector3(pos.x - offsetX, pos.y + offsetY, pos.z);
    }

    void LoadValues(Item item)
    {
        ItemName.text = item.name;
        ItemDescription.text = item.description;

        ItemIcon.sprite = Resources.Load<Sprite>("Icons/" + item.sprite);

        ItemCost.text = item.cost.ToString();
    }


    void Show()
    {
        if(!Showing)
        {
            Showing = true;
            Panel.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if(Showing)
        {
            Showing = false;
            Panel.gameObject.SetActive(false);
        }
    }
}
