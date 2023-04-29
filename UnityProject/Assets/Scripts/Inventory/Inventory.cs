using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject panel;
    public bool showingPanel;
    public Transform slotParent;
    public Transform iconsParent;

    public GameObject slotPref;
    public GameObject itemIconPref;
    public GameObject itemInGroundPref;

    public List<Slot> Slots = new List<Slot>();
    public List<Item> ItemsInInventory = new List<Item>();
    public List<ItemIcon> ItemIcons = new List<ItemIcon>();

    
 

    public Vector2 inventorySize = new Vector2(5,3);
    public Vector2 slotOffset = new Vector2(.1f,.5f);


    InventoryDatabase database;

    public static Inventory Instance = null;

    public ItemIcon iconInHand;

    bool mouseOnCooldown;

    float dropItemPositionThreshold = 100;

    void Awake() //Create Singleton
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);


    }

    void Start()
    {
        panel.SetActive(false);
        database = GetComponent<InventoryDatabase>();
        CreateSlots();
    }

    void CreateSlots()
    {
        for(int i= 0;i<inventorySize.y;i++)
        {
            for(int j=0;j<inventorySize.x;j++)
            {
                GameObject newSlot = (GameObject)Instantiate(slotPref);
                newSlot.transform.SetParent(slotParent);
                newSlot.transform.position = new Vector3(slotParent.transform.position.x + slotOffset.x*j, slotParent.transform.position.y - slotOffset.y * i, slotParent.transform.position.z);
                Slot slot = newSlot.GetComponent<Slot>();
                slot.positionX = j;
                slot.positionY = i;
                
                Slots.Add(slot);
            }
        }
    }


    public void AddItemFromDatabase(int ID)
    {

        Item FromDatabase = SearchItemByID(database.ItemDatabase, ID);
        Item NewItem = CloneItem(FromDatabase);

        AddItem(NewItem);
     
    
    }

    public void AddItem(Item NewItem)
    {
        Slot DesignatedSlot = CheckForSpace();

        if (DesignatedSlot != null)
        {
            DesignatedSlot.Used = true;
            NewItem.positionX = DesignatedSlot.positionX;
            NewItem.positionY = DesignatedSlot.positionY;
            ItemsInInventory.Add(NewItem); //CHECK IF SPACE FIRST

            CreateItemIcon(NewItem, DesignatedSlot);
        }
        else
        {
            print("Inventory full");
        }
    }

    public Item CloneItem(Item I)
    {
        Item item = new Item(I.id, I.name, I.damage, I.defense, I.sprite, I.cost);

        return item;
    }

    public Item SearchItemByID(List<Item> list, int ID)
    {
        Item item = null;
        foreach(Item I in list)
        {
            if(I.id == ID)
            {
                item = I;
                break;
            }
        }

        return item;
    }

    public Slot CheckForSpace()
    {
        Slot slot = null;
        foreach(Slot S in Slots)
        {
            if(!S.Used)
            {
                slot = S;
                break;
            }
        }

        return slot;
    }
   
    public void CreateItemIcon(Item item, Slot slot)
    {
        GameObject newIcon = (GameObject)Instantiate(itemIconPref);
        newIcon.transform.SetParent(iconsParent);

        ItemIcon icon = newIcon.GetComponent<ItemIcon>();
        icon.SetItem(item);
        
        ItemIcons.Add(icon);
        SetItemIcon(icon, slot);
    }

    public void SetItemIcon(ItemIcon icon, Slot slot)
    {
        ClearSlots();
        icon.transform.position = slot.transform.position;
        icon.CurrentSlot = slot;
        slot.Used = true;
        icon.CurrentItem.positionX = slot.positionX;
        icon.CurrentItem.positionY = slot.positionY;
    }



    public void SelectItem(ItemIcon icon)
    {
        if(!mouseOnCooldown)
        {
            SetItemInHand(icon);
            SetMouseCooldown();
        }
       
    }

    void SetItemInHand(ItemIcon icon)
    {
        if(icon.CurrentSlot != null)
        {
            icon.CurrentSlot.Used = false;
        }        
        icon.Selected = true;
        iconInHand = icon;
      
    }

    void RemoveItemFromSlot(ItemIcon icon)
    {
       icon.CurrentSlot.Used = false;
       icon.CurrentItem.positionX = 0;
       icon.CurrentItem.positionY = 0;

        //icon.Currentslot.changecolor
        icon.CurrentSlot = null;

    }

    void MoveItemIcon(ItemIcon icon)
    {
        Slot slot = GetSlotCloserToMouse();

        if(!CheckDistanceToMouse())
        {

            if(slot.Used)
            {
                icon.Selected = false;

                ItemIcon OtherItem = GetItemIconByPosition(slot.positionX, slot.positionY);
                OtherItem.CurrentSlot = null;                
                SetItemIcon(icon, slot);
                SetItemInHand(OtherItem);
                

            }
            else
            {

                SetItemIcon(icon, slot);

                icon.Selected = false;
                iconInHand = null;
            }
     

            
        }
        else
        {
            DropItem(iconInHand);
            iconInHand = null;
        }


        
        ClearSlots();
    }

    bool CheckDistanceToMouse()
    {
        bool DropArea = false;

        Vector2 mousePosition = Input.mousePosition;
        Vector2 slotPos = Slots[0].transform.position;

        float pos = mousePosition.x + dropItemPositionThreshold;
        if (pos < slotPos.x)
        {
            DropArea = true;
        }
      

        return DropArea;
        //if(distance>)

    }

    void DropItem(ItemIcon Icon)
    {
        if(iconInHand != null)
        {
            ClearSlots();
            Item itemToDrop = iconInHand.CurrentItem;

            GameObject NewItemInGround = (GameObject)Instantiate(itemInGroundPref, GameManager.Instance.Player.transform.position, Quaternion.identity);
            ItemInGround itemInGround =  NewItemInGround.GetComponent<ItemInGround>();
            itemInGround.item = itemToDrop;
            itemInGround.SetImage();

            Rigidbody2D rb = itemInGround.GetComponent<Rigidbody2D>();

            rb.AddForce(Vector3.down * Random.Range(3,5),ForceMode2D.Impulse);

            Destroy(iconInHand.gameObject);
        }
    }

    public void InventoryButton()
    {
        if (!showingPanel)
        {
            Open();
        }
        else
        {
            Close();
        }
            
    }

    public void Open()
    {
        if(!showingPanel)
        {
            showingPanel = true;
            panel.SetActive(true);
        }
    }

    public void Close()
    {
        if (showingPanel)
        {
            showingPanel = false;
            panel.SetActive(false);
        }
    }

    private void Update() 
    {

        //REMOVE
        if(Input.GetKeyUp(KeyCode.Space))
        {
            AddItemFromDatabase(Random.Range(0,database.ItemDatabase.Count));
        }

        if(iconInHand!= null)
        {
            iconInHand.transform.position = Input.mousePosition;

            if(!mouseOnCooldown)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MoveItemIcon(iconInHand);
                    SetMouseCooldown();
                }
                else
                {
                    ClearSlots();
                    Slot highlightSlot = GetSlotCloserToMouse();
                    highlightSlot.mouseOver();
                }
            }
          
        }
        
    }


    public ItemIcon GetItemIconByPosition(int X,int Y)
    {
        ItemIcon item = null;
        foreach(ItemIcon I in ItemIcons)
        {
            if(I.CurrentItem.positionX == X && I.CurrentItem.positionY == Y)
            {
                item = I;
                break;
            }
        }
        return item;
    }

    public Slot GetSlotByPosition(int X, int Y)
    {
        Slot slot = null;

        int ArrayPos = X + Y * (int)inventorySize.x;

        if (ArrayPos < Slots.Count)
        {
            slot = Slots[ArrayPos];
        }

        return (slot);
    }

    public Slot GetSlotCloserToMouse()
    {
        Slot slot = null;
        
        Vector2 MousePosition = Input.mousePosition;

        float distance = 9999f;
        foreach(Slot S in Slots)
        {
            float distanceDelta = Vector2.Distance(S.transform.position, MousePosition);
            if(distanceDelta < distance)
            {
                slot = S;
                distance = distanceDelta;
            }
        }

        return slot;
    }

    void ClearSlots()
    {
        foreach(Slot S in Slots)
        {
            if(!S.Used)
            {
                S.ClearColors();
            }
            else
            {
                S.SetUsed();
            }
        }
    }

    void SetMouseCooldown() //Prevents multiple actions from same click
    {
        CancelInvoke("MouseCooldown");
        mouseOnCooldown = true;
        Invoke("MouseCooldown",.1f);
    }

    void MouseCooldown()
    {
        mouseOnCooldown = false;
    }
}
