using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    [SerializeField] protected new string name;

    [TextArea(0, 4)]
    [SerializeField] protected string description;

    public string Name => name;
    public string Description => description;

    /// <summary>
    /// Uses this Usable.
    /// </summary>
    public abstract void Use();
}
