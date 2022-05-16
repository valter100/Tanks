using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInfo info;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Tank tank;
    [SerializeField] private AiTank aiTank;
    [SerializeField] private PlayerTank playerTank;

    private static GameManager gameManager;

    public PlayerInfo Info => info;
    public PlayerInventory Inventory => inventory;
    public Tank Tank => tank;


    private void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (gameManager.CurrentPlayer != this)
            return;

        inventory.ManualUpdate();

        if (playerTank)
            playerTank.ManualPlayerUpdate();
        else if (aiTank)
            aiTank.ManualAIUpdate();
    }

    public void Initialize(string name, Color color, Prefab tankPrefab, Control control, Vector3 position)
    {
        // Info

        info.name = name;
        info.color = color;
        info.tankPrefab = tankPrefab;
        info.control = control;

        // Tank

        if (info.control == Control.Player)
            playerTank = tank.GetComponent<PlayerTank>();
        else if (info.control == Control.Bot)
            aiTank = tank.GetComponent<AiTank>();


        tank = Instantiate(Prefabs.Tanks[tankPrefab], transform).GetComponent<Tank>();
        
        if (playerTank)
            playerTank.LinkPlayer(this);
        else if (aiTank)
            aiTank.LinkPlayer(this);

        tank.transform.position = position;

        // Inventory

        inventory.Clear();
        foreach (GameObject prefab in Prefabs.Items.Values)
            inventory.AddItem(prefab, 12);
    }

    public void Ready()
    {
        if (playerTank)
            playerTank.Ready();
        else if (aiTank)
            aiTank.Ready();
    }

    public void Unready()
    {
        tank.Unready();
    }
}
