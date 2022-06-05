using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool open;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private TextMeshProUGUI text;

    public bool Open => open;

    public void SetOpen(bool open)
    {
        this.open = open;
        GetComponent<Image>().enabled = open;

        for (int i = transform.childCount - 1; i >= 0; --i)
            transform.GetChild(i).gameObject.SetActive(open);

        gameManager.SetPaused(open);
        guiManager.Hotbar.SetOpen(!open, true);
        guiManager.Inventory.SetOpen(false);
        guiManager.Overlay.gameObject.SetActive(open);
    }

    public void OnClick_Rematch()
    {
        SetOpen(false);
        gameManager.RestartMatch();
    }

    public void OnClick_Exit()
    {
        Exit.ExitApplication();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

}
