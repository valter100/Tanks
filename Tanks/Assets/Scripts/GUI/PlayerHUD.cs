using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider fuelBar;
    [SerializeField] private Slider powerBar;
    [SerializeField] private FollowInWorldObject followInWorldObject;
    [SerializeField] private Vector3 offsetAdjustment;

    private static GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (player == null)
            return;

        // Deactivate if destroyed

        if (player.gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(false);
            return;
        }

        // Show/hide fuel and power bar
        
        if (fuelBar.gameObject.activeInHierarchy && player != gameManager.CurrentPlayer)
        {
            fuelBar.gameObject.SetActive(false);
            powerBar.gameObject.SetActive(false);

            playerName.transform.localPosition -= offsetAdjustment;
            healthBar.transform.localPosition -= offsetAdjustment;
        }
            
        else if (fuelBar.gameObject.activeInHierarchy == false && player == gameManager.CurrentPlayer)
        {
            fuelBar.gameObject.SetActive(true);
            powerBar.gameObject.SetActive(true);

            playerName.transform.localPosition += offsetAdjustment;
            healthBar.transform.localPosition += offsetAdjustment;
        }
            
        // Update values

        if (healthBar.value != player.Tank.GetHealthPercentage())
            healthBar.value = player.Tank.GetHealthPercentage();

        if (fuelBar.value != player.Tank.GetFuelPercentage())
            fuelBar.value = player.Tank.GetFuelPercentage();

        if (powerBar.value != player.Tank.GetPowerPercentage())
            powerBar.value = player.Tank.GetPowerPercentage();
    }

    public void LinkPlayer(Player player)
    {
        this.player = player;

        playerName.text = player.Info.name;
        healthBar.value = player.Tank.GetHealthPercentage();
        fuelBar.value = player.Tank.GetFuelPercentage();

        GetComponent<FollowInWorldObject>().followObject = player.Tank.transform;
    }
}
