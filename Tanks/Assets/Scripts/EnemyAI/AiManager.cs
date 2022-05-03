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
    
    Transform activeTargetPosition;
    List<GameObject> targetTanks;

    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    private bool lowHealthTarget, highHealthTarget, closestTarget, furthestTarget;

    void Start()
    {
        activeState = startState;
        targetTanks = new List<GameObject>();

        UpdateTargetsList();
    }

    private void Update()
    {
        ChooseTarget();

        distanceToEnemy = Vector3.Distance(gameObject.transform.position, activeTargetPosition.position);
        fuel = thisTank.GetFuelPercentage();
        maxShootingRange = thisTank.GetMaxShootForce();

        activeState.DoState(this, gameObject.transform, activeTargetPosition, thisTank);
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

        //if (lowHealthTarget)
        //{

        //}
        //else if (highHealthTarget)
        //{

        //}
        //else if (closestTarget)
        //{
        //    float distance = 100f;

        //    foreach(GameObject tank in targetTanks)
        //    {
        //        if (tank == gameObject) continue;

        //        if(Vector3.Distance(tank.transform.position, gameObject.transform.position) < distance)
        //        {
        //            activeTargetPosition = tank.transform;
        //            distance = Vector3.Distance(tank.transform.position, gameObject.transform.position);
        //        }
        //    }
        //}
        //else if (furthestTarget)
        //{
        //    float distance = 100f;
        //    foreach (GameObject tank in targetTanks)
        //    {
        //        if (tank == gameObject) continue;

        //        if (Vector3.Distance(tank.transform.position, gameObject.transform.position) > distance)
        //        {
        //            activeTargetPosition = tank.transform;
        //            distance = Vector3.Distance(tank.transform.position, gameObject.transform.position);
        //        }
        //    }
        //}
        //else
        //{
        //    int random = Random.Range(0, targetTanks.Count);

        //    if(gameObject == targetTanks[random]) 

        //    activeTargetPosition = targetTanks[random].transform;
        //}

        int random = Random.Range(0, targetTanks.Count);

        activeTargetPosition = targetTanks[random].transform;

        //choose a target from the list of targets. 
        //should we choose at random? (pick a random index from the list)
        //or should we pick the target that is closest or furthest away?
        //or pick the target with the least or most health?
        //maybe a mix of all options to create a ilusion of a smarter opponent?
    }

    public void UpdateTargetsList()
    {
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Tank");

        targetTanks.Clear();

        foreach(GameObject tank in tempArray)
        {
            if (gameObject == tank) continue;

            targetTanks.Add(tank);
        }

        Debug.Log(targetTanks.Count);
    }
}
