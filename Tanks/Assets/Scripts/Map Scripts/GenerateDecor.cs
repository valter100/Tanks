using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tanks
{
    public class GenerateDecor : MonoBehaviour
    {
        [SerializeReference] GameObject[] objects;
        [SerializeReference] float objectsPerWidth;
        [SerializeReference] float minimumObjectDistance;

        [SerializeReference] float depthAdjustment;
        [SerializeReference] float heightAdjustment;

        private Vector3[] linePositions;
        private float depth;

        private List<Vector3> usedPositions;

        public void GenerateObjects(Vector3[] linePositions, float width, float depth)
        {
            this.linePositions = linePositions;
            this.depth = depth;
            usedPositions = new List<Vector3>();

            int numberOfObjects = (int)(width * objectsPerWidth);
            for (int i = 0; i < numberOfObjects; i++)
                CreateObjectAtPosition(RandomizePosition(), PickRandomObject());
        }

        private Vector3 RandomizePosition()
        {
            int attemptsBeforeExit = 10;
            float zDepthAdjustment = depthAdjustment * (Random.value + 0.25f);
            Vector3 randomizedVector = Vector3.zero;
            while (attemptsBeforeExit > 0)
            {
                bool noConflicts = true;
                randomizedVector = linePositions[RandomInt(linePositions.Length)];
                randomizedVector.z = RandomBool() ? depth - zDepthAdjustment : zDepthAdjustment;
                randomizedVector.y += heightAdjustment;

                
                foreach (Vector3 position in usedPositions)
                    if ((position - randomizedVector).magnitude <= minimumObjectDistance)
                        noConflicts = false;

                if (noConflicts)
                    break;

                --attemptsBeforeExit;
            }

            usedPositions.Add(randomizedVector);
            return randomizedVector;
        }

        private GameObject PickRandomObject() => objects[RandomInt(objects.Length)];

        private int RandomInt(int max) => (int)(Random.value * max);

        private bool RandomBool() => Random.value > 0.5f;

        private void CreateObjectAtPosition(Vector3 position, GameObject gameObject)
        {
            gameObject.transform.position = position;
            Instantiate(gameObject, transform, true);
        }
    }
}

