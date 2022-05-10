using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] public new string name;
    [SerializeField] public Color color;
    [SerializeField] public Prefab tankPrefab;
    [SerializeField] public Control control;
}

public enum Control
{
    Player,
    Bot
}
