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

        ChangeGold(3000);

        Slots.Add(SpellSlot);
        Slots.Add(RingSlot);
        Slots.Add(ConsumableSlot);
    }

    void CreateSlots()
    {
       float resolutionMultiplerX = Screen.width / 1280f;
       float resolutionMultiplerY = Screen.height / 720f;

        Vector2 slotsOffsetScaled =  new Vector2(slotOffset.x * resolutionMultiplerX, slotOffset.y * resolutionMultiplerY);

        for (int i = 0; i < inventorySize.y; i++)
        {
            for (int j = 0; j < inventorySize.x; j++)
            {
                GameObject newSlot = (GameObject)Instantiate(slotPref);
                newSlot.transform.SetParent(slotParent);
                newSlot.transform.position = new Vector3(slotParent.transform.position.x + slotsOffsetScaled.x * j, slotParent.transform.position.y - slotsOffsetScaled.y * i, slotParent.transform.position.z);
                RectTransform rectTransform = newSlot.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(rectTransform.localScale.x * resolutionMultiplerX, rectTransform.localScale.y * resolutionMultiplerY, rectTransform.localScale.z);

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

    public Item SearchItemByID(List<Item> list, int ID)
    {
        Item item = null;
        foreach (Item I in list)
        {
            if (I.id == ID)
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

    public void CreateItemIcon(Item item, Slot slot)
    {
        GameObject newIcon = (GameObject)Instantiate(itemIconPref);
        newIcon.transform.SetParent(iconsParent);

        ItemIcon icon = newIcon.GetComponent<ItemIcon>();
        icon.SetItem(item);


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
        ItemIcons.Add(icon);

    }



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


    void MoveItemIcon(ItemIcon icon)
    {       
        Slot slot = GetSlotCloserToMouse();

        if (slot != null)
        {
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

            if (slot.Used)
            {
                icon.Selected = false;

                ItemIcon OtherItem = GetItemIconByPosition(slot.positionX, slot.positionY);
                OtherItem.CurrentSlot = null;
                SetItemIcon(icon, slot);
                SetItemInHand(OtherItem);
                AudioManager.Instance.PlayRandomSound(1);



            }
            else
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

    public void SpawnItem(Item item, Vector3 position, Vector3 Direction)
    {
        GameObject NewItemInGround = (GameObject)Instantiate(itemInGroundPref, position, Quaternion.identity);
        ItemInGround itemInGround = NewItemInGround.GetComponent<ItemInGround>();
        itemInGround.item = item;
        itemInGround.SetImage();

        Rigidbody2D rb = itemInGround.GetComponent<Rigidbody2D>();

        rb.AddForce(Direction * Random.Range(3, 5), ForceMode2D.Impulse);
    }

    public void Open()
    {
        if (!showingPanel)
        {
            showingPanel = true;
            panel.SetActive(true);
        }
    }

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
            iconInHand.transform.position = Input.mousePosition;

            if (!mouseOnCooldown)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MoveItemIcon(iconInHand);
                    
                    StopAllCoroutines();
                    StartCoroutine(SetMouseCooldown());
                }
                else
                {
                    ClearSlots();
                    Slot highlightSlot = GetSlotCloserToMouse();
                    if (highlightSlot != null)
                    {
                        highlightSlot.mouseOver();
                    }

                }
            }

        }
        else
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
                    else
                    {
                        print("itemNull");
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

        SellUI();



    }


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
        float maxRange = 65f;

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

    void MouseCooldown()
    {
        mouseOnCooldown = false;
    }


    public void ChangeGold(int value)
    {
        gold = gold + value;
        if (gold < 0)
        {
            gold = 0;
        }

        UpdateGoldUI();
    }

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


    void EquipItem(ItemIcon icon)
    {
        Equipment equipment = GameManager.Instance.Player.GetComponent<Equipment>();

        equipment.AddEquipment(icon);
    
    }

    void UnequipItem(Slot S)
    {
        Equipment equipment = GameManager.Instance.Player.GetComponent<Equipment>();

        equipment.RemoveEquipment(S);

       
    }

    public void SetDamageAndDefense(int damage, int defense)
    {
        damageText.text = "Damage: " + damage.ToString();
        defenseText.text = "Defense: " + defense.ToString();
    }
}
