using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Menu mainMenu, playMenu, settingsMenu, statisticsMenu, howToPlayMenu;
    [SerializeField] Vector3 cameraPosition;

    private Dictionary<Menu.Type, Menu> menus;

    private void Start()
    {
        Camera.main.transform.localPosition = cameraPosition;

        menus = new Dictionary<Menu.Type, Menu>()
        {
            { Menu.Type.Main, mainMenu },
            { Menu.Type.Play, playMenu },
            { Menu.Type.Settings, settingsMenu },
            { Menu.Type.Statistics, statisticsMenu },
            { Menu.Type.HowToPlay, howToPlayMenu }
        };

        foreach (KeyValuePair<Menu.Type, Menu> menu in menus)
            menu.Value.gameObject.SetActive(false);

        mainMenu.gameObject.SetActive(true);
    }

    public void SwapMenu(Menu callerMenu, Menu.Type menu)
    {
        callerMenu.gameObject.SetActive(false);
        menus[menu].gameObject.SetActive(true);
    }

}