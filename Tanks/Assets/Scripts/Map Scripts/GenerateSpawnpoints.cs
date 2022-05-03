using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class GenerateSpawnpoints : ObjectGenerator
    {
        [SerializeField] GameObject spawnPointPrefab;
        //[Range(2, 4)]
        //[SerializeField] int numberOfTanks;

        public override void GenerateObjects(Vector3[] linePositions, float width, float depth)
        {
            base.GenerateObjects(linePositions, width, depth);
            
        }

        public void GenerateTanks(int numberOfTanks, GameObject tank)
        {
            for (int i = 0; i < numberOfTanks; i++)
                InstantiateAtPosition(GetRandomSpawnPosition(), tank);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            int attemptsBeforeExit = 10;
            Vector3 randomizedVector = Vector3.zero;
            while (attemptsBeforeExit > 0)
            {
                randomizedVector = linePositions[RandomInt(linePositions.Length)];
                randomizedVector.z = depth / 2.0f;
                randomizedVector.y += heightAdjustment;

                if (ValidDistanceFromObjects(randomizedVector))
                    break;

                --attemptsBeforeExit;
            }
            usedPositions.Add(randomizedVector);
            return randomizedVector;
        }

        private void InstantiateAtPosition(Vector3 position, GameObject tank) => CreateObjectAtPosition(position, tank);
    }
}

