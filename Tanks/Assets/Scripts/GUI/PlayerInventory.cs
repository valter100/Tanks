using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public class Item
    {
        public GameObject gameObject;
        public int amount;

        public Item() { }
        public Item(GameObject gameObject, int amount)
        {
            this.gameObject = gameObject;
            this.amount = amount;
        }
    }

    [SerializeField] public int selectedItem;
    [SerializeField] public List<Item> items;

    private PlayerController playerController;
    private Inventory inventory;

    /// <summary>
    /// Returns whether or not an item is selected.
    /// </summary>
    public bool HasItemSelected() => selectedItem != -1;

    /// <summary>
    /// Returns the selected item, or null if no item is selected.
    /// </summary>
    public Item GetSelectedItem() => HasItemSelected() ? items[selectedItem] : null;

    private void Start()
    {
        playerController = GameObject.Find("Player Controller").GetComponent<PlayerController>();
        inventory = GameObject.Find("Canvas").transform.Find("Inventory").GetComponent<Inventory>();
        selectedItem = -1;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            IncrementSelectedItem(false);

        else if (Input.GetKeyDown(KeyCode.E))
            IncrementSelectedItem(true);
    }

    private void IncrementSelectedItem(bool increment)
    {
        selectedItem += items.Count + (increment ? 1 : -1);
        selectedItem %= items.Count;

        inventory.SelectItemSlot(selectedItem);
    }

    public void DecreaseAmountOfSelectedItem()
    {
        if (--items[selectedItem].amount > 0)
            return;

        items.RemoveAt(selectedItem);

        if (items.Count == 0 || selectedItem == items.Count)
            --selectedItem;
    }

    public void AddItem(GameObject gameObject, int amount)
    {
        if (amount <= 0)
            return;

        int i = items.FindIndex(item => item.gameObject == gameObject);

        if (i == -1)
            items.Add(new Item(gameObject, amount));
        else
            items[i].amount += amount;
    }

}
