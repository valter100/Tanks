using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanks
{
    public class GenerateMap : MonoBehaviour
    {
        [SerializeField] private bool generateNewMap;
        [SerializeField] private bool drawDebugLines = false;

        [Min(0.1f)]
        [SerializeField] private float pointsPerWidth = 3;
        [SerializeField] private float amplitude = 1.75f;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float smoothFactor = 0.9f;
        [SerializeField] private float width = 20f;
        [SerializeField] private float height = 6f;
        [SerializeField] private float depth = 3f;
        [SerializeField] private float bottomDepth = 1f;
        [SerializeField] private float overhangHeight;
        [SerializeField] private float slopeWidth;

        private float[] heights;
        public Vector3[] LinePositions { get; private set; }

        List<int> triangleIndices;
        List<Vector3> vertices;
        List<Quad> quads;
        void Start() => GenerateRandomMap();

        private void Update()
        {
            if (generateNewMap)
            {
                foreach (Transform child in transform)
                    Destroy(child.gameObject);
                GenerateRandomMap();
                generateNewMap = false;
            }
            
            if (drawDebugLines) 
                DrawDebugLine(LinePositions);

        }

        private void DrawDebugLine(Vector3[] positions)
        {
            if (positions.Length <= 1)
                return;
            for (int i = 1; i < positions.Length; i++)
                Debug.DrawLine(positions[i - 1] + transform.position, positions[i] + transform.position, Color.green);
        }

        public void UpdateLinePositions(Vector3[] newLinePositions)
        {
            LinePositions = newLinePositions;
            GenerateQuads();
        }

        private void GenerateRandomMap()
        {
            if (height < amplitude * 2)
                height = amplitude * 2;
            GenerateRandomHeights();
            SmoothenHeights();
            AddSideSlopes();
            CreatePositions();
            GenerateQuads();
            GenerateObjects();
        }

        private float GetRandomHeight() => Random.value * 2.0f - 1.0f;

        private void GenerateRandomHeights()
        {
            heights = new float[(int)(pointsPerWidth * width)];
            for (int i = 0; i < heights.Length; i++)
                heights[i] = GetRandomHeight();
        }

        private void SmoothenHeights()
        {
            for (int i = 1; i < heights.Length; i++)
                heights[i] = Mathf.Lerp(heights[i], heights[i - 1], smoothFactor);
        }

        private void AddSideSlopes()
        {
            float baseAmplitude = -0.9f;
            int numberOfPoints = (int)(slopeWidth * pointsPerWidth);
            List<float> heightList = new List<float>();
            var firstSlope = GetSlope(heights[0]);
            var lastSlope = GetSlope(heights[heights.Length - 1]);

            for (int i = 0; i < firstSlope.Length; i++)
                heightList.Add(firstSlope[i]);
            for (int i = 0; i < heights.Length; i++)
                heightList.Add(heights[i]);
            for (int i = lastSlope.Length - 1; i >= 0; i--)
                heightList.Add(lastSlope[i]);

            heights = heightList.ToArray();

            float[] GetSlope(float height)
            {
                float[] slopeHeights = new float[numberOfPoints];
                for (int i = 0; i < numberOfPoints; i++)
                    slopeHeights[i] = Mathf.Lerp(baseAmplitude, height, Mathf.Log(i + 1.0f, numberOfPoints));
                return slopeHeights;
            }
        }

        private void CreatePositions()
        {
            float widthPerPoint = 1.0f / pointsPerWidth;
            float trueWidth = width + slopeWidth * 2;
            LinePositions = new Vector3[heights.Length];
            for (int i = 0; i < heights.Length; i++)
                LinePositions[i] = new Vector3(i * widthPerPoint - trueWidth / 2.0f, height - amplitude + heights[i] * amplitude);
        }

        private Point[] PositionsToPoints()
        {
            Point[] points = new Point[LinePositions.Length];
            for (int i = 0; i < points.Length; i++)
                    points[i] = new Point(LinePositions[i]);

            for (int i = 0; i < points.Length; i++)
                for (int j = 0; j < points.Length; j++)
                    if (ConflictWithEarlierPoint(i, j) || ConflictWithLaterPoint(i, j))
                    {
                        points[i].CanReachFloor = false;
                        break;
                    }

            return points;

            bool ConflictWithEarlierPoint(int i, int j) => i > j && !IsToTheLeft(i, j) && IsBelow(i, j);
            bool ConflictWithLaterPoint(int i, int j) => i < j && !IsToTheRight(i, j) && IsBelow(i, j);

            bool IsToTheRight(int i, int j) => points[i].position.x < points[j].position.x;
            bool IsToTheLeft(int i, int j) => points[i].position.x > points[j].position.x;
            bool IsBelow(int i, int j) => points[i].position.y > points[j].position.y;
        }

        private void GenerateQuads()
        {
            Point[] points = PositionsToPoints();

            vertices = new List<Vector3>();
            triangleIndices = new List<int>();
            quads = new List<Quad>();
            int verticesPerPosition = 4;

            for (int i = 0; i < points.Length; i++)
            {
                if (i < 1)
                {
                    AddFirstSide(points[i].position);
                    points[i].IsConnected = true;
                    continue;
                }
                if (!points[i].CanReachFloor)
                    continue;

                points[i].vertexIndex = i+1 != LinePositions.Length ? AddVertex(points[i].position) : AddLastSide(points[i].position);
                int pastIndex = points[i].vertexIndex - verticesPerPosition;
                for (int j = 0; j < verticesPerPosition - 1; j++)
                    AddQuad(pastIndex + j, points[i].vertexIndex + j, j < 2);
                points[i].IsConnected = true;
            }

            //TODO kolla �ver algoritmen f�r att koppla trianglarna med andra punkter d� den kan bli fel om flera okopplade sitter n�ra varandra
            int previousIndex, nextIndex = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if (!points[i].IsConnected)
                {
                    previousIndex = points[i - 1].vertexIndex;
                    points[i].vertexIndex = AddVertex(points[i].position, true);
                    if (nextIndex < i)
                        nextIndex = GetNextConnectedPoint(i);
                    AddQuad(previousIndex, points[i].vertexIndex, true);
                    quads.Add(new Quad(points[i].vertexIndex + 1, previousIndex + 1, points[nextIndex].vertexIndex + 1, -1, true));

                    if (nextIndex == i + 1)
                        AddQuad(points[i].vertexIndex, points[nextIndex].vertexIndex, true);

                    points[i].IsConnected = true;
                }

            }

            CreateMesh(CreateSubMeshDescriptors(quads));

            void AddQuad(int pastIndex, int vertexIndex, bool isTopside){
                quads.Add(new Quad(pastIndex, pastIndex + 1, vertexIndex, vertexIndex + 1, isTopside));
            }

            int GetNextConnectedPoint(int startIndex)
            {
                int index = startIndex;
                if (index + 1 >= points.Length)
                {
                    Debug.Log($"Index was {index} the length of points is {points.Length}");
                    return -1;
                }
                    
                
                while (!points[++index].IsConnected)
                    if (index + 1 >= points.Length)
                        return -1;
                return index;
            }
        }

        //private void GenerateQuads()
        //{
        //    vertices = new List<Vector3>();
        //    triangleIndices = new List<int>();
        //    quads = new List<Quad>();
        //    int verticesPerPosition = 4;

        //    for (int i = 0; i < LinePositions.Length; i++)
        //    {
        //        if (i < 1)
        //        {
        //            AddFirstSide(LinePositions[i]);
        //            continue;
        //        }
        //        int vertexIndex = i + 1 != LinePositions.Length ? AddVertex(LinePositions[i]) : AddLastSide(LinePositions[i]);
        //        int pastIndex = vertexIndex - verticesPerPosition;
        //        for (int j = 0; j < verticesPerPosition - 1; j++)
        //            AddToQuad(pastIndex + j, vertexIndex + j, j < 2);
        //    }

        //    CreateMesh(CreateSubMeshDescriptors(quads));

        //    void AddToQuad(int pastIndex, int vertexIndex, bool isTopside)
        //    {
        //        quads.Add(new Quad(pastIndex, pastIndex + 1, vertexIndex, vertexIndex + 1, isTopside));
        //    }
        //}

        //private void ConnectPoints(Point[] points)
        //{
        //    for (int i = 0; i < points.Length; i++)
        //        if (!points[i].IsConnected)
        //            points[i].vertexIndex = AddVertex(points[i].position, true);

        //    int nextIndex = 0, lastIndex = 0;
        //    for (int i = 0; i < points.Length - 2; i++)
        //    {
        //        if (points[i].IsConnected && GetNextConnectedPoint(i) - i >= 2)
        //        {
        //            lastIndex = GetNextConnectedPoint(i);
        //            int currentIndex = i;
        //            while (currentIndex < lastIndex)
        //            {
        //                quads.Add(new Quad(points[i].vertexIndex + 1, points[currentIndex + 1].vertexIndex + 1,
        //                                    points[currentIndex + 2].vertexIndex + 1, points[currentIndex + 3].vertexIndex + 1 + 1, true));
        //                AddQuad(points[i].vertexIndex, points[currentIndex + 1].vertexIndex, true);
        //                AddQuad(points[currentIndex + 1].vertexIndex, points[currentIndex + 2].vertexIndex, true);
        //                AddQuad(points[currentIndex + 2].vertexIndex, points[currentIndex + 3].vertexIndex, true);
        //                points[currentIndex + 1].IsConnected = points[currentIndex + 2].IsConnected = true;
        //                currentIndex += 2;
        //            }
        //        }
        //        else if (!points[i + 1].IsConnected)
        //        {
        //            nextIndex = i + 1;
        //            lastIndex = i + 2;
        //            points[nextIndex].IsConnected = true;
        //            AddTriangle(points[i].vertexIndex, points[nextIndex].vertexIndex, points[lastIndex].vertexIndex, true);
        //            AddQuad(points[i].vertexIndex, points[nextIndex].vertexIndex, true);
        //            AddQuad(points[nextIndex].vertexIndex, points[lastIndex].vertexIndex, true);
        //        }
        //    }

        //    void AddTriangle(int vertexA, int vertexB, int vertexC, bool isTopside)
        //    {
        //        quads.Add(new Quad(vertexB + 1, vertexA + 1, vertexC + 1, -1, isTopside));
        //    }
        //}

        int AddVertex(Vector3 vertex, bool onlyTop = false)
        {
            int index = vertices.Count;
            
            vertices.Add(new Vector3(vertex.x, vertex.y, depth));
            vertices.Add(vertex);
            if (onlyTop)
                return index;
            vertices.Add(new Vector3(vertex.x, vertex.y - overhangHeight, -GetOverhangDepth()));
            vertices.Add(new Vector3(vertex.x, 0, -bottomDepth));
            return index;
            float GetOverhangDepth()
            {
                float angle = Mathf.Atan(bottomDepth/vertex.y);
                return overhangHeight * Mathf.Tan(angle);
            }
        }

        void AddFirstSide(Vector3 vertex)
        {
            vertices.Add(new Vector3(vertex.x, vertex.y - overhangHeight, depth));
            vertices.Add(new Vector3(vertex.x, 0, depth));
            int index = AddVertex(vertex);
            quads.Add(new Quad(index, index - 2, index + 1, index + 2, true));
            quads.Add(new Quad(index - 2, index - 1, index + 2, index + 3));
        }

        int AddLastSide(Vector3 vertex)
        {
            int index = AddVertex(vertex);
            vertices.Add(new Vector3(vertex.x, vertex.y - overhangHeight, depth));
            vertices.Add(new Vector3(vertex.x, 0, depth));
            quads.Add(new Quad(index + 1, index + 2, index + 0, index + 4, true));
            quads.Add(new Quad(index + 2, index + 3, index + 4, index + 5));
            return index;
        }

        private List<SubMeshDescriptor> CreateSubMeshDescriptors(List<Quad> quads)
        {
            List<SubMeshDescriptor> descriptors = new List<SubMeshDescriptor>();

            IndexQuads(true);
            int lastIndex = triangleIndices.Count - 1;
            descriptors.Add(new SubMeshDescriptor(0, lastIndex + 1));
            IndexQuads(false);
            descriptors.Add(new SubMeshDescriptor(lastIndex + 1, triangleIndices.Count - lastIndex - 1));

            return descriptors;

            void IndexQuads(bool isTopside)
            {
                foreach (Quad quad in quads)
                    if (quad.isTopside == isTopside)
                        foreach (int index in quad.GetTriangleIndices())
                            triangleIndices.Add(index);
            }
        }

        private void CreateMesh(List<SubMeshDescriptor> descriptors)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.triangles = triangleIndices.ToArray();
            mesh.RecalculateNormals();
            mesh.SetSubMeshes(descriptors);

            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshCollider collider = gameObject.GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        private void GenerateObjects()  
        {
            GetComponent<GenerateDecor>().GenerateObjects(LinePositions, width, depth);
            GetComponent<GenerateSpawnpoints>().GenerateObjects(LinePositions, width, depth);
        }
    }

    public struct Point
    {
        public readonly Vector3 position;
        public int vertexIndex;
        public bool IsConnected { get; set; }
        public bool CanReachFloor { get; set; }
        public Point(Vector3 position)
        {
            this.position = position;
            IsConnected = false;
            CanReachFloor = true;
            vertexIndex = -1;
        }
    }

    public struct Quad
    {
        private readonly int upperLeftIndex, lowerLeftIndex, upperRightIndex, lowerRightIndex;
        public readonly bool isTopside;
        public Quad(int upperLeftIndex, int lowerLeftIndex, int upperRightIndex, int lowerRightIndex, bool isTopside = false)
        {
            this.upperLeftIndex = upperLeftIndex;
            this.lowerLeftIndex = lowerLeftIndex;
            this.upperRightIndex = upperRightIndex;
            this.lowerRightIndex = lowerRightIndex;
            this.isTopside = isTopside;
        }

        public int[] GetTriangleIndices()
        {
            if (lowerRightIndex < 0)
            {
                int[] triangle = { lowerLeftIndex, upperLeftIndex, upperRightIndex };
                return triangle;
            }

            int[] indices = { lowerLeftIndex, upperLeftIndex, upperRightIndex,
                              lowerLeftIndex, upperRightIndex, lowerRightIndex};
            return indices;
        }
    }
}


