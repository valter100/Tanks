using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayerButton : MonoBehaviour
{
    [SerializeField] private GameObject customizationPanelPrefab;
    [SerializeField] public GameObject playerSpot;

    public void OnClick_AddPlayer()
    {
        GameObject playMenu = GameObject.Find("Canvas").transform.Find("Play Menu").gameObject;
        GameObject customizationPanel = Instantiate(customizationPanelPrefab, playMenu.transform);
        customizationPanel.GetComponent<FollowInWorldObject>().followObject = playerSpot.transform;
        customizationPanel.GetComponent<CustomizationPanel>().tankCustomization = playerSpot.GetComponent<PlayerInfo>();

        playerSpot.SetActive(true);
        gameObject.SetActive(false);
    }
}
