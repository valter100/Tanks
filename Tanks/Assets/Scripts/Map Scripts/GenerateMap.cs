using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class GenerateMap : MonoBehaviour
    {
        [Range(3, 100)]
        [SerializeField] private int numberOfPoints = 50;
        [Range(0.0f, 3.0f)]
        [SerializeField] private float amplitude = 1.75f;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float smoothFactor = 0.9f;

        [SerializeField] private float width = 20f;
        [Range(6.0f, 15.0f)]
        [SerializeField] private float height = 6f;

        private float[] heights;
        private Vector3[] linePositions;

        void Start()
        {
            GenerateRandomMap();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                GenerateRandomMap();

            DrawDebugLine(linePositions);
        }

        private void GenerateRandomMap()
        {
            GenerateRandomHeights();
            SmoothenHeights();
            UpdatePositions();
            GenerateMesh();
        }

        private void DrawDebugLine(Vector3[] positions)
        {
            if (positions.Length <= 1)
                return;

            for (int i = 1; i < positions.Length; i++)
                Debug.DrawLine(positions[i - 1] + transform.position, positions[i] , Color.green);

            //Height
            Debug.DrawLine(new Vector3(-width / 2.0f, 0) + transform.position, new Vector3(-width / 2.0f, height) + transform.position, Color.red);
            //Width
            Debug.DrawLine(new Vector3(-width / 2.0f, 0) + transform.position, new Vector3(width / 2.0f - width / numberOfPoints, 0) + transform.position, Color.red);
        }

        private float GetRandomHeight() => Random.value * 2.0f - 1.0f;

        private void GenerateRandomHeights()
        {
            heights = new float[numberOfPoints];
            for (int i = 0; i < heights.Length; i++)
                heights[i] = GetRandomHeight();
        }

        private void UpdatePositions()
        {
            float widthPerPoint = width / numberOfPoints;
            linePositions = new Vector3[heights.Length];
            for (int i = 0; i < heights.Length; i++)
                linePositions[i] = new Vector3(i * widthPerPoint - width / 2.0f, height - amplitude + heights[i] * amplitude);
        }

        private void SmoothenHeights()
        {
            for (int i = 1; i < heights.Length; i++)
                heights[i] = Mathf.Lerp(heights[i], heights[i - 1], smoothFactor);
        }
    
        private void GenerateMesh()
        {
            List<Vector3> vertecies = new List<Vector3>();
            List<int> triangleIndexes = new List<int>();
            foreach (Vector3 position in linePositions)
            {
                vertecies.Add(position);
                vertecies.Add(new Vector3(position.x, 0));
            }

            for (int i = 0; i < vertecies.Count - 2; i++)
            {
                triangleIndexes.Add(i);
                triangleIndexes.Add(i + 1);
                triangleIndexes.Add(i + 2);

                triangleIndexes.Add(i + 2);
                triangleIndexes.Add(i + 1);
                triangleIndexes.Add(i);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertecies);
            mesh.triangles = triangleIndexes.ToArray();
            mesh.RecalculateNormals();
            
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }
    }

    public struct MeshPoint
    {
        public Vector3 position;
        public int index;
        public MeshPoint(Vector3 position, int index)
        {
            this.position = position;
            this.index = index;
        }
    }
}


