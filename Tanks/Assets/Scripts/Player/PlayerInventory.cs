using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] public int selectedIndex;
    [SerializeField] public List<Item> items;

    private PlayerController playerController;
    private static Inventory inventory;
    private Player player;

    /// <summary>
    /// Returns whether or not an item is selected.
    /// </summary>
    public bool ItemSelected => selectedIndex != -1;

    /// <summary>
    /// Returns the selected item, or null if no item is selected.
    /// </summary>
    public Item SelectedItem => ItemSelected ? items[selectedIndex] : null;

    private void Awake()
    {
        if (inventory == null)
            inventory = GameObject.Find("Canvas").transform.Find("Inventory").GetComponent<Inventory>();

        items = new List<Item>();
        playerController = GetComponent<PlayerController>();
        selectedIndex = -1;
        player = transform.parent.GetComponent<Player>();
    }
    
    public void ManualUpdate()
    {
        if (playerController.Trigger_PreviousItem())
            IncrementSelectedItem(-1);

        else if (playerController.Trigger_NextItem())
            IncrementSelectedItem(1);
    }

    public void IncrementSelectedItem(int increment)
    {
        selectedIndex += items.Count + increment;
        selectedIndex %= items.Count;
        inventory.SelectItemSlot(selectedIndex);
    }

    public void DecreaseAmountOfSelectedItem()
    {
        if (--items[selectedIndex].amount == 0)
            RemoveItem(selectedIndex);
    }

    public void AddItem(Usable usable, int amount, bool message)
    {
        if (usable == null || amount <= 0)
            return;

        if (message)
            MessagesManager.AddMessage("+" + amount + " " + usable.Name + "s").SetColor(Color.yellow).SetDuration(3.5f).SetWorldPosition(player.Tank.transform.position);

        // Add new Item or increase amount of already existing Item

        int i = items.FindIndex(item => item.usable == usable);

        if (i == -1)
            items.Add(new Item(usable, amount));
        else
            items[i].amount += amount;

        // Select Item if none was selected before

        if (!ItemSelected)
            IncrementSelectedItem(1);

        // Order Items and save selected item

        else
        {
            Item selectedItem = SelectedItem;
            Order();
            selectedIndex = items.FindIndex(item => item == selectedItem);
        }

        inventory.Reload(player);
    }

    public void RemoveItem(int i)
    {
        if (i >= items.Count)
            return;

        items.RemoveAt(i);

        if (i < selectedIndex)
            IncrementSelectedItem(-1);

        else if (i == selectedIndex)
        {
            if (items.Count == 0 || i == items.Count)
                IncrementSelectedItem(-1);
        }
    }

    public void Clear()
    {
        selectedIndex = -1;
        items.Clear();
        inventory.SelectItemSlot(-1);
    }

    private void Order()
    {
        items = items.OrderBy(item => item.usable.name).ToList();
    }

}
