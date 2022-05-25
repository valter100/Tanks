using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool open;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GUIManager guiManager;

    public bool Open => open;

    public void Start()
    {
        SetOpen(false);
    }

    private void Update()
    {

    }

    public void SetOpen(bool open)
    {
        this.open = open;
        GetComponent<Image>().enabled = open;

        for (int i = transform.childCount - 1; i >= 0; --i)
            transform.GetChild(i).gameObject.SetActive(open);

        gameManager.SetPaused(open);
        guiManager.Hotbar.SetHidden(open);
        guiManager.Overlay.gameObject.SetActive(open);
    }

    public void OnClick_Resume()
    {
        SetOpen(false);
    }

    public void OnClick_Restart()
    {
        SetOpen(false);
        gameManager.RestartMatch();
    }

    public void OnClick_Settings()
    {

    }

    public void OnClick_Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}
