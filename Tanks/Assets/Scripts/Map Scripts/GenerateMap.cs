using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Tanks.MapPoint;

public class GenerateMap : MonoBehaviour
{
    enum GenerationMethod
    {
        Smoothen, Points
    }

    [Header("Debug Variables")]
    [SerializeField] private bool generateNewMap;
    [SerializeField] private bool drawDebugLines = false;

    [Header("Generation Variables")]
    [SerializeField] private GenerationMethod generationMethod;
    [Range(0.1f, 6.0f)]
    [SerializeField] private float pointsPerWidth;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float smoothFactor;
    [Tooltip("How far down past the edge the overhang goes")]
    [SerializeField] private float overhangHeight;

    [Header("Map Dimensions")]
    [Min(0.1f)]
    [SerializeField] private float amplitude;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float depth;
    [Tooltip("How far out the bottom of the map is")]
    [Min(0)]
    [SerializeField] private float bottomDepth;
    [SerializeField] private float slopeWidth;


    private float[] heights;
    public Vector3[] LinePositions { get; private set; }

    private List<int> triangleIndices;
    private List<Vector3> vertices;
    private List<Quad> quads;
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
        GenerateLinePositions();
        CreatePositions();
        GenerateQuads();
        GenerateObjects();
    }

    private void GenerateLinePositions()
    {
        GenerateRandomHeights();
        switch (generationMethod)
        {   //Smoothen heights
            case GenerationMethod.Smoothen:
                for (int i = 1; i < heights.Length; i++)
                    heights[i] = Mathf.Lerp(heights[i], heights[i - 1], smoothFactor);
                break;
            case GenerationMethod.Points:
                SmoothenTowardsPoints();
                break;
        }
        AddSideSlopes();
    }

    private float GetRandomHeight() => Random.value * 2.0f - 1.0f;

    private void GenerateRandomHeights()
    {
        float heightToAmplitude = 2.5f;
        if (height < amplitude * heightToAmplitude)
            height = amplitude * heightToAmplitude;

        heights = new float[(int)(pointsPerWidth * width)];
        for (int i = 0; i < heights.Length; i++)
            heights[i] = GetRandomHeight();
    }

    private void SmoothenTowardsPoints()
    {
        //TODO se om det går att göra saker lite rundare
        float percentageOfPoints = 1 / (5f + pointsPerWidth);
        int numberOfPoints = (int)(heights.Length * percentageOfPoints);
        Debug.Log($"Percentage of points {percentageOfPoints}, number of points {heights.Length * percentageOfPoints}");
        int pointFrequency = heights.Length / numberOfPoints;
        int firstIndex = 0, nextIndex = 0;
        Debug.Log($"Number of points {numberOfPoints}, point frequency {pointFrequency}");
        for (int i = 0; i < heights.Length; i++)
        {
            if (i % pointFrequency == 0)
            {
                firstIndex = i;
                nextIndex = i + pointFrequency >= heights.Length ? heights.Length - 1 : i + pointFrequency;
                continue;
            }

            float percentBetween = (i - firstIndex) / (float)(nextIndex - firstIndex);
            float targetHeight = Mathf.Lerp(heights[firstIndex], heights[nextIndex], percentBetween);
            Debug.Log($"i:{i} is {percentBetween} percent between {firstIndex} and {nextIndex}");
            heights[i] = Mathf.Lerp(heights[i], targetHeight, smoothFactor);
        }
    }

    private void AddSideSlopes()
    {
        //TODO lägg till lite slump på sluttningarna
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

            points[i].VertexIndex = i + 1 != LinePositions.Length ? AddVertex(points[i].position) : AddLastSide(points[i].position);
            int pastIndex = points[i].VertexIndex - verticesPerPosition;
            for (int j = 0; j < verticesPerPosition - 1; j++)
                AddQuad(pastIndex + j, points[i].VertexIndex + j, j < 2);
            points[i].IsConnected = true;
        }

        //TODO kolla över algoritmen för att koppla trianglarna med andra punkter då den kan bli fel om flera okopplade sitter nära varandra
        int previousIndex, nextIndex = 0;
        for (int i = 0; i < points.Length; i++)
        {
            if (!points[i].IsConnected)
            {
                previousIndex = points[i - 1].VertexIndex;
                points[i].VertexIndex = AddVertex(points[i].position, true);
                if (nextIndex < i)
                    nextIndex = GetNextConnectedPoint(i);
                AddQuad(previousIndex, points[i].VertexIndex, true);
                quads.Add(new Quad(points[i].VertexIndex + 1, previousIndex + 1, points[nextIndex].VertexIndex + 1, -1, true));

                if (nextIndex == i + 1)
                    AddQuad(points[i].VertexIndex, points[nextIndex].VertexIndex, true);

                points[i].IsConnected = true;
            }

        }

        CreateMesh(CreateSubMeshDescriptors(quads));

        void AddQuad(int pastIndex, int vertexIndex, bool isTopside)
        {
            quads.Add(new Quad(pastIndex, pastIndex + 1, vertexIndex, vertexIndex + 1, isTopside));
        }

        int GetNextConnectedPoint(int startIndex)
        {
            int index = startIndex;
            if (index + 1 >= points.Length)
            {
                Debug.LogError($"Index was {index} the length of points is {points.Length}");
                return -1;
            }

            while (!points[++index].IsConnected)
                if (index + 1 >= points.Length)
                    return -1;
            return index;
        }
    }

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
            float angle = Mathf.Atan(bottomDepth / vertex.y);
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
        GetComponent<GenerateSpawnpoints>().PrepareGeneration(LinePositions, width, depth);
    }
}

namespace Tanks.MapPoint
{
    public struct Point
    {
        public readonly Vector3 position;
        public int VertexIndex { get; set; }
        public bool IsConnected { get; set; }
        public bool CanReachFloor { get; set; }
        public Point(Vector3 position)
        {
            this.position = position;
            IsConnected = false;
            CanReachFloor = true;
            VertexIndex = -1;
        }
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



