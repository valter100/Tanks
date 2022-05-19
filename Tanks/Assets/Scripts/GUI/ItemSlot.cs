using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] public Item item;
    [SerializeField] private GameObject displayObject;
    [SerializeField] private MeshRenderer renderObj;

    private int amount;

    private Inventory inventory;
    private TextMeshProUGUI amountText;
    private GameObject frame;

    public bool Selected => frame.activeSelf;

    void Start()
    {
        inventory = transform.parent.parent.GetComponent<Inventory>();
        amountText = transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        frame = transform.Find("Frame").gameObject;

        amountText.text = "";
    }

    void Update()
    {
        if (item != null)
        {
            if (item.amount != amount)
            {
                if (item.amount <= 0)
                    SetItem(null);

                else
                {
                    amount = item.amount;
                    amountText.text = amount == 0 ? "" : amount.ToString();
                }
            }
        }
            
        else if (amount != 0)
            SetItem(null);
    }

    public void OnClick_Select()
    {
        if (item != null)
            inventory.ItemSlotClicked(this);
    }

    public void Select()
    {
        frame.SetActive(true);
    }

    public void Deselect()
    {
        frame.SetActive(false);
    }

    public void SetItem(Item item)
    {
        this.item = item;

        if (displayObject != null)
            Destroy(displayObject);

        if (item != null)
        {
            displayObject = Instantiate(item.usable.gameObject, transform);
            amount = item.amount;
            amountText.text = amount == 0 ? "" : amount.ToString();
        }

        else
        {
            amount = 0;
            amountText.text = "";
        }
    }

}
