using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Tank tank;

    [SerializeField] private string[] colorNames;
    [SerializeField] private Color[] colors;
    [SerializeField] GameObject[] tanks;

    private void Start()
    {
        nametag.text = "";
        colorIndex = Random.Range(0, colors.Length - 1);
        transform.GetComponentInParent<PlayMenu>().customizationPanels.Add(this);
        tank = playerInfo.gameObject.transform.GetComponentInChildren<Tank>();

        UpdateTankColor();
        UpdateOptionTexts();
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

    public void OnClick_RemovePlayer()
    {
        transform.GetComponentInParent<PlayMenu>().customizationPanels.Remove(this);
        PlayerSpot playerSpot = tank.transform.parent.GetComponent<PlayerSpot>();
        playerSpot.gameObject.SetActive(true);
        playerSpot.Start();
        Destroy(gameObject);
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
        tank.SetColor(playerInfo.color);
    }

    private void UpdateTankType()
    {
        PlayerSpot playerSpot = tank.transform.parent.GetComponent<PlayerSpot>();
        Destroy(tank.gameObject);
        tank = Instantiate(Prefabs.Tanks[playerInfo.tankPrefab], playerSpot.transform).GetComponent<Tank>();
        tank.gameObject.transform.localScale = Vector3.one;
        UpdateTankColor();
    }

}
