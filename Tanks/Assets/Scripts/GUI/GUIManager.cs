using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public Inventory inventory { get; private set; }
    public Hotbar hotbar { get; private set; }

    void Start()
    {
        inventory = transform.Find("Inventory").GetComponent<Inventory>();
        hotbar = transform.Find("Hotbar").GetComponent<Hotbar>();
    }

}
