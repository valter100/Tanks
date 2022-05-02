using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpot : MonoBehaviour
{
    [SerializeField] GameObject addPlayerButtonPrefab;

    void Start()
    {
        GameObject playMenu = GameObject.Find("Canvas").transform.Find("Play Menu").gameObject;
        GameObject addPlayerButton = Instantiate(addPlayerButtonPrefab, playMenu.transform);
        addPlayerButton.GetComponent<AddPlayerButton>().playerSpot = gameObject;
        addPlayerButton.GetComponent<FollowInWorldObject>().followObject = transform;

        gameObject.SetActive(false);
    }
}
