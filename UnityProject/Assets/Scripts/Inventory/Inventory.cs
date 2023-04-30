using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory : MonoBehaviour
{

    public GameObject panel;
    public bool showingPanel;
    public Transform slotParent;
    public Transform iconsParent;

    public GameObject slotPref;
    public GameObject itemIconPref;
    public GameObject itemInGroundPref;

    public List<Slot> Slots = new List<Slot>();
    public List<ItemIcon> ItemIcons = new List<ItemIcon>();

    public Slot SpellSlot;
    [HideInInspector]
    public ItemIcon EquipedSpell;

    public Slot RingSlot;
    [HideInInspector]
    public ItemIcon EquipedRing;

    public Slot ConsumableSlot;
    [HideInInspector]
    public ItemIcon EquipedConsumable;


    public Vector2 inventorySize = new Vector2(5, 3);
    public Vector2 slotOffset = new Vector2(.1f, .5f);


    InventoryDatabase database;

    public static Inventory Instance = null;

    public ItemIcon iconInHand;

    bool mouseOnCooldown;    

    public Tooltip tooltip;

    [SerializeField]
    private int gold;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI defenseText;

    public bool nearShopKeeper;

    public GameObject SellPanel;

    float resolutionMultiplerX;
    float resolutionMultiplerY; 

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

        //Create the inventory slots
        CreateSlots();

        //Starting Gold
        ChangeGold(3000);

        //Add the equipment slots to the slot list
        Slots.Add(SpellSlot);
        Slots.Add(RingSlot);
        Slots.Add(ConsumableSlot);
    }

    //Create the multiple slots that represents player inventory
    void CreateSlots()
    {
        //for scaling to different resolutions
        resolutionMultiplerX = Screen.width / 1280f;
        resolutionMultiplerY = Screen.height / 720f;

        //slot offset is also scaled
        Vector2 slotsOffsetScaled =  new Vector2(slotOffset.x * resolutionMultiplerX, slotOffset.y * resolutionMultiplerY);

        for (int i = 0; i < inventorySize.y; i++)
        {
            for (int j = 0; j < inventorySize.x; j++)
            {
                GameObject newSlot = (GameObject)Instantiate(slotPref);
                newSlot.transform.SetParent(slotParent);
                //Position the slot in the array position
                newSlot.transform.position = new Vector3(slotParent.transform.position.x + slotsOffsetScaled.x * j, slotParent.transform.position.y - slotsOffsetScaled.y * i, slotParent.transform.position.z);
                RectTransform rectTransform = newSlot.GetComponent<RectTransform>();
                ScaleObjectToResolution(rectTransform);

                Slot slot = newSlot.GetComponent<Slot>();

                //Set the slot position to the array position
                slot.positionX = j;
                slot.positionY = i;

                Slots.Add(slot);
            }
        }
    }  

    //Add an item, creating an item icon on inventory
    public bool AddItem(Item item)
    {
        bool space = false;

        Item NewItem = CloneItem(item);
        Slot DesignatedSlot = CheckForSpace();

        if (DesignatedSlot != null)
        {
            DesignatedSlot.Used = true;
            NewItem.positionX = DesignatedSlot.positionX;
            NewItem.positionY = DesignatedSlot.positionY;


            CreateItemIcon(NewItem, DesignatedSlot);
            space = true;
        }
        else
        {
            space = false;
            GameManager.Instance.messageManager.SpawnMessage("Inventory full", GameManager.Instance.Player.transform, true);
            AudioManager.Instance.PlaySound(4);
        }

        return space;
    }

    public Item CloneItem(Item I)
    {
        Item item = new Item(I.id, I.name, I.damage, I.defense, I.sprite, I.cost, I.description, I.itemType);

        return item;
    }

    //Checks if inventory has space for more items
    public Slot CheckForSpace()
    {
        Slot slot = null;
        foreach (Slot S in Slots)
        {
            if(!S.EquipmentSlot)
            {
                if (!S.Used)
                {
                    slot = S;
                    break;
                }
            }
          
        }

        return slot;
    }

    //Create an item icon
    public void CreateItemIcon(Item item, Slot slot)
    {
        GameObject newIcon = (GameObject)Instantiate(itemIconPref);
        newIcon.transform.SetParent(iconsParent);

        ItemIcon icon = newIcon.GetComponent<ItemIcon>();
        icon.SetItem(item);

        ScaleObjectToResolution(icon.rectTransform);

        SetItemIcon(icon, slot);
    }

    //Set item icon to slot position
    public void SetItemIcon(ItemIcon icon, Slot slot)
    {
        ClearSlots();
        icon.transform.position = slot.transform.position;
        icon.CurrentSlot = slot;
        slot.Used = true;
        icon.CurrentItem.positionX = slot.positionX;
        icon.CurrentItem.positionY = slot.positionY;
        ItemIcons.Add(icon);

    }


    //Select an item to begin drag
    public void SelectItem(ItemIcon icon)
    {
        if (!mouseOnCooldown && iconInHand == null)
        {
            tooltip.Hide();
            StopAllCoroutines();
            SetMouseCooldown();
            SetItemInHand(icon);

        }

    }

    //When selecting an item, beggin the drag item action
    void SetItemInHand(ItemIcon icon)
    {
        ItemIcons.Remove(icon);
        if (icon.CurrentSlot != null)
        {

            if (icon.CurrentSlot.EquipmentSlot)
            {
                UnequipItem(icon.CurrentSlot);
            }

            icon.CurrentSlot.Used = false;
        }
        icon.Selected = true;
        iconInHand = icon;



    }

    //Leaves the selected item in a given position, or switch the item that was already in that position
    void MoveItemIcon(ItemIcon icon)
    {       
        Slot slot = GetSlotCloserToMouse();

        if (slot != null)
        {

            //If it is an equipment slot, only equip if the type match
            if (slot.EquipmentSlot)
            {
                if (icon.CurrentItem.itemType != slot.type)
                {
                    return;
                }
                else
                {
                    EquipItem(icon);
                }


            }

            //Switch items
            if (slot.Used)
            {
                icon.Selected = false;

                ItemIcon OtherItem = GetItemIconByPosition(slot.positionX, slot.positionY);
                OtherItem.CurrentSlot = null;
                SetItemIcon(icon, slot);
                SetItemInHand(OtherItem);
                AudioManager.Instance.PlayRandomSound(1);

            }
            else //Leave item in that position
            {
                AudioManager.Instance.PlayRandomSound(1);
                SetItemIcon(icon, slot);

                icon.Selected = false;
                iconInHand = null;
            }


            ClearSlots();
        }
        else
        {
            DropItem(iconInHand);
            iconInHand = null;
        }


    }

    //When dragging items outside of inventory space, drop it on the ground
    void DropItem(ItemIcon Icon)
    {
        if (iconInHand != null)
        {
            ClearSlots();
            Item itemToDrop = iconInHand.CurrentItem;

            AudioManager.Instance.PlaySound(1);
            SpawnItem(itemToDrop, GameManager.Instance.Player.transform.position, Vector3.down);

            Destroy(iconInHand.gameObject);
        }
    }


    //Creates an item in a given position adding a force to given direction
    public void SpawnItem(Item item, Vector3 position, Vector3 Direction)
    {
        GameObject NewItemInGround = (GameObject)Instantiate(itemInGroundPref, position, Quaternion.identity);
        ItemInGround itemInGround = NewItemInGround.GetComponent<ItemInGround>();
        itemInGround.item = item;
        itemInGround.SetImage();

        Rigidbody2D rb = itemInGround.GetComponent<Rigidbody2D>();

        rb.AddForce(Direction * Random.Range(3, 5), ForceMode2D.Impulse);
    }

    //Open inventory
    public void Open()
    {
        if (!showingPanel)
        {
            showingPanel = true;
            panel.SetActive(true);
        }
    }

    //Close Inventory
    public void Close()
    {
        if (showingPanel)
        {
            tooltip.Hide();
            showingPanel = false;
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!showingPanel) { return; }


        if (iconInHand != null)
        {
            DraggingItem();
        }
        else
        {
           MouseOverItem();
        }

        SellUI();


    }

    //If an item was selected previously, drag it with the mouse
    void DraggingItem()
    {
        iconInHand.transform.position = Input.mousePosition;

        if (!mouseOnCooldown)
        {
            //On click, leave the item in that position or switch with the item in that position
            if (Input.GetMouseButtonDown(0))
            {
                MoveItemIcon(iconInHand);

                StopAllCoroutines();
                StartCoroutine(SetMouseCooldown());
            }
            else
            {
                HighlightSlot();
            }
        }
    }

    //When pointing items with the mouse, having not selected an item
    void MouseOverItem()
    {
        Slot S = GetSlotCloserToMouse();
        if (S != null)
        {
            if (S.Used)
            {
                ItemIcon ToShow = GetItemIconByPosition(S.positionX, S.positionY);
                if (ToShow != null)
                {
                    tooltip.SetTooltip(ToShow.CurrentItem, S.transform.position, true);

                    if (Input.GetMouseButtonDown(1))
                    {
                        SellItem(ToShow);
                    }
                }

            }
            else
            {
                tooltip.Hide();
            }

        }
        else
        {
            if (tooltip.Showing)
            {
                if (tooltip.FromUI)
                {
                    tooltip.Hide();
                }
            }

        }
    }

    //Highlights slot closer to mouse
    void HighlightSlot()
    {
        ClearSlots();
        Slot highlightSlot = GetSlotCloserToMouse();
        if (highlightSlot != null)
        {
            highlightSlot.mouseOver();
        }
    }

    
    //Returns an item based on given array position
    public ItemIcon GetItemIconByPosition(int X, int Y)
    {
        ItemIcon item = null;
        foreach (ItemIcon I in ItemIcons)
        {
            if (I.CurrentItem.positionX == X && I.CurrentItem.positionY == Y)
            {
                item = I;
                break;
            }
        }
        return item;
    }

    //Returns a slot based on given array position
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

    //Returns the slot that is closer to mouse position
    public Slot GetSlotCloserToMouse()
    {
        Slot slot = null;
        float maxRange = 85f;

        Vector2 MousePosition = Input.mousePosition;

        float distance = 9999f;
        foreach (Slot S in Slots)
        {
            float distanceDelta = Vector2.Distance(S.transform.position, MousePosition);
            if (distanceDelta < distance)
            {
                slot = S;
                distance = distanceDelta;
            }
        }


        if (distance > maxRange)
        {
            slot = null;
        }

        return slot;
    }


    //Clears slots colors
    void ClearSlots()
    {
        foreach (Slot S in Slots)
        {
            if (!S.Used)
            {
                S.ClearColors();
            }
            else
            {
                S.SetUsed();
            }
        }
    }

    IEnumerator SetMouseCooldown() //Prevents multiple actions from same click
    {       
        mouseOnCooldown = true;       
        yield return new WaitForSeconds(.1f);
        mouseOnCooldown = false;
    }    

    //Modifies the gold value and updates the UI
    public void ChangeGold(int value)
    {
        gold = gold + value;
        if (gold < 0)
        {
            gold = 0;
        }

        UpdateGoldUI();
    }

    //Check if gold is enough to buy items, returns the result
    public bool CheckGold(int value)
    {
        bool EnoughGold = false;
        if (gold + value > 0)
        {
            EnoughGold = true;
        }

        return EnoughGold;
    }

    
    void UpdateGoldUI()
    {
        goldText.text = gold.ToString();
    }

    //When right clicking an item near shopkeeper, sell the item
    void SellItem(ItemIcon itemIcon)
    {
        if (nearShopKeeper)
        {
            itemIcon.CurrentSlot.Used = false;

            if(itemIcon.CurrentSlot.EquipmentSlot)
            {
                UnequipItem(itemIcon.CurrentSlot);
            }

            ChangeGold(itemIcon.CurrentItem.cost);
            ItemIcons.Remove(itemIcon);
            Destroy(itemIcon.gameObject);

            ClearSlots();

            AudioManager.Instance.PlayRandomSound(0);
        }
    }

    //Show UI when near shop keeper
    void SellUI()
    {
        if (nearShopKeeper)
        {
            if (showingPanel)
            {
                if (!SellPanel.gameObject.activeSelf)
                {
                    SellPanel.SetActive(true);
                }

            }
        }
        else
        {
            if (SellPanel.gameObject.activeSelf)
            {
                SellPanel.SetActive(false);
            }
        }
    }

    //When equiping an item, update the player stats and cast the spells
    void EquipItem(ItemIcon icon)
    {
        Equipment equipment = GameManager.Instance.Player.GetComponent<Equipment>();

        equipment.AddEquipment(icon);
    
    }

    //When unequiping items update player stats and remove spells
    void UnequipItem(Slot S)
    {
        Equipment equipment = GameManager.Instance.Player.GetComponent<Equipment>();

        equipment.RemoveEquipment(S);

       
    }

    //Update UI values
    public void SetDamageAndDefense(int damage, int defense)
    {
        damageText.text = "Damage: " + damage.ToString();
        defenseText.text = "Defense: " + defense.ToString();
    }

    //Adapts a given rectransform to different resolutions
    void ScaleObjectToResolution(RectTransform rectTransform)
    {
        rectTransform.localScale = new Vector3(rectTransform.localScale.x * resolutionMultiplerX, rectTransform.localScale.y * resolutionMultiplerY, rectTransform.localScale.z);
    }
}
