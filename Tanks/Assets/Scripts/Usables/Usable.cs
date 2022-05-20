using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    [Header("Usable")]
    [SerializeField] protected new string name;
    
    [TextArea(0, 4)]
    [SerializeField] protected string description;

    [SerializeField] protected float iconScale;
    [SerializeField] protected float startAmount;
    [SerializeField] protected float amountRange;

    /// <summary>
    /// The name of this Usable.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// The description of this Usable.
    /// </summary>
    public string Description => description;

    /// <summary>
    /// The scale of this Usable when placed as an icon in an ItemSlot.
    /// </summary>
    public float IconScale => iconScale;

    /// <summary>
    /// Uses this Usable.
    /// </summary>
    public abstract void Use();

    public int GetStartAmount()
    {
        return (int)Random.Range(startAmount - amountRange, startAmount + amountRange + 1);
    }
}
