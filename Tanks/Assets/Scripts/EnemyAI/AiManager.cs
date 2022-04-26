using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiManager : MonoBehaviour
{
    private ActiveState activeState;
    public MoveState moveState = new MoveState();
    public ShootState shootState = new ShootState();

    [SerializeField]
    Tank thisTank;

    Transform thisPosition;
    Transform targetPosition;

    

    void Start()
    {
        activeState = null;
        thisPosition = null;
        targetPosition = null;
        
    }

    
    void Update()
    {
        activeState = activeState.DoState(this, thisPosition, targetPosition, thisTank);
    }
}
