using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] public Item item;
    [SerializeField] private bool selected;

    private int amount;

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
        if (item != null)
        {
            if (item.amount != amount)
            {
                amount = item.amount;
                amountText.text = amount == 0 ? "" : amount.ToString();
            }
        }
            
        else if (amount != 0)
            Clear();
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

    public void Clear()
    {
        item = null;
        amount = 0;
        amountText.text = "";
    }

}
