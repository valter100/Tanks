using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    [SerializeField] protected new string name;
    [SerializeField] protected float startAmount;
    [SerializeField] protected float amountRange;

    [TextArea(0, 4)]
    [SerializeField] protected string description;

    public string Name => name;
    public string Description => description;

    /// <summary>
    /// Uses this Usable.
    /// </summary>
    public abstract void Use();

    public int GetStartAmount()
    {
        return (int)Random.Range(startAmount - amountRange, startAmount + amountRange + 1);
    }
}
