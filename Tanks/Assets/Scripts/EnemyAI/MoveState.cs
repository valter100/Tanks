using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    private Transform oldPosition;

    private LayerMask groundLayer;
    public override void DoState(AiManager thisManager,Transform enemyTankPosition, AiTank thisTank)
    {
        if (!oldPosition) oldPosition = thisTank.transform;


        if (thisManager.GetDistanceToEnemy() < 15 || thisManager.GetFuel() <= 0 || oldPosition == thisTank.transform)
        {
            thisManager.SetFireState();
            return;
        }

        oldPosition = thisTank.transform;

        DoMove(enemyTankPosition, thisTank);

    }

    private void DoMove(Transform enemyTankPosition, AiTank thisTank)
    {
        thisTank.Move(enemyTankPosition);
    }
}
