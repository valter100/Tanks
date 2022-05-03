using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    private LayerMask groundLayer;
    public override void DoState(AiManager thisManager, Transform thisTankPosition,Transform enemyTankPosition, Tank thisTank)
    {
        if (thisManager.GetDistanceToEnemy() < 1 || thisManager.GetFuel() <= 0)
        {
            thisManager.SetFireState();
            return;
        }

        DoMove(enemyTankPosition, thisTank, thisManager, thisTankPosition);
    }

    public void Initilize()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void DoMove(Transform enemyTankPosition, Tank thisTank, AiManager thisManager, Transform thisTankPosition)
    {
        groundLayer = LayerMask.GetMask("Ground");

        if (enemyTankPosition.position.x > thisTankPosition.position.x)
            thisTankPosition.rotation = Quaternion.Euler(0, 0, 0);
        else if (enemyTankPosition.position.x < thisTankPosition.position.x)
            thisTankPosition.rotation = Quaternion.Euler(0, -180, 0);

        Vector3 localDirection = thisTankPosition.InverseTransformDirection(Vector3.right);

        RaycastHit hit;
        if (Physics.Raycast(thisTankPosition.position + new Vector3(0, 1, 0) * 0.66f + localDirection, Vector3.down, out hit, 2, groundLayer))
        {
            //Debug.DrawLine(transform.position + new Vector3(0,1,0) * 0.66f + localDirection, hit.point);
            //Debug.Log(hit.normal.y);
            if (hit.normal.y < 0.85f)
                return;
        }

        if (enemyTankPosition.position.x > thisManager.transform.position.x)
            thisTankPosition.position += Vector3.right * thisTank.GetMovementSpeed();
        else if (enemyTankPosition.position.x < thisManager.transform.position.x)
            thisTankPosition.position -= Vector3.right * thisTank.GetMovementSpeed();
        
        //rb.AddForce(playerController.GetMovement() * movementSpeed);
        thisTank.SetTimeSinceLastEffect(Time.deltaTime);
        if (thisTank.GetTimeSinceLastEffect() > thisTank.GetTimeBetweenEffectSpawn())
        {
            Instantiate(thisTank.GetMovementEffect(), thisTankPosition.position, Quaternion.identity);
            thisTank.SetTimeSinceLastEffect(-thisTank.GetTimeSinceLastEffect());
        }

        if (!thisTank.GetIsSlowed())
            thisTank.LowerFuelPercentage(2); //Value must be changed
        else
            thisTank.LowerFuelPercentage(2); //Value must be changed

        thisTank.UpdateFuelSlider();
    }
}
