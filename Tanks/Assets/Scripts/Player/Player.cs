using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInfo info;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Tank tank;
    [SerializeField] private GameObject playerHudPrefab;
    [SerializeField] private PlayerHUD playerHud;

    private static GameManager gameManager;

    public PlayerInfo Info => info;
    public PlayerInventory Inventory => inventory;
    public Tank Tank => tank;

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ManualUpdate()
    {
        inventory.ManualUpdate();
        tank.ManualUpdate();
    }

    public void Initialize(string name, Color color, Prefab tankPrefab, Control control, Vector3 position)
    {
        // Info

        info.name = name;
        info.color = color;
        info.tankPrefab = tankPrefab;
        info.control = control;

        // Tank

        GameObject tankObject = Instantiate(Prefabs.Tanks[tankPrefab], transform);

        if (control == Control.Player)
        {
            tank = tankObject.GetComponent<PlayerTank>();
            tankObject.GetComponent<PlayerController>().enabled = true;

            Destroy(tankObject.GetComponent<AiTank>());
            Destroy(tankObject.GetComponent<AiManager>());
        }

        else if (control == Control.Bot)
        {
            tank = tankObject.GetComponent<AiTank>();
            tankObject.GetComponent<AiManager>().enabled = true;

            Destroy(tankObject.GetComponent<PlayerTank>());
            Destroy(tankObject.GetComponent<PlayerController>());
        }

        tank.enabled = true;
        tank.transform.position = position;
        tank.LinkPlayer(this);

        // Inventory

        inventory.Clear();

        foreach (GameObject prefab in Prefabs.Usables.Values)
            inventory.AddItem(prefab.GetComponent<Usable>(), prefab.GetComponent<Usable>().GetStartAmount(), false);

        inventory.IncrementSelectedItem(Random.Range(0, inventory.items.Count));

        // PlayerHUD

        GameObject canvas = GameObject.Find("Canvas");
        playerHud = Instantiate(playerHudPrefab, canvas.transform).GetComponent<PlayerHUD>();
        playerHud.LinkPlayer(this);
        playerHud.transform.SetAsFirstSibling();
    }

    public void Ready()
    {
        tank.Ready();
    }

    public void Unready()
    {
        tank.Unready();
    }

    private void OnDestroy()
    {
        if (playerHud != null)
            Destroy(playerHud.gameObject);
    }

}
