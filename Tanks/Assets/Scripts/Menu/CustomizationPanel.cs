using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomizationPanel : MonoBehaviour
{
    [SerializeField] public TankCustomization tankCustomization;
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
        tankCustomization.color = colors[colorIndex];
        UpdateOptionTexts();
        transform.GetComponentInParent<PlayMenu>().customizationPanels.Add(this);
    }

    public void Update()
    {
        tankCustomization.name = nametag.text;
    }

    public void OnClick_PreviousColor()
    {
        if (--colorIndex == -1)
            colorIndex = colors.Length - 1;

        tankCustomization.color = colors[colorIndex];
        UpdateOptionTexts();
    }

    public void OnClick_NextColor()
    {
        if (++colorIndex == colors.Length)
            colorIndex = 0;

        tankCustomization.color = colors[colorIndex];
        UpdateOptionTexts();
    }

    public void OnClick_PreviousTank()
    {
        tankCustomization.tankType.SetPrevious();
        UpdateOptionTexts();
    }

    public void OnClick_NextTank()
    {
        tankCustomization.tankType.SetNext();
        UpdateOptionTexts();
    }

    public void OnClick_PreviousControl()
    {
        tankCustomization.control.SetPrevious();
        UpdateOptionTexts();
    }

    public void OnClick_NextControl()
    {
        tankCustomization.control.SetNext();
        UpdateOptionTexts();
    }

    public void UpdateOptionTexts()
    {
        colorOption.text = colorNames[colorIndex];
        tankOption.text = tankCustomization.tankType.ToString().SplitPascalCase();
        controlOption.text = tankCustomization.control.ToString().SplitPascalCase();
    }

}
