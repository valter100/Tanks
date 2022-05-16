using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private bool open;
    [SerializeField] private PlayerController playerController;
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
        if (playerController.Trigger_Inventory())
            ToggleOpen();

        if (gameManager.CurrentPlayer == null)
        {
            if (playerInventory != null)
                Clear();
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
        {
            itemSlots[i].Deselect();
            itemSlots[i].SetItem(null);
        }
    }

    public void Reload(PlayerInventory playerInventory)
    {
        if (this.playerInventory == playerInventory)
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
            itemSlots[i].SetItem(null);
        }
    }

    public void ItemSlotClicked(ItemSlot itemSlot)
    {
        for (int i = 0; i < itemSlots.Length; ++i)
        {
            if (itemSlot == itemSlots[i])
            {
                SelectItemSlot(i);
                break;
            }
        }
    }

    public void SelectItemSlot(int i)
    {
        if (playerInventory == null)
            return;

        if (selectedItemSlot != -1)
            itemSlots[selectedItemSlot].Deselect();

        if (i != -1)
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
