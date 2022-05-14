using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CustomizationPanel : MonoBehaviour
{
    [SerializeField] public PlayerInfo playerInfo;
    [SerializeField] private int colorIndex;

    [SerializeField] private TextMeshProUGUI nametag;
    [SerializeField] private TextMeshProUGUI colorOption;
    [SerializeField] private TextMeshProUGUI tankOption;
    [SerializeField] private TextMeshProUGUI controlOption;

    [SerializeField] private string[] colorNames;
    [SerializeField] private Color[] colors;

    private void Start()
    {
        playerInfo.color = colors[colorIndex];
        UpdateOptionTexts();
        transform.GetComponentInParent<PlayMenu>().customizationPanels.Add(this);
        nametag.text = "";
    }

    public void Update()
    {
        playerInfo.name = nametag.text;
    }

    public void OnClick_PreviousColor()
    {
        if (--colorIndex == -1)
            colorIndex = colors.Length - 1;

        UpdateTankColor();
        UpdateOptionTexts();
    }

    public void OnClick_NextColor()
    {
        if (++colorIndex == colors.Length)
            colorIndex = 0;

        UpdateTankColor();
        UpdateOptionTexts();
    }

    public void OnClick_PreviousTank()
    {
        playerInfo.tankPrefab.SetPrevious();

        if (!Prefabs.Tanks.ContainsKey(playerInfo.tankPrefab))
            playerInfo.tankPrefab = Prefabs.Tanks.Last().Key;

        UpdateTankType();
        UpdateOptionTexts();
    }

    public void OnClick_NextTank()
    {
        playerInfo.tankPrefab.SetNext();

        if (!Prefabs.Tanks.ContainsKey(playerInfo.tankPrefab))
            playerInfo.tankPrefab = Prefabs.Tanks.First().Key;

        UpdateTankType();
        UpdateOptionTexts();
    }

    public void OnClick_PreviousControl()
    {
        playerInfo.control.SetPrevious();
        UpdateOptionTexts();
    }

    public void OnClick_NextControl()
    {
        playerInfo.control.SetNext();
        UpdateOptionTexts();
    }

    private void UpdateOptionTexts()
    {
        colorOption.text = colorNames[colorIndex];
        tankOption.text = playerInfo.tankPrefab.ToString().SplitPascalCase();
        controlOption.text = playerInfo.control.ToString().SplitPascalCase();
    }

    private void UpdateTankColor()
    {
        playerInfo.color = colors[colorIndex];
        //Tank tank = playerInfo.gameObject.transform.GetComponentInChildren<Tank>();
        //tank.SetColor(playerInfo.color);
    }

    private void UpdateTankType()
    {
        
    }

}
