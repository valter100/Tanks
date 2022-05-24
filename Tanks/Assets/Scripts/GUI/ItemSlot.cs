using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private Item item;
    [SerializeField] private GameObject icon;
    [SerializeField] private float iconScale;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject frame;

    private int amount;

    public bool Selected => frame.activeSelf;

    public Item Item => item;

    public void ManualStart()
    {
        amountText.text = "";
    }

    private void Update()
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

            if (icon != null)
                icon.transform.rotation = inventory.IconRotation;
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

        if (icon != null)
        {
            Destroy(icon);
            icon = null;
        }
        
        if (item != null)
        {
            icon = Instantiate(item.usable.gameObject, transform);

            float scale = icon.GetComponent<Usable>().IconScale * iconScale;
            icon.transform.localScale = new Vector3(scale, scale, scale);

            Destroy(icon.GetComponent<Usable>());
            Destroy(icon.GetComponent<AttackPattern>());
            Destroy(icon.GetComponent<Explosion>());
            Destroy(icon.GetComponent<Rigidbody>());

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
