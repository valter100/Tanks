using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireState :  State
{
    public override void DoState(AiManager thisBehaviour, Transform thisTankPosition, Transform enemyTankPosition, AiTank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() >= thisBehaviour.GetMaxShootingRange() && thisBehaviour.GetFuel() > 0)
        {
           thisBehaviour.SetMoveState();
            return;
        }

        DoShoot();
    }

    private void DoShoot()
    {
        //If we are in fireing state, calculate the correct aim to hit target, then randomize a bias and ad to aim to make sure not every shot is a direct hit.
    }
}
