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
    void Start()
    {
        StartCoroutine(PickCD());
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
        anim.SetTrigger("Drop");
        spriteRenderer.sprite = Resources.Load<Sprite>("Icons/" + item.name);

       
    }

    void PickItem()
    {
        if(!pickCooldown)
        {
            Inventory.Instance.AddItem(item);
            Remove();
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
                Inventory.Instance.SpawnItem(item, this.transform.position, Vector3.right);
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
        Vector2 PosFix = Camera.main.WorldToScreenPoint(this.transform.position);
        Inventory.Instance.tooltip.SetTooltip(item, PosFix, false);
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
