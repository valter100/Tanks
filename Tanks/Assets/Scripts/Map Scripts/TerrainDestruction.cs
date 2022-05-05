using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class TerrainDestruction : MonoBehaviour
    {
        [Range(1, 8)]
        [SerializeField] int numberOfDivisions = 2;

        private List<Line> lines;
        private List<Point> points;
        private List<Vector3> newLinePositions;

        //public void DestroyTerrain(Vector3 explosionOrigin, float explosionRadius)
        //{
        //    //TODO byt ut metod till att kolla om linjer korsar cirkeln, via cirkelns ekvation.
        //    //TODO Lägg även till nya punkter längs cirkelbågen.
        //    GenerateMap mapGenerator = gameObject.GetComponent<GenerateMap>();
        //    newLinePositions = new List<Vector3>();
        //    List<Vector3> linePositions = new List<Vector3>();
        //    linePositions.AddRange(mapGenerator.LinePositions);
        //    Vector3 origin = explosionOrigin - transform.position;
        //    origin.z = 0;

        //    Debug.Log("Origin: " + origin);

        //    for (int i = 0; i < linePositions.Count; i++)
        //    {
        //        if (!IsInExplosion(i))
        //        {
        //            newLinePositions.Add(linePositions[i]);
        //            continue;
        //        }
        //        int previousIndex = i - 1;
        //        int nextIndex = i + 1;

        //        if (IsWithinBounds(previousIndex))
        //            newLinePositions.AddRange(GetProjectedPositions(origin, explosionRadius, linePositions[previousIndex], linePositions[i]));

        //        if (IsWithinBounds(nextIndex))
        //            newLinePositions.AddRange(GetProjectedPositions(origin, explosionRadius, linePositions[nextIndex], linePositions[i]));

        //        if (!IsWithinBounds(previousIndex) && !IsWithinBounds(nextIndex))
        //            newLinePositions.Add(GetProjectedPosition(origin, explosionRadius, linePositions[i]));
        //        else
        //            ++i;
        //    }

        //    Debug.Log("Previous line positions: " + linePositions.Count);
        //    Debug.Log("New line positions: " + newLinePositions.Count);

        //    mapGenerator.UpdateLinePositions(newLinePositions.ToArray());
        //    bool IsInExplosion(int i) => (linePositions[i] - origin).magnitude < explosionRadius;
        //    bool IsWithinBounds(int i) => i >= 0 && i < linePositions.Count;
        //}

        //private Vector3 GetProjectedPosition(Vector3 origin, float radius, Vector3 position)
        //{
        //    float height = origin.y + Mathf.Sqrt(Mathf.Pow(radius, 2) + Mathf.Pow(origin.x, 2) + Mathf.Pow(position.x, 2) + position.x * origin.x * 2);
        //    return new Vector3(position.x, height, position.z);
        //}

        //private List<Vector3> GetProjectedPositions(Vector3 origin, float radius, Vector3 positionA, Vector3 positionB, int recursionDepth = 0)
        //{
        //    List<Vector3> positions = new List<Vector3>();
        //    if (recursionDepth >= numberOfDivisions)
        //        return positions;

        //    Vector3 newPosition = GetProjection(origin, radius, positionA, positionB);

        //    positions.AddRange(GetProjectedPositions(origin, radius, positionA, newPosition, recursionDepth + 1));
        //    positions.Add(newPosition);
        //    positions.AddRange(GetProjectedPositions(origin, radius, newPosition, positionB, recursionDepth + 1));

        //    return positions;
        //}

        //private Vector3 GetProjection(Vector3 origin, float radius, Vector3 positionA, Vector3 positionB)
        //{
        //    Vector3 vectorA = positionA - origin;
        //    Vector3 vectorB = positionB - origin;
        //    return (vectorA + vectorB).normalized * radius + new Vector3(origin.x, origin.y);
        //}

        public void DestroyTerrain(Vector3 explosionOrigin, float explosionRadius)
        {
            //TODO Ta bort punkter om de inte är kopplade frammåt eller bakåt.
            //TODO Gå igenom alla punkter som är kvar och placera ut punkter i vinkeln mellan de som placerats ut under borttagning. 
            GenerateMap mapGenerator = gameObject.GetComponent<GenerateMap>();
            
            newLinePositions = new List<Vector3>();
            CreatePointsAndLines(mapGenerator.LinePositions);
            Vector3 origin = explosionOrigin - transform.position;
            origin.z = 0;
            Explosion explosion = new Explosion(origin, explosionRadius);

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsInExplosion(explosion))
                {
                    if (i > 0 && !lines[i - 1].IsInExplosion(explosion))
                    {
                        Vector3[] intersections = GetIntersection(lines[i], explosion);
                        Debug.Log($"Intersections size: {intersections.Length}");
                        if (intersections.Length > 0)
                            lines[i].PointB.Position = GetClosestValidPosition(lines[i], lines[i].PointA, intersections);
                        else
                            AddPoint(lines[i].PointB, lines[i].PointA.ConnectedToPrevious);
                    }
                        
                    if (i < lines.Count - 1 && !lines[i + 1].IsInExplosion(explosion))
                    {
                        Vector3[] intersections = GetIntersection(lines[i], explosion);
                        Debug.Log($"Intersections size: {intersections.Length}");
                        if (intersections.Length > 0)
                            lines[i].PointA.Position = GetClosestValidPosition(lines[i], lines[i].PointB, intersections);
                        else
                            AddPoint(lines[i].PointB, lines[i].PointA.ConnectedToPrevious);
                    }
                    
                    continue;
                }

                if (i < 1)
                    AddLine(lines[i], true);
                else
                    AddPoint(lines[i].PointB, lines[i].PointA.ConnectedToPrevious);
            }

            Debug.Log("Previous line positions: " + points.Count);
            Debug.Log("New line positions: " + newLinePositions.Count);
            mapGenerator.UpdateLinePositions(newLinePositions.ToArray());
        }

        private void CreatePointsAndLines(Vector3[] positions)
        {
            points = new List<Point>();
            lines = new List<Line>();
            for (int i = 0; i < positions.Length; i++)
            {
                points.Add(new Point(positions[i]));
                if (i > 0)
                    lines.Add(new Line(points[i-1], points[i]));
            }
        }

        private void AddPoint(Point point, bool connectedToPrevious, bool connectedToNext = true)
        {
            if (point.PositionIndex != -1)
                RemovePoint(point);

            point.ConnectedToPrevious = connectedToPrevious;
            point.ConnectedToNext = connectedToNext;
            point.PositionIndex = newLinePositions.Count;
            newLinePositions.Add(point.Position);
        }

        private void AddLine(Line line, bool connectedToPrevious, bool connectedToNext = true)
        {
            AddPoint(line.PointA, connectedToPrevious, connectedToNext);
            AddPoint(line.PointB, connectedToPrevious, connectedToNext);
        }

        private void RemovePoint(Point point)
        {
            if (point.PositionIndex < 0)
                return;
            newLinePositions.RemoveAt(point.PositionIndex);
            point.PositionIndex = -1;
        }

        private void RemovePointRange(Point firstPoint, Point lastPoint)
        {
            int count = lastPoint.PositionIndex - firstPoint.PositionIndex + 1;
            newLinePositions.RemoveRange(firstPoint.PositionIndex, count);
        }

        private Vector3[] GetIntersection(Line line, Explosion explosion)
        {   // Calculates intersection between explosion circle and line
            // (x - a)^2 + (y - b)^2 = r^2
            // y = kx + m
            Vector3[] intersections = new Vector3[0];
            float k, m, a, b, r;
            float p, q;
            k = line.k;
            m = line.m;
            a = explosion.Origin.x;
            b = explosion.Origin.y;
            r = explosion.Radius;
            if(k != 0)
            {
                p = 2 * m - 2 * b - 2 * a / k;
                q = (a * a + b * b + m * m - r * r - 2 * b * m) / k;
            }
            else
            {
                p = -2 * a;
                q = m * m + a * a + b * b - 2 * m * b - r * r;
            }
            float discriminant = Mathf.Pow(p / 2, 2) - q;
            if (discriminant >= 0)
            {
                float x1 = -p / 2 + Mathf.Sqrt(Mathf.Pow(p / 2, 2) - q);
                float x2 = -p / 2 - Mathf.Sqrt(Mathf.Pow(p / 2, 2) - q);
                intersections = new Vector3[] { new Vector3(x1, k * x1 + m), new Vector3(x2, k * x2 + m) };
            }
            return intersections;
        }

        private Vector3 GetClosestValidPosition(Line line, Point targetPoint, Vector3[] positions)
        {
            int closestIndex = 0;
            if (line.IsPointOnLineSegment(positions[0]) != line.IsPointOnLineSegment(positions[1]))
                closestIndex = line.IsPointOnLineSegment(positions[0]) ? 0 : 1;
            else
            {
                bool firstIsCloser = Vector3.Distance(targetPoint.Position, positions[0]) < Vector3.Distance(targetPoint.Position, positions[1]);
                closestIndex = firstIsCloser ? 0 : 1;
            }
            return positions[closestIndex];
        }

        public class Point
        {
            public Vector3 Position { get; set; }
            public int PositionIndex { get; set; }
            public bool ConnectedToPrevious { get; set; }
            public bool ConnectedToNext { get; set; }

            private bool isInExplosion;

            public Point(Vector3 position)
            {
                Position = position;
                PositionIndex = -1;
                ConnectedToPrevious = true;
                ConnectedToNext = true;
            }

            public bool IsInExplosion(Explosion explosion)
            {
                if (!isInExplosion)
                    isInExplosion = (Position - explosion.Origin).magnitude < explosion.Radius;
                return isInExplosion;
            }
        }

        public class Line
        {
            public Point PointA { get; set; }
            public Point PointB { get; set; }
            private Point LeftPoint => PointA.Position.x < PointB.Position.x ? PointA : PointB;
            private Point RightPoint => PointA.Position.x >= PointB.Position.x ? PointA : PointB;
            public readonly float k, m;
            public Line(Point pointA, Point pointB)
            {
                PointA = pointA;
                PointB = pointB;
                m = LeftPoint.Position.y;
                k = (RightPoint.Position.y - LeftPoint.Position.y) / (RightPoint.Position.x - LeftPoint.Position.x);               
            }

            //TODO Räkna med linjer som överskrids men som inte är har punkter i explosioner
            public bool IsInExplosion(Explosion explosion) => LeftPoint.IsInExplosion(explosion) || RightPoint.IsInExplosion(explosion);
            public bool IsPointOnLineSegment(Vector3 point)
            {   //Checks if point is within bounds and if kx + m - y = 0
                float kxmy = k * point.x + m - point.y;
                float accuracy = 0.001f;
                return LeftPoint.Position.x <= point.x && RightPoint.Position.x > point.x && (kxmy < accuracy || kxmy > -accuracy);
            }
        }

        public struct Explosion
        {
            public Vector3 Origin { get; set; }
            public float Radius { get; set; }
            public Explosion(Vector3 origin, float radius)
            {
                Origin = origin;
                Radius = radius;   
            }
        }
    }
}

