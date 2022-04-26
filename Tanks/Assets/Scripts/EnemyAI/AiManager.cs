using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiManager : MonoBehaviour
{
    private ActiveState activeState;
    public StartState startState = new StartState();
    public MoveState moveState = new MoveState();
    public ShootState shootState = new ShootState();

    [SerializeField]
    Tank thisTank;

    Transform thisPosition;
    Transform targetPosition;

    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    void Start()
    {
        activeState = startState;
        thisPosition = null;
        targetPosition = null;
    }

    
    private void Update()
    {
        distanceToEnemy = Vector3.Distance(thisPosition.position, targetPosition.position);
        fuel = thisTank.GetFuelPercentage();
        maxShootingRange = thisTank.GetMaxShootForce();

        activeState = activeState.DoState(this, thisPosition, targetPosition, thisTank);
    }

    public float GetFuel() => fuel;
    public float GetDistanceToEnemy() => distanceToEnemy;
    public float GetMaxShootingRange() => maxShootingRange;
    
        
    
}
