using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class GenerateDecor : ObjectGenerator
    {
        [SerializeField] GameObject[] objects;
        [SerializeField] float objectsPerWidth;

        [SerializeField] float depthAdjustment;

        public override void GenerateObjects(Vector3[] linePositions, float width, float depth)
        {
            base.GenerateObjects(linePositions, width, depth);

            int numberOfObjects = (int)(width * objectsPerWidth);
            for (int i = 0; i < numberOfObjects; i++)
                CreateObjectAtPosition(RandomizePosition(), PickRandomObject(), RandomAngle());
        }

        private Vector3 RandomizePosition()
        {
            int attemptsBeforeExit = 10;
            float zDepthAdjustment = depthAdjustment * (Random.value + 0.25f);
            Vector3 randomizedVector = Vector3.zero;
            while (attemptsBeforeExit > 0)
            {
                randomizedVector = linePositions[RandomInt(linePositions.Length)];
                randomizedVector.z = RandomBool() ? depth - zDepthAdjustment : zDepthAdjustment;
                randomizedVector.y += heightAdjustment;

                if (ValidDistanceFromObjects(randomizedVector))
                    break;

                --attemptsBeforeExit;
            }

            usedPositions.Add(randomizedVector);
            return randomizedVector + transform.position;
        }

        private GameObject PickRandomObject() => objects[RandomInt(objects.Length)];
    }


