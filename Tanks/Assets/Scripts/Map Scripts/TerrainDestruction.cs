using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class TerrainDestruction : MonoBehaviour
    {
        [Range(1, 8)]
        [SerializeField] int numberOfDivisions = 2;
        [SerializeField] 
        public void DestroyTerrain(Vector3 explosionOrigin, float explosionRadius)
        {
            //TODO byt ut metod till att kolla om linjer korsar cirkeln, via cirkelns ekvation.
            //TODO Lägg även till nya punkter längs cirkelbågen.
            GenerateMap mapGenerator = gameObject.GetComponent<GenerateMap>();
            List<Vector3> newLinePositions = new List<Vector3>();
            List<Vector3> linePositions = new List<Vector3>();
            linePositions.AddRange(mapGenerator.LinePositions);
            Vector3 origin = explosionOrigin - transform.position;
            origin.z = 0;

            Debug.Log("Origin: " + origin);

            for (int i = 0; i < linePositions.Count; i++)
            {
                if (!IsInExplosion(i))
                {
                    newLinePositions.Add(linePositions[i]);
                    continue;
                }
                int previousIndex = i - 1;
                int nextIndex = i + 1;

                if (IsWithinBounds(previousIndex))
                    newLinePositions.AddRange(GetProjectedPositions(origin, explosionRadius, linePositions[previousIndex], linePositions[i]));

                if (IsWithinBounds(nextIndex))
                    newLinePositions.AddRange(GetProjectedPositions(origin, explosionRadius, linePositions[nextIndex], linePositions[i]));

                if (!IsWithinBounds(previousIndex) && !IsWithinBounds(nextIndex))
                    newLinePositions.Add(GetProjectedPosition(origin, explosionRadius, linePositions[i]));
                else
                    ++i;
            }

            Debug.Log("Previous line positions: " + linePositions.Count);
            Debug.Log("New line positions: " + newLinePositions.Count);

            mapGenerator.UpdateLinePositions(newLinePositions.ToArray());
            bool IsInExplosion(int i) => (linePositions[i] - origin).magnitude < explosionRadius;
            bool IsWithinBounds(int i) => i >= 0 && i < linePositions.Count;
        }

        private Vector3 GetProjectedPosition(Vector3 origin, float radius, Vector3 position)
        {
            float height = origin.y + Mathf.Sqrt(Mathf.Pow(radius, 2) + Mathf.Pow(origin.x, 2) + Mathf.Pow(position.x, 2) + position.x * origin.x * 2);
            return new Vector3(position.x, height, position.z);
        }

        private List<Vector3> GetProjectedPositions(Vector3 origin, float radius, Vector3 positionA, Vector3 positionB, int recursionDepth = 0)
        {
            List<Vector3> positions = new List<Vector3>();
            if (recursionDepth >= numberOfDivisions)
                return positions;

            Vector3 newPosition = GetProjection(origin, radius, positionA, positionB);

            positions.AddRange(GetProjectedPositions(origin, radius, positionA, newPosition, recursionDepth + 1));
            positions.Add(newPosition);
            positions.AddRange(GetProjectedPositions(origin, radius, newPosition, positionB, recursionDepth + 1));

            return positions;
        }

        private Vector3 GetProjection(Vector3 origin, float radius, Vector3 positionA, Vector3 positionB)
        {
            Vector3 vectorA = positionA - origin;
            Vector3 vectorB = positionB - origin;
            return (vectorA + vectorB).normalized * radius + new Vector3(origin.x, origin.y);
        }
    }
}

