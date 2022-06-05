using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool open;
    [SerializeField] private int selectedItemSlot;
    [SerializeField] private ItemSlot[] itemSlots;

    [Header("Icon rotation")]
    [SerializeField] private Quaternion iconEulerRotation;
    [SerializeField] private Vector3 iconRotation;
    [SerializeField] private Vector3 iconAngularVelocity;

    [Header("Temporary references")]
    [SerializeField] private Player player;

    [Header("Permanent references")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private GridLayoutGroup grid;
    
    public bool Open => open;
    public Quaternion IconRotation => iconEulerRotation;

    private void OnEnable() => Start();

    private void Start()
    {
        itemSlots = grid.transform.GetComponentsInChildren<ItemSlot>();
        selectedItemSlot = -1;

        foreach (ItemSlot itemSlot in itemSlots)
            itemSlot.ManualStart();
    }

    private void Update()
    {
        if (playerController.Trigger_Inventory() && !gameManager.Paused && player != null && player.Info.control == Control.Player)
            ToggleOpen();

        if (gameManager.CurrentPlayer == null)
        {
            if (player != null)
                Clear();
        }

        else if (gameManager.CurrentPlayer != player)
        {
            Link(gameManager.CurrentPlayer);
            SetOpen(false);
        }

        iconRotation += iconAngularVelocity * Time.deltaTime;
        iconRotation.x %= 360f;
        iconRotation.y %= 360f;
        iconRotation.z %= 360f;
        iconEulerRotation = Quaternion.Euler(iconRotation);
    }

    public void Clear()
    {
        player = null;
        selectedItemSlot = -1;

        for (int i = itemSlots.Length - 1; i >= 0; --i)
        {
            itemSlots[i].Deselect();
            itemSlots[i].SetItem(null);
        }
    }

    public void Reload(Player player)
    {
        if (this.player == player)
            Link(player);
    }

    private void Link(Player player)
    {
        this.player = player;
        SelectItemSlot(player.Inventory.selectedIndex);

        for (int i = player.Inventory.items.Count - 1; i >= 0; --i)
        {
            itemSlots[i].SetItem(player.Inventory.items[i]);
        }

        for (int i = player.Inventory.items.Count; i < itemSlots.Length; ++i)
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
        if (player == null)
            return;

        if (selectedItemSlot != -1)
            itemSlots[selectedItemSlot].Deselect();

        if (i != -1)
            itemSlots[i].Select();

        player.Inventory.selectedIndex = selectedItemSlot = i;
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
