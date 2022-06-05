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
    [SerializeField] private EndScreen endScreen;
    [SerializeField] private Image overlay;

    public Inventory Inventory => inventory;
    public Hotbar Hotbar => hotbar;
    public Pause Pause => pause;
    public Image Overlay => overlay;
    public EndScreen EndScreen => endScreen;

    private void Start()
    {
        if (!inventory.gameObject.activeSelf)
            inventory.gameObject.SetActive(true);

        if (!hotbar.gameObject.activeSelf)
            hotbar.gameObject.SetActive(true);

        if (!pause.gameObject.activeSelf)
            pause.gameObject.SetActive(true);

        if (!endScreen.gameObject.activeSelf)
            endScreen.gameObject.SetActive(true);

        overlay.gameObject.SetActive(false);
        endScreen.SetOpen(false);
        pause.SetOpen(false);
        hotbar.SetOpen(false, true);
        inventory.SetOpen(false);
    }

    private void Update()
    {
        if (playerController.Trigger_Back())
        {
            if (endScreen.Open)
                endScreen.OnClick_Exit();

            else if (pause.Open)
                pause.SetOpen(false);

            else if (inventory.Open)
                inventory.SetOpen(false);

            else
                pause.SetOpen(true);
        }
    }

}
