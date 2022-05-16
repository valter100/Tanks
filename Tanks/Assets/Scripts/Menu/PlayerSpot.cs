using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpot : MonoBehaviour
{
    [SerializeField] private GameObject addPlayerButtonPrefab;
    [SerializeField] private bool startWithTank;

    public bool StartWithTank => startWithTank;

    public void Start()
    {
        GameObject playMenu = GameObject.Find("Canvas").transform.Find("Play Menu").gameObject;
        AddPlayerButton addPlayerButton = Instantiate(addPlayerButtonPrefab, playMenu.transform).GetComponent<AddPlayerButton>();
        addPlayerButton.playerSpot = gameObject;
        addPlayerButton.GetComponent<FollowInWorldObject>().followObject = transform;
        gameObject.SetActive(false);
    }
}
