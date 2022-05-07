using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private bool open;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private int selectedItemSlot;
    [SerializeField] private ItemSlot[] itemSlots;

    void Start()
    {
        itemSlots = (ItemSlot[])grid.transform.GetComponentsInChildren(typeof(ItemSlot));
        selectedItemSlot = -1;
        Open(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Open(!open);
    }

    public void Clear()
    {
        playerInventory = null;
        selectedItemSlot = -1;

        for (int i = itemSlots.Length - 1; i >= 0; --i)
        {
            itemSlots[i].itemPrefab = null;
            itemSlots[i].UpdateAmount(0);
        }
    }

    public void Reload()
    {
        Link(playerInventory);
    }

    public void Link(PlayerInventory playerInventory)
    {
        this.playerInventory = playerInventory;
        SelectItemSlot(playerInventory.selectedItem);

        for (int i = playerInventory.items.Count - 1; i >= 0; --i)
        {
            itemSlots[i].itemPrefab = playerInventory.items[i].gameObject;
            itemSlots[i].UpdateAmount(playerInventory.items[i].amount);
        }

        for (int i = playerInventory.items.Count; i < itemSlots.Length; ++i)
        {
            itemSlots[i].itemPrefab = null;
            itemSlots[i].UpdateAmount(0);
        }
    }

    public void ItemSlotClicked(ItemSlot itemSlot)
    {
        int i = -1;
        while (itemSlots[++i] != itemSlot) ;
        SelectItemSlot(i);
    }

    public void SelectItemSlot(int i)
    {
        if (!open)
            return;

        if (selectedItemSlot >= 0 && selectedItemSlot < itemSlots.Length)
            itemSlots[selectedItemSlot].Deselect();

        itemSlots[i].Select();
        playerInventory.selectedItem = selectedItemSlot = i;
    }

    public void Open(bool open)
    {
        if (this.open == open)
            return;

        this.open = open;
        GetComponent<Image>().enabled = open;

        for (int i = transform.childCount - 1; i >= 0; --i)
            transform.GetChild(i).gameObject.SetActive(open);
    }

}
