using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDecor : ObjectGenerator
{
    [SerializeField] GameObject[] smallObjects;
    [SerializeField] GameObject[] largeObjects;
    [SerializeField] float objectsPerWidth;
    [Range(0f, 1f)]
    [SerializeField] float largeObjectPercentage;

    [SerializeField] float depthAdjustment;

    public override void GenerateObjects(Vector3[] linePositions, float width, float depth)
    {
        base.GenerateObjects(linePositions, width, depth);

        int numberOfObjects = (int)(width * objectsPerWidth);
        bool getLargeObject;
        for (int i = 0; i < numberOfObjects; i++)
        {
            getLargeObject = Random.value < largeObjectPercentage;
            CreateObjectAtPosition(RandomizePosition(getLargeObject), PickRandomObject(getLargeObject), RandomAngle());//
        }  
    }

    private Vector3 RandomizePosition(bool objectIsLarge)
    {
        int attemptsBeforeExit = 10;
        float zDepthAdjustment = depthAdjustment * (Random.value + 0.25f);
        Vector3 randomizedVector = Vector3.zero;
        while (attemptsBeforeExit-- > 0)
        {
            randomizedVector = linePositions[RandomInt(linePositions.Length)];
            if (objectIsLarge)
                randomizedVector.z = RandomBool() ? depth - zDepthAdjustment : zDepthAdjustment;
            else
                randomizedVector.z = Random.Range(0, depth);
            randomizedVector.y += heightAdjustment;

            if (ValidDistanceFromObjects(randomizedVector))
                break;
        }

        usedPositions.Add(randomizedVector);
        return randomizedVector + transform.position;
    }

    private GameObject PickRandomObject(bool getLargeObject)
    {
        if (getLargeObject)
            return largeObjects[RandomInt(largeObjects.Length)];
        return smallObjects[RandomInt(smallObjects.Length)];
    }
}


