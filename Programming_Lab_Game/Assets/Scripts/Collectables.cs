using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable
{
    //features
    public int coinValue;
    public int rarity;
    public enum eRar
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public Collectable()
    {
        Debug.Log("collectable called");
    }

    void PickUp()
    {
        Debug.Log("item picked up");
    }
}
