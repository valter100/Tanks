using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankType
{
    TankOne,
    TankTwo,
    TankThree,
    TankFour
}

public enum Control
{
    Player,
    Bot
}

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] public new string name;
    [SerializeField] public Color color;
    [SerializeField] public TankType tankType;
    [SerializeField] public Control control;
}
