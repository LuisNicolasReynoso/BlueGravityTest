using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int id;
    public string name;

    public int damage;
    public int defense;

    public string sprite;

    public int positionX;
    public int positionY;

    public int cost;
    public string description;

    public Item(int ID, string Name, int Damage, int Defense, string spritePath, int Cost, string Description)
    {
        id = ID;
        name = Name;
        damage = Damage;
        defense = Defense;
        sprite = spritePath;
        cost = Cost;
        description = Description;
    }
}
