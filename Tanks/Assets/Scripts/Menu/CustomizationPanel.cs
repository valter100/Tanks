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

    private static string[] colorNames =
    {
        "Red", "Orange", "Yellow",
        "Green", "Dark green", "Cyan",
        "Light blue", "Dark blue", "Purple",
        "Pink", "Magenta", "White",
        "Light gray", "Dark gray", "Black"
    };

    private static Color[] colors =
    {
        new Color(255,   0,   0),
        new Color(255, 140,   0),
        new Color(255, 255,   0),
        new Color(  0, 255,   0),
        new Color(  0, 160,   0),
        new Color(  0, 255, 255),
        new Color(  0, 170, 255),
        new Color(  0,   0, 255),
        new Color(135,   0, 255),
        new Color(255,   0, 255),
        new Color(255,   0, 150),
        new Color(255, 255, 255),
        new Color(140, 140, 140),
        new Color( 60,  60,  60),
        new Color(  0,   0,   0)
    };

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

        playerInfo.color = colors[colorIndex];

        // TODO: Change update tank

        UpdateOptionTexts();
    }

    public void OnClick_NextColor()
    {
        if (++colorIndex == colors.Length)
            colorIndex = 0;

        playerInfo.color = colors[colorIndex];

        // TODO: Change update tank

        UpdateOptionTexts();
    }

    public void OnClick_PreviousTank()
    {
        playerInfo.tankPrefab.SetPrevious();

        if (!Prefabs.Tanks.ContainsKey(playerInfo.tankPrefab))
            playerInfo.tankPrefab = Prefabs.Tanks.Last().Key;

        // TODO: Change update tank

        UpdateOptionTexts();
    }

    public void OnClick_NextTank()
    {
        playerInfo.tankPrefab.SetNext();

        if (!Prefabs.Tanks.ContainsKey(playerInfo.tankPrefab))
            playerInfo.tankPrefab = Prefabs.Tanks.First().Key;

        // TODO: Change update tank

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

    public void UpdateOptionTexts()
    {
        colorOption.text = colorNames[colorIndex];
        tankOption.text = playerInfo.tankPrefab.ToString().SplitPascalCase();
        controlOption.text = playerInfo.control.ToString().SplitPascalCase();
    }

}
