using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    [SerializeField] Vector3 cameraPosition;
    [SerializeField] Quaternion cameraRotation;
    [SerializeField] Tank mainTank;

    protected override void Start()
    {
        base.Start();

        Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue, Color.cyan };
        mainTank.SetColor(colors[Random.Range(0, colors.Length - 1)]);
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

    public void OnClick_Play()
    {
        menuManager.SwapMenu(this, Type.Play);
    }

    public void OnClick_Settings()
    {
        menuManager.SwapMenu(this, Type.Settings);
    }

    public void OnClick_Statistics()
    {
        menuManager.SwapMenu(this, Type.Statistics);
    }

    public void OnClick_HowToPlay()
    {
        menuManager.SwapMenu(this, Type.HowToPlay);
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
