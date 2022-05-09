using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] public int selectedIndex;
    [SerializeField] public List<Item> items;

    private static PlayerController playerController;
    private static Inventory inventory;

    /// <summary>
    /// Returns whether or not an item is selected.
    /// </summary>
    public bool ItemSelected => selectedIndex != -1;

    /// <summary>
    /// Returns the selected item, or null if no item is selected.
    /// </summary>
    public Item SelectedItem => ItemSelected ? items[selectedIndex] : null;

    private void Start()
    {
        if (playerController == null)
            playerController = GameObject.Find("Player Controller").GetComponent<PlayerController>();

        if (inventory == null)
            inventory = GameObject.Find("Canvas").transform.Find("Inventory").GetComponent<Inventory>();

        selectedIndex = -1;
    }
    
    public void ManualUpdate()
    {
        if (playerController.Trigger_PreviousItem())
            IncrementSelectedItem(-1);

        else if (playerController.Trigger_NextItem())
            IncrementSelectedItem(1);
    }

    private void IncrementSelectedItem(int increment)
    {
        selectedIndex += items.Count + increment;
        selectedIndex %= items.Count;
        inventory.SelectItemSlot(selectedIndex);
    }

    public void DecreaseAmountOfSelectedItem()
    {
        if (--items[selectedIndex].amount > 0)
            return;

        items.RemoveAt(selectedIndex);

        if (items.Count == 0 || selectedIndex == items.Count)
            --selectedIndex;
    }

    public void AddItem(GameObject prefab, int amount)
    {
        if (amount <= 0)
            return;

        int i = items.FindIndex(item => item.gameObject == prefab);

        if (i == -1)
            items.Add(new Item(prefab, amount));
        else
            items[i].amount += amount;
    }

    public void Clear()
    {
        selectedIndex = -1;
        items.Clear();
    }

}
