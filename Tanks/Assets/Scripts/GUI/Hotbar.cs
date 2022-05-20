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

        playerName.text = "";
        playerHealth.text = "";
        playerFuel.text = "";
        playerAngle.text = "";
        playerPower.text = "";

        itemName.text = "";
        itemDescription.text = "";
        itemDamage.text = "";
        itemExplosiveRadius.text = "";
        itemRange.text = "";
    }

    private void Update()
    {
        if (gameManager.CurrentPlayer != player)
        {
            player = gameManager.CurrentPlayer;
            playerName.text = player == null ? "" : player.Info.name;
        }

        if (player != null && player.Inventory.SelectedItem != itemSlot.Item)
        {
            itemSlot.SetItem(player.Inventory.SelectedItem);
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

        playerHealth.text = PercentageString(player.Tank.GetHealthPercentage());
        playerFuel.text = PercentageString(player.Tank.GetFuelPercentage());
        playerAngle.text = "---";
        playerPower.text = PercentageString(player.Tank.GetPowerPercentage());
    }

    private void UpdateItemPanel()
    {
        itemName.text = itemSlot.Item.usable.Name;
        itemDescription.text = itemSlot.Item.usable.Description;

        Projectile projectile = itemSlot.Item.usable.GetComponent<Projectile>();
        bool isProjectile = projectile != null;

        itemDamage.transform.parent.gameObject.SetActive(isProjectile);
        itemExplosiveRadius.transform.parent.gameObject.SetActive(isProjectile);
        itemRange.transform.parent.gameObject.SetActive(isProjectile);

        if (isProjectile)
        {
            itemDamage.text = projectile.GetDamage().ToString("0");

            ExplodingProjectile explodingProjectile = projectile.GetComponent<ExplodingProjectile>();

            itemExplosiveRadius.text = explodingProjectile == null ? "-" : explodingProjectile.Radius.ToString("0.0" + "m");
            itemRange.text = PercentageString(1.0f);
        }
    }

    private string PercentageString(float value)
    {
        return (int)(value * 100.0f + 0.5f) + "%";
    }

}
