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
    [SerializeField] private Tank tank;
    [SerializeField] private Projectile item;

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
        Open(true);
    }

    private void Update()
    {
        if (gameManager.GetCurrentTank() != tank)
        {
            tank = gameManager.GetCurrentTank();
            playerName.text = tank == null ? "" : tank.GetPlayerName();
        }

        if (tank.GetCurrentProjectile() != item)
        {
            item = tank.GetCurrentProjectile();
            UpdateItemPanel();
            UpdateItemSlot();
        }

        UpdatePlayerPlanel();
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

    private void UpdatePlayerPlanel()
    {
        if (tank == null)
            playerHealth.text = playerFuel.text = playerAngle.text = playerPower.text = "";

        playerHealth.text = (int)(tank.GetHealthPercentage() * 100.0f + 0.5f) + "%";
        playerFuel.text = (int)(tank.GetFuelPercentage() * 100.0f + 0.5f) + "%";
        playerAngle.text = "---";
        playerPower.text = "---";
    }
    
    private void UpdateItemPanel()
    {
        itemName.text = item.name;
        itemDescription.text = "Not implemented";
        itemDamage.text = item.GetDamage().ToString("0.0") + "HP";
        itemExplosiveRadius.text = "---";
        itemRange.text = "---";
    }

    private void UpdateItemSlot()
    {
        itemSlot.itemPrefab = item.gameObject;
        //itemSlot.UpdateAmount(tank.playerInventory.GetSelectedItem());
    }
}
