using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private Pause pause;
    [SerializeField] private Image overlay;

    public Inventory Inventory => inventory;
    public Hotbar Hotbar => hotbar;
    public Pause Pause => pause;
    public Image Overlay => overlay;

    private void Start()
    {
        if (Activate(inventory.gameObject))
            inventory.Start();

        if (Activate(hotbar.gameObject))
            hotbar.Start();

        if (Activate(pause.gameObject))
            pause.Start();

        overlay.gameObject.SetActive(false);

        // Ensures the GameObject is active and returns whether or not it was already activated
        bool Activate(GameObject gameObject)
        {
            if (gameObject.activeSelf)
                return false;

            gameObject.SetActive(true);
            return true;
        }
    }

    private void Update()
    {
        if (playerController.Trigger_Back())
        {
            if (pause.Open)
                pause.SetOpen(false);

            else if (inventory.Open)
                inventory.SetOpen(false);

            else
                pause.SetOpen(true);
        }
    }

}
