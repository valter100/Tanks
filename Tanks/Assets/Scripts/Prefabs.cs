using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    [Header("Tanks")]
    [SerializeField] private Prefab[] tankKeys;
    [SerializeField] private GameObject[] tankValues;

    [Header("Items")]
    [SerializeField] private Prefab[] itemKeys;
    [SerializeField] private GameObject[] itemValues;

    public static Dictionary<Prefab, GameObject> All { get; private set; }
    public static Dictionary<Prefab, GameObject> Tanks { get; private set; }
    public static Dictionary<Prefab, GameObject> Items { get; private set; }

    private void Start()
    {
        All = new Dictionary<Prefab, GameObject>();
        Tanks = new Dictionary<Prefab, GameObject>();
        Items = new Dictionary<Prefab, GameObject>();

        FillDictionary(All, tankKeys, tankValues);
        FillDictionary(All, itemKeys, itemValues);

        FillDictionary(Tanks, tankKeys, tankValues);
        FillDictionary(Items, itemKeys, itemValues);
    }

    private void FillDictionary(Dictionary<Prefab, GameObject> dictionary, Prefab[] keys, GameObject[] values)
    {
        for (int i = 0; i < keys.Length; ++i)
            dictionary.Add(keys[i], values[i]);
    }
}

public enum Prefab
{
    // Tanks

    TankOne,
    TankTwo,
    TankThree,

    // Items / Projectiles

    BounceBomb,
    Bullet,
    DropBomb,
    ExplodingShell,
    FreezeBullet,
    LargeExplodingShell,
    RollerBomb,
    ShockwaveBullet,
    SpreadBullet
}