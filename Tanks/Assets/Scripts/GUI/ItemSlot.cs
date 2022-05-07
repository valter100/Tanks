using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] public GameObject itemPrefab;
    [SerializeField] private bool selected;

    private Inventory inventory;
    private TextMeshProUGUI amountText;
    private GameObject frame;

    void Start()
    {
        inventory = transform.parent.parent.GetComponent<Inventory>();
        amountText = transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        frame = transform.Find("Frame").gameObject;

        amountText.text = "";
        Deselect();
    }

    void Update()
    {
        
    }

    public void OnClick()
    {
        inventory.ItemSlotClicked(this);
    }

    public void Select()
    {
        selected = true;
        frame.SetActive(true);
    }

    public void Deselect()
    {
        selected = false;
        frame.SetActive(false);
    }

    public void UpdateAmount(int newAmount)
    {
        amountText.text = newAmount == 0 ? "" : newAmount.ToString();
    }
}
