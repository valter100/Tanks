using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private bool open;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private int selectedItemSlot;
    [SerializeField] private ItemSlot[] itemSlots;

    /// <summary>
    /// Returns whether or not the invenrory is open.
    /// </summary>
    public bool Open => open;

    void Start()
    {
        itemSlots = grid.transform.GetComponentsInChildren<ItemSlot>();
        selectedItemSlot = -1;
        SetOpen(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleOpen();

        if (gameManager.CurrentPlayer == null)
        {
            if (selectedItemSlot != -1)
            {
                selectedItemSlot = -1;
                for (int i = 0; i < itemSlots.Length; ++i)
                    itemSlots[i].Clear();
            }
        }

        else if (gameManager.CurrentPlayer.Inventory != playerInventory)
        {
            Link(gameManager.CurrentPlayer.Inventory);
        }
    }

    public void Clear()
    {
        playerInventory = null;
        selectedItemSlot = -1;

        for (int i = itemSlots.Length - 1; i >= 0; --i)
            itemSlots[i].Clear();
    }

    public void Reload()
    {
        Link(playerInventory);
    }

    private void Link(PlayerInventory playerInventory)
    {
        this.playerInventory = playerInventory;
        SelectItemSlot(playerInventory.selectedIndex);

        for (int i = playerInventory.items.Count - 1; i >= 0; --i)
        {
            itemSlots[i].item = playerInventory.items[i];
        }

        for (int i = playerInventory.items.Count; i < itemSlots.Length; ++i)
        {
            itemSlots[i].Clear();
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
        playerInventory.selectedIndex = selectedItemSlot = i;
    }

    public void SetOpen(bool open)
    {
        this.open = open;
        GetComponent<Image>().enabled = open;

        for (int i = transform.childCount - 1; i >= 0; --i)
            transform.GetChild(i).gameObject.SetActive(open);
    }

    public void ToggleOpen()
    {
        SetOpen(!open);
    }

}
