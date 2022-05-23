using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : Menu
{
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private Quaternion cameraRotation;
    [SerializeField] private Button startButton;

    public List<CustomizationPanel> customizationPanels;

    protected override void Start()
    {
        base.Start();
        customizationPanels = new List<CustomizationPanel>();
    }

    protected override void Update()
    {
        if (playerController.Trigger_Back())
            OnClick_Back();
    }

    protected override void OnEnable()
    {
        Camera.main.transform.localPosition = cameraPosition;
        Camera.main.transform.rotation = cameraRotation;

        AddPlayerButton[] addPlayerButtons = transform.GetComponentsInChildren<AddPlayerButton>();

        foreach (AddPlayerButton addPlayerButton in addPlayerButtons)
            if (addPlayerButton.gameObject.activeInHierarchy && addPlayerButton.playerSpot.GetComponent<PlayerSpot>().StartWithTank)
                addPlayerButton.OnClick_AddPlayer();
    }

    protected override void OnDisable()
    {

    }

    public void OnClick_Back()
    {
        menuManager.SwapMenu(this, Type.Main);
    }

    public void OnClick_Start()
    {
        if (customizationPanels.Count < 2)
            return;

        GameInfo gameInfo = GameObject.Find("Game Info").GetComponent<GameInfo>();
        gameInfo.SaveInfo();

        SceneManager.LoadScene(1);
        Disable();
    }

}
