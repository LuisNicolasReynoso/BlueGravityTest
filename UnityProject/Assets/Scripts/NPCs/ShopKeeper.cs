using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform messagePos;

    float distanceThreshold = 4;

    bool nearShop;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceDelta = Vector2.Distance(this.transform.position, GameManager.Instance.Player.transform.position);
        

        if(distanceThreshold > distanceDelta)
        {
            nearShop = true;
            Inventory.Instance.nearShopKeeper = true;
        }
        else
        {
            nearShop = false;
            
        }

        Inventory.Instance.nearShopKeeper = nearShop;
     
      
    }

    public void OnMouseDown()
    {
        if(nearShop)
        {
            GameManager.Instance.messageManager.SpawnMessage("Hello traveler! Right click items while near me to sell them", messagePos, false);

            Inventory.Instance.Open();
        }
        
    }
}
