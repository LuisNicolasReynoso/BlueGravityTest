using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInGround : MonoBehaviour
{
    // Start is called before the first frame update

    public Item item;
    public SpriteRenderer spriteRenderer;

    Animator anim;

    public bool ForSale;

    bool pickCooldown = true;
    bool buyCooldown;

    bool pickedUp;
    void Start()
    {
        StartCoroutine(PickCD());

        if(ForSale)
        {
            SetImage();
        }
    }

    // Update is called once per frame
    void OnClick()
    {
        if(!ForSale)
        {
            PickItem();
            
        }
        else
        {

            BuyItem();
          
        }
       
    }

    public void SetImage()
    {
        anim = GetComponent<Animator>();     
        spriteRenderer.sprite = Resources.Load<Sprite>("Icons/" + item.sprite);


        if (!ForSale)
        {
            anim.SetTrigger("Drop");
        }

    }

    void PickItem()
    {
        if(!pickCooldown)
        {
            if(Inventory.Instance.AddItem(item))
            {
                pickedUp = true;
                AudioManager.Instance.PlayRandomSound(1);                
                Remove();
            }
           
        }
        
    }

    void BuyItem()
    {
        if (!buyCooldown)
        {
            buyCooldown = true;
            if (Inventory.Instance.CheckGold(-item.cost))
            {
                Inventory.Instance.ChangeGold(-item.cost);
                Inventory.Instance.SpawnItem(item, this.transform.position, this.transform.right);
                AudioManager.Instance.PlayRandomSound(0);
            }
            else
            {
                AudioManager.Instance.PlayRandomSound(4);
                GameManager.Instance.messageManager.SpawnMessage("Not enough gold", GameManager.Instance.Player.transform, true);
            }

            StartCoroutine(BuyCD());
        }
           
    }

    private void OnMouseDown()
    {
        OnClick();
    }

    

    private void OnMouseOver()
    {
        if(!pickedUp)
        {
            Vector2 PosFix = Camera.main.WorldToScreenPoint(this.transform.position);
            Inventory.Instance.tooltip.SetTooltip(item, PosFix, false);
        }
        
    }

    private void OnMouseExit()
    {
        Inventory.Instance.tooltip.Hide();
    }

    void Remove()
    {
        Inventory.Instance.tooltip.Hide();
        Destroy(this.gameObject);
    }


    IEnumerator BuyCD()
    {
        yield return new WaitForSeconds(.25f);
        buyCooldown = false;
    }

    IEnumerator PickCD()
    {
        yield return new WaitForSeconds(.5f);
        pickCooldown = false;
    }


   



    
}
