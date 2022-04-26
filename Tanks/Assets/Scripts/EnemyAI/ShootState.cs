using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState :  ActiveState
{
    public override ActiveState DoState(AiManager thisBehaviour, Transform thisTankPosition, Transform enemyTankPosition, Tank thisTank)
    {
        if (thisBehaviour.GetDistanceToEnemy() >= thisBehaviour.GetMaxShootingRange() && thisBehaviour.GetFuel() > 0)
        {
            return thisBehaviour.moveState;
        }

        DoShoot();

        return null;
    }

    private void DoShoot()
    {

    }
}
