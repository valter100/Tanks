using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] public Item item;
    [SerializeField] private GameObject icon;
    [SerializeField] private GameObject emptyGameObjectPrefab;

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
        if (icon != null)
        {
            Destroy(icon);
            icon = null;
        }
            

        if (item != null)
        {
            icon = Instantiate(item.usable.gameObject, transform);
            Destroy(icon.GetComponent<Usable>());
            Destroy(icon.GetComponent<AttackPattern>());
            Destroy(icon.GetComponent<Explosion>());
            Destroy(icon.GetComponent<Rigidbody>());
            //MeshFilter meshFilter = icon.AddComponent<MeshFilter>();
            //meshFilter = item.usable.GetComponent<MeshFilter>();
            //MeshRenderer meshRenderer = icon.AddComponent<MeshRenderer>();
            //meshRenderer = item.usable.GetComponent<MeshRenderer>();

            icon.transform.localScale = new Vector3(63, 63, 63);


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
