using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public GameObject prefab;
    public int amount;

    public Item() { }
    public Item(GameObject prefab, int amount)
    {
        this.prefab = prefab;
        this.amount = amount;
    }
}
