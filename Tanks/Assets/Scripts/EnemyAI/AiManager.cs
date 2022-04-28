using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiManager : MonoBehaviour
{
    private State activeState;
    private StartState startState = new StartState();
    private MoveState moveState = new MoveState(); 
    private FireState fireState = new FireState();

    [SerializeField]
    Tank thisTank;
    [SerializeField]
    Transform thisPosition;

    List<Transform> targetPositions;
    Transform activeTargetPosition;

    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    void Start()
    {
        activeState = startState;
        activeTargetPosition = null;
        targetPositions = null;
    }

    private void Update()
    {
        distanceToEnemy = Vector3.Distance(thisPosition.position, activeTargetPosition.position);
        fuel = thisTank.GetFuelPercentage();
        maxShootingRange = thisTank.GetMaxShootForce();

        activeState.DoState(this, thisPosition, activeTargetPosition, thisTank);
    }

    public float GetFuel() => fuel;
    public float GetDistanceToEnemy() => distanceToEnemy;
    public float GetMaxShootingRange() => maxShootingRange;

    public State GetActiveState() => activeState;
    public void SetFireState() => activeState = fireState;
    public void SetMoveState() => activeState = moveState;

    public void ChooseTarget()
    {
        //choose a target from the list of targets. 
        //should we choose at random? (pick a random index from the list)
        //or should we pick the target that is closest or furthest away?
        //or pick the target with the least or most health?
        //maybe a mix of all options to create a ilusion of a smarter opponent?
    }
    
}
