using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : Menu
{
    [SerializeField] Vector3 cameraPosition;
    [SerializeField] Quaternion cameraRotation;

    public List<CustomizationPanel> customizationPanels;

    protected override void Start()
    {
        base.Start();
        customizationPanels = new List<CustomizationPanel>();
    }

    protected override void Update()
    {

    }

    protected override void OnEnable()
    {
        Camera.main.transform.localPosition = cameraPosition;
        Camera.main.transform.rotation = cameraRotation;
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
        GameInfo gameInfo = GameObject.Find("Game Info").GetComponent<GameInfo>();
        gameInfo.SaveInfo();

        SceneManager.LoadScene(1);
        Disable();
    }

}
