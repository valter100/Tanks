using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInfo info;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Tank tank;

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
        tank.ManualUpdate();
    }

    public void Initialize(PlayerInfo info)
    {
        this.info = info;
        tank.LinkPlayer(this);
    }

    public void Ready()
    {
        tank.Ready();
    }

    public void Unready()
    {
        tank.Unready();
    }
}
