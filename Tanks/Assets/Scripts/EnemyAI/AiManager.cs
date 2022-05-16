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

    AiTank thisTank;

    public Transform activeTargetPosition;
    List<GameObject> targetTanks;

    private float distanceToEnemy;
    private float maxShootingRange;
    private float fuel;

    [SerializeField] private bool lowHealthTarget, highHealthTarget, closestTarget, furthestTarget;

    void Start()
    {
        activeState = startState;
        targetTanks = new List<GameObject>();
        thisTank = GetComponent<AiTank>();
        maxShootingRange = thisTank.GetMaxShootForce();

        activeTargetPosition = null;

        UpdateTargetsList();
    }

    public void ManualUpdate()
    {
        if (!activeTargetPosition)
            ChooseTarget();

        distanceToEnemy = Vector3.Distance(gameObject.transform.position, activeTargetPosition.position);
        fuel = thisTank.GetFuelPercentage();
       
        activeState.DoState(this, activeTargetPosition, thisTank);
    }

    public float GetFuel() => fuel;
    public float GetDistanceToEnemy() => distanceToEnemy;
    public float GetMaxShootingRange() => maxShootingRange;

    public State GetActiveState() => activeState;
    public void SetFireState() => activeState = fireState;
    public void SetMoveState() => activeState = moveState;

    public void SetStateToStartState() => activeState = startState;

    public void ChooseTarget()
    {
        UpdateTargetsList();

        if (lowHealthTarget)
        {
            float health = 100f;

            foreach (GameObject tank in targetTanks)
            {
                if (tank.GetComponent<Tank>().GetHealthPercentage() < health)
                {
                    activeTargetPosition = tank.transform;
                    health = tank.GetComponent<Tank>().GetHealthPercentage();
                }
            }
        }
        else if (highHealthTarget)
        {
            float health = 1f;

            foreach (GameObject tank in targetTanks)
            {
                if (tank.GetComponent<Tank>().GetHealthPercentage() > health)
                {
                    activeTargetPosition = tank.transform;
                    health = tank.GetComponent<Tank>().GetHealthPercentage();
                }
            }
        }
        else if (closestTarget)
        {
            float distance = 100f;

            foreach (GameObject tank in targetTanks)
            {
                if (Vector3.Distance(tank.transform.position, gameObject.transform.position) < distance)
                {
                    activeTargetPosition = tank.transform;
                    distance = Vector3.Distance(tank.transform.position, gameObject.transform.position);
                }
            }
        }
        else if (furthestTarget)
        {
            float distance = 0f;
            foreach (GameObject tank in targetTanks)
            {
                if (Vector3.Distance(tank.transform.position, gameObject.transform.position) > distance)
                {
                    activeTargetPosition = tank.transform;
                    distance = Vector3.Distance(tank.transform.position, gameObject.transform.position);
                }
            }
        }
        else
        {
            int random = Random.Range(0, targetTanks.Count);
            activeTargetPosition = targetTanks[random].transform;
        }

        //Debug.Log(activeTargetPosition.position);

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

        foreach (GameObject tank in tempArray)
        {
            if (gameObject == tank) continue;

            targetTanks.Add(tank);
        }

        //Debug.Log(targetTanks.Count);
    }
}
