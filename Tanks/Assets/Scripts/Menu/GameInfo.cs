using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public List<PlayerInfo> info;

    [SerializeField] public List<string> names;
    [SerializeField] public List<Color> colors;
    [SerializeField] public List<Prefab> tankPrefabs;
    [SerializeField] public List<Control> controls;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SaveInfo()
    {
        foreach (PlayerInfo tankCustomization in info)
        {
            if (tankCustomization.transform.GetChild(1).gameObject.activeInHierarchy)
            {
                names.Add(tankCustomization.name);
                colors.Add(tankCustomization.color);
                tankPrefabs.Add(tankCustomization.tankPrefab);
                controls.Add(tankCustomization.control);
            }
        }
    }
}
