using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public List<PlayerInfo> info;

    [SerializeField] public List<string> names;
    [SerializeField] public List<Color> colors;
    [SerializeField] public List<TankType> tankTypes;
    [SerializeField] public List<Control> controls;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        names = new List<string>();
        colors = new List<Color>();
        tankTypes = new List<TankType>();
        controls = new List<Control>();
    }

    public void SaveInfo()
    {
        foreach (PlayerInfo tankCustomization in info)
        {
            if (tankCustomization.transform.Find("Tank").gameObject.activeInHierarchy)
            {
                names.Add(tankCustomization.name);
                colors.Add(tankCustomization.color);
                tankTypes.Add(tankCustomization.tankType);
                controls.Add(tankCustomization.control);
            }
        }
    }
}
