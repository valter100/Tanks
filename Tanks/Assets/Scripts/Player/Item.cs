using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public Usable usable;
    public int amount;

    public Item() { }
    public Item(Usable usable, int amount)
    {
        this.usable = usable;
        this.amount = amount;
    }
}
