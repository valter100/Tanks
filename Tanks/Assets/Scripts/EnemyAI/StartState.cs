using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : State
{
    public override void DoState(AiManager thisBehaviour, Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() >= 1 && thisBehaviour.GetFuel() >= 1f)
        {
           thisBehaviour.SetMoveState();
           return;
        }
        else
        {
           thisBehaviour.SetFireState();
           return;
        }
    }
}
