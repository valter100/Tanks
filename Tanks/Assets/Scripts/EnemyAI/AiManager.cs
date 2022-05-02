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
    GameObject thisGameObject;
    [SerializeField]
    Tank thisTank;
    [SerializeField]
    protected Transform thisTransform;

    List<Transform> targetPositions;
    Transform activeTargetPosition;
    GameObject[] targetTanks;

    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    private bool lowHealthTarget, highHealthTarget, closestTarget, furthestTarget;

    void Start()
    {
        activeState = startState;
        
        UpdateTargetsList();
    }

    private void Update()
    {
        distanceToEnemy = Vector3.Distance(thisTransform.position, activeTargetPosition.position);
        fuel = thisTank.GetFuelPercentage();
        maxShootingRange = thisTank.GetMaxShootForce();

        activeState.DoState(this, thisTransform, activeTargetPosition, thisTank);
    }

    public float GetFuel() => fuel;
    public float GetDistanceToEnemy() => distanceToEnemy;
    public float GetMaxShootingRange() => maxShootingRange;

    public State GetActiveState() => activeState;
    public void SetFireState() => activeState = fireState;
    public void SetMoveState() => activeState = moveState;

    public void ChooseTarget()
    {
        UpdateTargetsList();

        if (lowHealthTarget)
        {

        }
        else if (highHealthTarget)
        {

        }
        else if (closestTarget)
        {
            float distance = 100f;

            foreach(GameObject tank in targetTanks)
            {
                if (tank == thisGameObject) continue;

                if(Vector3.Distance(tank.transform.position, thisTransform.position) < distance)
                {
                    activeTargetPosition = tank.transform;
                    distance = Vector3.Distance(tank.transform.position, thisTransform.position);
                }
            }
        }
        else if (furthestTarget)
        {
            float distance = 100f;
            foreach (GameObject tank in targetTanks)
            {
                if (tank == thisGameObject) continue;

                if (Vector3.Distance(tank.transform.position, thisTransform.position) > distance)
                {
                    activeTargetPosition = tank.transform;
                    distance = Vector3.Distance(tank.transform.position, thisTransform.position);
                }
            }
        }
        else
        {
            int random = Random.Range(0, targetPositions.Count);

            activeTargetPosition = targetPositions[random];
        }

        

        //choose a target from the list of targets. 
        //should we choose at random? (pick a random index from the list)
        //or should we pick the target that is closest or furthest away?
        //or pick the target with the least or most health?
        //maybe a mix of all options to create a ilusion of a smarter opponent?
    }

    public void UpdateTargetsList()
    {
        targetPositions.Clear();

        targetTanks = GameObject.FindGameObjectsWithTag("Tank");

        foreach(GameObject tank in targetTanks)
        {
            if (tank == thisGameObject) continue;

            targetPositions.Add(tank.transform);
        }
    }
}
