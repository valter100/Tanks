using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Tanks.MapPoint;

public class GenerateMap : MonoBehaviour
{
    enum GenerationMethod
    {
        Lerp, Points, Wave
    }

    [Header("Debug Variables")]
    [SerializeField] private bool generateNewSeed;
    [SerializeField] private bool generateOldSeed;
    [SerializeField] private bool drawDebugLines = false;
    [SerializeField] private bool printDebug;
    [SerializeField] private string mapSeed;
    private int pointsInSlope;

    [Header("Generation Variables")]
    [SerializeField] private GenerationMethod generationMethod;
    [Range(0.1f, 6.0f)]
    [SerializeField] private float pointsPerWidth;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float smoothFactor;
    [Tooltip("How far down past the edge the overhang goes")]
    [SerializeField] private float overhangHeight;
    [SerializeField] private bool randomizeSlopes = true;

    [Header("Point options")]
    [Min(1)]
    [SerializeField] private int minPointDistance;
    [Min(2)]
    [SerializeField] private int maxPointDistance;

    [Header("Wave options")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float waveWidthMin;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float waveWidthMax;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float waveAmplitudeMin;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float waveAmplitudeMax;
    [Tooltip("How often a new wave is started")]
    [Range(0.01f, 1.0f)]
    [SerializeField] private float changeFrequency;


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
    [Range(0, 1)]
    [SerializeField] private float slopeAmplitudePercentage;

    private float[] heights;
    public Vector3[] LinePositions { get; private set; }
    private Vector3[] InitialTopside;

    private List<int> triangleIndices;
    private List<Vector3> vertices;
    private List<Quad> quads;

    private void Print(string text) { if (printDebug) { Debug.Log(text); } }

    private void Update()
    {
        if (generateNewSeed || generateOldSeed)
        {
            if (generateOldSeed)
                GenerateRandomMap(mapSeed);
            else
                GenerateRandomMap();

            generateNewSeed = generateOldSeed = false;
        }

        if (drawDebugLines)
            DrawDebugLine(LinePositions);

    }

    private void DrawDebugLine(Vector3[] positions)
    {
        Vector3 basePosition = transform.position;
        Vector3 forwardPosition = basePosition + new Vector3(0, 0, -0.25f);
        if (positions.Length <= 1)
            return;
        for (int i = 1; i < positions.Length; i++)
            Debug.DrawLine(positions[i - 1] + basePosition, positions[i] + basePosition, Color.green);
        Debug.DrawLine(positions[0] + forwardPosition, positions[pointsInSlope] + forwardPosition, Color.magenta);
        Debug.DrawLine(positions[positions.Length -1] + forwardPosition, positions[positions.Length - 1 - pointsInSlope] + forwardPosition, Color.magenta);
    }

    public void UpdateLinePositions(Vector3[] newLinePositions)
    {
        LinePositions = newLinePositions;
        GenerateQuads();
    }

    public void GenerateRandomMap(string seed = "")
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (seed == "")
            seed = System.DateTime.Now.Ticks.ToString();

        mapSeed = seed;
        Random.InitState(seed.GetHashCode());
        GenerateHeights();
        SetLinePositions();
        GenerateQuads();
        GenerateObjects();
    }

    private void GenerateHeights()
    {
        GenerateRandomHeights();
        switch (generationMethod)
        {
            case GenerationMethod.Lerp:
                for (int i = 1; i < heights.Length; i++)
                    heights[i] = Mathf.Lerp(heights[i], heights[i - 1], smoothFactor);
                break;
            case GenerationMethod.Points:
                SmoothenTowardsPoints();
                break;
            case GenerationMethod.Wave:
                SmoothenAlongWave();
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
        int nextValue;
        List<int> pointIndices = new List<int>() { 0 };
        if (minPointDistance > maxPointDistance)
            maxPointDistance = minPointDistance;

        while (pointIndices[pointIndices.Count - 1] < heights.Length - 1)
        {
            nextValue = pointIndices[pointIndices.Count - 1] + Random.Range(minPointDistance, maxPointDistance);
            nextValue = nextValue >= heights.Length ? heights.Length - 1 : nextValue;
            pointIndices.Add(nextValue);
            Print("Added point generation index value: " + nextValue);
        }

        int pointIndex = 0;
        for (int i = 0; i < heights.Length; i++)
        {
            if (i == pointIndices[pointIndex + 1])
            {
                ++pointIndex;
                continue;
            }
            float percentBetween = (i - pointIndices[pointIndex]) / (float)(pointIndices[pointIndex + 1] - pointIndices[pointIndex]);
            float targetHeight = Mathf.Lerp(heights[pointIndices[pointIndex]], heights[pointIndices[pointIndex + 1]], percentBetween);
            heights[i] = Mathf.Lerp(heights[i], targetHeight, smoothFactor);
        }
    }

    private void SmoothenAlongWave()
    {
        float targetHeight;
        float waveWidth = 0;
        float waveAmplitude = 0;
        if (waveWidthMin > waveWidthMax)
            waveWidthMax = waveWidthMin;
        if (waveAmplitudeMin > waveAmplitudeMax)
            waveAmplitudeMin = waveAmplitudeMax;

        for (int i = 0; i < heights.Length; i++)
        {
            targetHeight = GetTargetHeightFor(i);
            heights[i] = Mathf.Lerp(heights[i], targetHeight, smoothFactor);
        }

        void GetRandomWave()
        {
            waveWidth = Mathf.PI / (Random.Range(waveWidthMin, waveWidthMax) * heights.Length / 2.0f + 1);
            waveAmplitude = amplitude * Random.Range(waveAmplitudeMin, waveAmplitudeMax);
            Print($"wave width {waveWidth}, amplitude {waveAmplitude}");
        }

        float GetTargetHeightFor(int i)
        {
            float sinusValue = Mathf.Sin(i * waveWidth);
            if (sinusValue > -changeFrequency && sinusValue < changeFrequency)
                GetRandomWave();
            return sinusValue * waveAmplitude;
        }
    }

    private void AddSideSlopes()
    {
        float baseAmplitude = -1f;
        pointsInSlope = (int)(slopeWidth * pointsPerWidth);
        List<float> heightList = new List<float>();
        var firstSlope = GetSlope(heights[0], false);
        var lastSlope = GetSlope(heights[heights.Length - 1], true);

        for (int i = 0; i < firstSlope.Length; i++)
            heightList.Add(firstSlope[i]);
        for (int i = 0; i < heights.Length; i++)
            heightList.Add(heights[i]);
        for (int i = 0; i < lastSlope.Length; i++)
            heightList.Add(lastSlope[i]);

        heights = heightList.ToArray();

        float[] GetSlope(float height, bool isBackwards)
        {
            float[] slopeHeights = new float[pointsInSlope];
            float modifiedAmplitude;
            float slopeAmplitude = amplitude * slopeAmplitudePercentage;

            for (int i = 0; i < pointsInSlope; i++)
            {
                float lerpAmount;
                if (isBackwards)
                {
                    lerpAmount = Mathf.Log(pointsInSlope - i, pointsInSlope);
                    modifiedAmplitude = baseAmplitude + Mathf.Lerp(GetRandomHeight() * slopeAmplitude, 0, lerpAmount);
                }
                else
                {
                    lerpAmount = Mathf.Log(i + 1.0f, pointsInSlope);
                    modifiedAmplitude = baseAmplitude + Mathf.Lerp(GetRandomHeight() * slopeAmplitude, 0, 1 - lerpAmount);
                }
                
                slopeHeights[i] = Mathf.Lerp(randomizeSlopes ? modifiedAmplitude : baseAmplitude, height, lerpAmount);
                
                if (isBackwards && i == pointsInSlope -1)
                    slopeHeights[i] = Mathf.Lerp(slopeHeights[i], slopeHeights[i - 1], 0.69f);
            }
            return slopeHeights;
        }
    }

    private void SetLinePositions()
    {
        float widthPerPoint = 1.0f / pointsPerWidth;
        float trueWidth = width + slopeWidth * 2;
        LinePositions = new Vector3[heights.Length];
        InitialTopside = new Vector3[heights.Length];
        for (int i = 0; i < heights.Length; i++)
        {
            LinePositions[i] = new Vector3(i * widthPerPoint - trueWidth / 2.0f, height - amplitude + heights[i] * amplitude);
            InitialTopside[i] = LinePositions[i] + new Vector3(0, -overhangHeight);
        }
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
                AddQuad(pastIndex + j, points[i].VertexIndex + j);
            points[i].IsConnected = true;
        }

        GenerateTriangles(points);

        CreateMesh(CreateSubMeshDescriptors(quads));
    }

    private void GenerateTriangles(Point[] points)
    {
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
                AddQuad(previousIndex, points[i].VertexIndex);
                AddTriangle(points[i].VertexIndex, points[nextIndex].VertexIndex);

                if (nextIndex == i + 1)
                    AddQuad(points[i].VertexIndex, points[nextIndex].VertexIndex);

                points[i].IsConnected = true;
            }

        }

        int SecureAddVertex(Point point)
        {
            if (!point.IsIndexed)
                point.VertexIndex = AddVertex(point.position, true);
            return point.VertexIndex;
        }

        int GetNextConnectedPoint(int startIndex)
        {
            int index = startIndex;
            if (index + 1 >= points.Length)
            {
                Print($"Index was {index} the length of points is {points.Length}");
                return -1;
            }

            while (!points[++index].IsConnected)
                if (index + 1 >= points.Length)
                    return -1;
            return index;
        }

        void AddTriangle(int currentIndex, int nextIndex)
        {
            bool isTopside = IsQuadAboveGrass(vertices[currentIndex + 1], vertices[nextIndex + 1], false);
            quads.Add(new Quad(currentIndex + 1, previousIndex + 1, nextIndex + 1, -1, isTopside));
        }
    }

    private void AddQuad(int pastIndex, int vertexIndex)
    {
        bool isTopside = IsQuadAboveGrass(vertices[pastIndex], vertices[vertexIndex]);
        quads.Add(new Quad(pastIndex, pastIndex + 1, vertexIndex, vertexIndex + 1, isTopside));
    }

    private bool IsQuadAboveGrass(Vector3 topLeft, Vector3 topRight, bool requireBoth = true)
    {
        if (requireBoth)
            return GetClosestOverhangPosition(topLeft.x).y < topLeft.y && GetClosestOverhangPosition(topRight.x).y < topRight.y;
        return GetClosestOverhangPosition(topLeft.x).y < topLeft.y || GetClosestOverhangPosition(topRight.x).y < topRight.y;
    }

    Vector3 GetClosestOverhangPosition(float positionX)
    {
        float previousDistance = float.PositiveInfinity, newDistance;
        for (int i = 0; i < InitialTopside.Length; i++)
        {
            newDistance = Mathf.Abs(positionX - InitialTopside[i].x);
            if (newDistance >= previousDistance)
                return InitialTopside[i - 1];
            previousDistance = newDistance;
        }

        return InitialTopside[InitialTopside.Length - 1];
    }

    int AddVertex(Vector3 vertex, bool onlyTop = false)
    {
        int index = vertices.Count;

        vertices.Add(new Vector3(vertex.x, vertex.y, depth));
        vertices.Add(vertex);
        if (onlyTop)
            return index;

        //If above overhangPosition use old overhang instead
        Vector3 overhangPosition = GetClosestOverhangPosition(vertex.x);
        if (overhangPosition.y < vertex.y)
            vertices.Add(overhangPosition - new Vector3(0, 0, GetOverhangDepth(overhangPosition.y + overhangHeight)));
        else
            vertices.Add(new Vector3(vertex.x, vertex.y - overhangHeight, -GetOverhangDepth(overhangPosition.y + overhangHeight)));

        vertices.Add(new Vector3(vertex.x, 0, -bottomDepth));
        return index;
        float GetOverhangDepth(float targetHeight)
        {
            float angle = Mathf.Atan(bottomDepth / targetHeight);
            return overhangHeight * Mathf.Tan(angle);
        }
    }

    void AddFirstSide(Vector3 vertex)
    {
        vertices.Add(new Vector3(vertex.x, vertex.y - overhangHeight, depth));
        vertices.Add(new Vector3(vertex.x, 0, depth));
        int index = AddVertex(vertex);
        quads.Add(new Quad(index, index - 2, index + 1, index + 2, IsQuadAboveGrass(vertex, vertex)));
        quads.Add(new Quad(index - 2, index - 1, index + 2, index + 3));
    }

    int AddLastSide(Vector3 vertex)
    {
        int index = AddVertex(vertex);
        vertices.Add(new Vector3(vertex.x, vertex.y - overhangHeight, depth));
        vertices.Add(new Vector3(vertex.x, 0, depth));
        quads.Add(new Quad(index + 1, index + 2, index + 0, index + 4, IsQuadAboveGrass(vertex, vertex)));
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
        public bool IsIndexed => VertexIndex != -1;
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



