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
    [SerializeField] private Prefab[] usableKeys;
    [SerializeField] private GameObject[] usableValues;

    public static Dictionary<Prefab, GameObject> All { get; private set; }
    public static Dictionary<Prefab, GameObject> Tanks { get; private set; }
    public static Dictionary<Prefab, GameObject> Usables { get; private set; }

    private void Start()
    {
        All = new Dictionary<Prefab, GameObject>();
        Tanks = new Dictionary<Prefab, GameObject>();
        Usables = new Dictionary<Prefab, GameObject>();

        FillDictionary(All, tankKeys, tankValues);
        FillDictionary(All, usableKeys, usableValues);

        FillDictionary(Tanks, tankKeys, tankValues);
        FillDictionary(Usables, usableKeys, usableValues);
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

    // Usables > Projectiles

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
