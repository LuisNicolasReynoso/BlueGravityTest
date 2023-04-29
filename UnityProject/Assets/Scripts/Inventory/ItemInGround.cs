using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInGround : MonoBehaviour
{
    // Start is called before the first frame update

    public Item item;
    public SpriteRenderer spriteRenderer;

    Animator anim;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnClick()
    {
        Inventory.Instance.AddItem(item);
        Remove();
    }

    public void SetImage()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Drop");
        spriteRenderer.sprite = Resources.Load<Sprite>("Icons/" + item.name);
    }

    private void OnMouseDown()
    {
        OnClick();
    }

    void Remove()
    {
        Destroy(this.gameObject);
    }
}
