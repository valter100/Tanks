using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hotbar : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool open;

    [Header("Temporary references")]
    [SerializeField] private Player player;

    [Header("Permanent references")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private ItemSlot itemSlot;

    [Header("Player panel references")]
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private TextMeshProUGUI playerFuel;
    [SerializeField] private TextMeshProUGUI playerAngle;
    [SerializeField] private TextMeshProUGUI playerPower;

    [Header("Item panel references")]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI itemDamage;
    [SerializeField] private TextMeshProUGUI itemExplosiveRadius;
    [SerializeField] private TextMeshProUGUI itemRange;

    private void Start()
    {
        SetOpen(true);
    }

    private void Update()
    {
        if (gameManager.CurrentPlayer != player)
        {
            player = gameManager.CurrentPlayer;
            playerName.text = player == null ? "" : player.Info.name;
        }

        if (player != null && player.Inventory.SelectedItem != itemSlot.item)
        {
            itemSlot.item = player.Inventory.SelectedItem;
            UpdateItemPanel();
        }

        UpdatePlayerPlanel();
    }

    public void SetOpen(bool open)
    {
        this.open = open;
        GetComponent<Image>().enabled = open;

        for (int i = transform.childCount - 1; i >= 0; --i)
            transform.GetChild(i).gameObject.SetActive(open);
    }

    private void UpdatePlayerPlanel()
    {
        if (player == null)
        {
            playerHealth.text = playerFuel.text = playerAngle.text = playerPower.text = "";
            return;
        }

        playerHealth.text = (int)(player.Tank.GetHealthPercentage() * 100.0f + 0.5f) + "%";
        playerFuel.text = (int)(player.Tank.GetFuelPercentage() * 100.0f + 0.5f) + "%";
        playerAngle.text = "---";
        playerPower.text = "---";
    }
    
    private void UpdateItemPanel()
    {
        itemName.text = itemSlot.item.prefab.name.SplitPascalCase();
        itemDescription.text = "---";
        itemDamage.text = "---";
        itemExplosiveRadius.text = "---";
        itemRange.text = "---";
    }

}
