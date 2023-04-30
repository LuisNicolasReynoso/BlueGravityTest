using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Spell;

    public int Damage;
    public int Defense;

    void Start()
    {
        
    }

    public void CastSpell(Item item)
    {
        if(Spell != null)
        {
            Destroy(Spell.gameObject);
        }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Spells/" + item.name);
        Spell = (GameObject)Instantiate(prefab, this.transform.position, prefab.transform.rotation);
    }

    public void RemoveSpell()
    {
        Destroy(Spell.gameObject);
    }

    public void AddEquipment(ItemIcon icon)
    {
        switch (icon.CurrentItem.itemType)
        {
            case Item.Type.Spell:

                Inventory.Instance.EquipedSpell = icon;
                CastSpell(icon.CurrentItem);

                break;

            case Item.Type.Jewerly:
                Inventory.Instance.EquipedRing = icon;
                break;

            case Item.Type.Consumable:
                Inventory.Instance.EquipedConsumable = icon;
                break;
        }


        CalculateStats();
    }

    public void RemoveEquipment(Slot S)
    {
        switch (S.type)
        {
            case Item.Type.Spell:

                Inventory.Instance.EquipedSpell = null;
                RemoveSpell();

                break;

            case Item.Type.Jewerly:
                Inventory.Instance.EquipedRing = null;
                break;

            case Item.Type.Consumable:
                Inventory.Instance.EquipedConsumable = null;
                break;
        }

        CalculateStats();
    }

    public void CalculateStats()
    {
        Damage = 0;
        Defense = 0;

        if(Inventory.Instance.EquipedSpell != null)
        {
            Item spell = Inventory.Instance.EquipedSpell.CurrentItem;
            Damage += spell.damage;
            Defense += spell.defense;
        }

        if(Inventory.Instance.EquipedRing != null)
        {
            Item ring = Inventory.Instance.EquipedRing.CurrentItem;
            Damage += ring.damage;
            Defense += ring.defense;
        }
        
        if(Inventory.Instance.EquipedConsumable != null)
        {
            Item consumable = Inventory.Instance.EquipedConsumable.CurrentItem;
            Damage += consumable.damage;
            Defense += consumable.defense;
        }


        Inventory.Instance.SetDamageAndDefense(Damage, Defense);
       
    }
}
