using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class GenerateMap : MonoBehaviour
    {
        [SerializeField] private bool generateNewMap;
        [SerializeField] private bool drawDebugLines = true;

        [Min(0.1f)]
        [SerializeField] private float pointsPerWidth = 3;
        [Range(0.0f, 5.0f)]
        [SerializeField] private float amplitude = 1.75f;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float smoothFactor = 0.9f;

        [SerializeField] private float width = 20f;
        [Range(5.0f, 25.0f)]
        [SerializeField] private float height = 6f;
        [Range(1.0f, 10.0f)]
        [SerializeField] private float depth = 3f;
        [Min(1)]
        [SerializeField] private int tileAmount = 1;

        private float[] heights;
        private Vector3[] linePositions;

        void Start()
        {
            GenerateRandomMap();
        }

        private void Update()
        {
            if (generateNewMap)
            {
                foreach (Transform child in transform)
                    Destroy(child.gameObject);
                GenerateRandomMap();
                generateNewMap = false;
            }
                
            if(drawDebugLines)
                DrawDebugLine(linePositions);
        }

        private void GenerateRandomMap()
        {
            if (height < amplitude * 2)
                height = amplitude * 2;
            GenerateRandomHeights();
            SmoothenHeights();
            UpdatePositions();
            GenerateMesh();
            GenerateEnvironmentalObjects();
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
            Debug.DrawLine(new Vector3(-width / 2.0f, 0) + transform.position, new Vector3(width / 2.0f - 1.0f / pointsPerWidth, 0) + transform.position, Color.red);
        }

        private float GetRandomHeight() => Random.value * 2.0f - 1.0f;

        private void GenerateRandomHeights()
        {
            heights = new float[(int)(pointsPerWidth * width)];
            for (int i = 0; i < heights.Length; i++)
                heights[i] = GetRandomHeight();
        }

        private void UpdatePositions()
        {
            float widthPerPoint = 1.0f / pointsPerWidth;
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
            List<Vector3> vertices = new List<Vector3>();
            List<Quad> quads = new List<Quad>();
            List<int> triangleIndices = new List<int>();

            for (int i = 0; i < linePositions.Length; i++)
            {
                int vertexIndex = AddVertex(linePositions[i]);
                int verticesPerPosition = 3;
                if (i < 1) continue;
                int pastIndex = vertexIndex - verticesPerPosition;
                quads.Add(new Quad(pastIndex, pastIndex + 1, vertexIndex, vertexIndex + 1));
                quads.Add(new Quad(pastIndex + 2, pastIndex, vertexIndex + 2, vertexIndex));
            }

            foreach (Quad quad in quads)
                foreach (int index in quad.GetTriangleIndices())
                    triangleIndices.Add(index);

            CreateMesh(vertices, triangleIndices);

            int AddVertex(Vector3 vertex)
            {
                int index = vertices.Count;
                vertices.Add(vertex);
                vertices.Add(new Vector3(vertex.x, 0));
                vertices.Add(new Vector3(vertex.x, vertex.y, depth));
                return index;
            }
        }

        private void CreateMesh(List<Vector3> vertices, List<int> triangleIndices)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.triangles = triangleIndices.ToArray();
            mesh.RecalculateNormals();
            mesh.uv = CreateUV(vertices);

            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshCollider collider = gameObject.GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        private void GenerateEnvironmentalObjects()  
        {
            GetComponent<GenerateDecor>().GenerateObjects(linePositions, width, depth);
        }
        
        private Vector2[] CreateUV(List<Vector3> vertices)
        {   //Unfolds the map to determine UV-coord
            Vector2[] uvs = new Vector2[vertices.Count];
            int divideMapSize = 2;
            for (int i = 0; i < vertices.Count; i++)
            {   
                float vertexHeight = vertices[i].y + vertices[i].z;//both y and z are counted as height.
                float fillPercentX = Mathf.InverseLerp(width / divideMapSize, -width / divideMapSize, vertices[i].x);
                float fillPercentY = Mathf.InverseLerp((height + depth) / divideMapSize, -(height + depth) / divideMapSize, vertexHeight);
                uvs[i] = new Vector2(fillPercentX, fillPercentY) * tileAmount;
            }

            return uvs;
        }
    }

    public struct Quad
    {
        private readonly int upperLeftIndex, lowerLeftIndex, upperRightIndex, lowerRightIndex;
        public Quad(int upperLeftIndex, int lowerLeftIndex, int upperRightIndex, int lowerRightIndex)
        {
            this.upperLeftIndex = upperLeftIndex;
            this.lowerLeftIndex = lowerLeftIndex;
            this.upperRightIndex = upperRightIndex;
            this.lowerRightIndex = lowerRightIndex;
        }

        public int[] GetTriangleIndices()
        {
            int[] indices = { lowerLeftIndex, upperLeftIndex, upperRightIndex,
                              lowerLeftIndex, upperRightIndex, lowerRightIndex};
            return indices;
        }
    }
}

