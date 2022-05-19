using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSpawnpoints : ObjectGenerator
{
    [SerializeField] private int distanceFromEdge;
    [SerializeField] private bool ready;

    public bool Ready => ready;

    public void PrepareGeneration(Vector3[] linePositions, float width, float depth)
    {
        base.GenerateObjects(linePositions, width, depth);
        ready = true;
    }

    public Vector3 GetNewSpawnpoint()
    {
        if (!ready)
        {
            Debug.LogWarning("GenerateSpawnpoints script not ready.");
            return Vector3.zero;
        }

        int attemptsBeforeExit = 20;
        Vector3 randomizedVector = Vector3.zero;

        while (attemptsBeforeExit > 0)
        {
            randomizedVector = linePositions[RandomInt(linePositions.Length, distanceFromEdge)];
            randomizedVector.z = depth / 2.0f;
            randomizedVector.y += heightAdjustment;

            if (ValidDistanceFromObjects(randomizedVector))
                break;

            --attemptsBeforeExit;
        }

        usedPositions.Add(randomizedVector);
        return randomizedVector;
    }

}
