using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class TerrainDestruction : MonoBehaviour
    {
        [SerializeField] ParticleSystem groundParticles;
        [SerializeField] float pointsPerDegree = 0.05f;
        [SerializeField] bool printDebug;

        private List<Line> lines;
        private List<Point> points;
        private List<Vector3> newLinePositions;
        private Explosion explosion;

        private void Print(string text) { if (printDebug) { Debug.Log(text); } }

        public void DestroyTerrain(Vector3 explosionOrigin, float explosionRadius)
        {
            GenerateMap mapGenerator = gameObject.GetComponent<GenerateMap>();

            newLinePositions = new List<Vector3>();
            CreatePointsAndLines(mapGenerator.LinePositions);
            Vector3 origin = explosionOrigin - transform.position;
            origin.z = 0;
            explosion = new Explosion(origin, explosionRadius);

            AddPointsAfterExplosion();

            Print("Previous line positions: " + points.Count);
            Print("New line positions: " + newLinePositions.Count);
            mapGenerator.UpdateLinePositions(newLinePositions.ToArray());

            GenerateExplosionEffect(explosionOrigin, explosionRadius);
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

        private void AddPointsAfterExplosion()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (i == 0 && !points[0].IsInExplosion(explosion))
                    AddLinePosition(lines[i].PointA, true);

                if (lines[i].IsInExplosion(explosion))
                {
                    HandleExplosion(i);
                    continue;
                }
                AddLinePosition(lines[i].PointB, true);
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (!points[i].ConnectedToNext && !points[j].ConnectedToPrevious)
                    {
                        Print("Adding points to cicle");
                        AddPointsAlongCircle(i, j);
                    }
                }
            }     
        }

        private void HandleExplosion(int i)
        {
            if (i == 0) //Hantera f�rsta punkt
            {
                if (lines[i].PointA.IsInExplosion(explosion))
                    lines[i].PointA.Position = new Vector3(lines[i].PointA.Position.x, explosion.GetY(lines[i].PointA.Position.x)[1]);
                AddLinePosition(lines[i].PointA, true, false);
            }

            if (i > 0 && !lines[i - 1].IsInExplosion(explosion))
                HandleFirstPoint();


            if (i < lines.Count - 1 && !lines[i + 1].IsInExplosion(explosion))
                HandleLastPoint();

            if (i == lines.Count - 1) //Hanterar sista punkt
            {
                if (lines[i].PointB.IsInExplosion(explosion))
                {
                    lines[i].PointB.Position = new Vector3(lines[i].PointB.Position.x, explosion.GetY(lines[i].PointB.Position.x)[1]);
                    AddLinePosition(lines[i].PointB, false, true);
                }
                else
                    HandleLastPoint();
            }

            void HandleFirstPoint()
            {
                Vector3[] intersections = GetIntersections(lines[i]);
                if (intersections.Length > 0)
                    lines[i].PointB.Position = GetClosestValidPosition(lines[i], lines[i].PointA, intersections);
                AddLinePosition(lines[i].PointB, true, false);
                i -= HandleRangeRemoval(lines[i].PointB);
            }

            void HandleLastPoint()
            {
                if (!lines[i].PointA.ConnectedToNext)
                {
                    Point point = new Point(lines[i].PointA.Position);
                    AddLinePosition(point, true, false);
                    AddPoint(point, i);
                }

                Vector3[] intersections = GetIntersections(lines[i]);
                if (intersections.Length > 0)
                    lines[i].PointA.Position = GetClosestValidPosition(lines[i], lines[i].PointB, intersections);
                AddLinePosition(lines[i].PointA, false);
                AddLinePosition(lines[i].PointB, true);
            }
        }

        private void AddPointsAlongCircle(int startIndex, int stopIndex)
        {   //TODO kika p� extremfall som f�r cirkeln att placeras i luften
            Point startPoint = points[startIndex];
            Point stopPoint = points[stopIndex];

            Debug.DrawLine(startPoint.Position + transform.position, stopPoint.Position + transform.position, Color.red, 2000);

            float angleStart = GetAngleOnExplosion(startPoint.Position);
            float angleStop = GetAngleOnExplosion(stopPoint.Position);
            if (angleStart >= angleStop)
                angleStart -= 360;
            int numberOfPoints = (int)(explosion.Radius * pointsPerDegree * Mathf.Abs(angleStop - angleStart));
            if (numberOfPoints < 2)
                numberOfPoints = 2;
            Print($"Start angle {angleStart}, stop angle {angleStop}, number of points {numberOfPoints}");

            List<Vector3> positions = new List<Vector3>();

            for (int i = 0; i < numberOfPoints + 2; i++)
            {
                float angle = Mathf.Lerp(angleStart, angleStop, i / (float)(numberOfPoints + 1));
                positions.Add(GetCirclePosition(angle));
            }

            AddLinePositionRange(positions, stopPoint.PositionIndex);
            
            Vector3 GetCirclePosition(float angle)
            {
                Print("Angle: " + angle);
                angle *= Mathf.Deg2Rad;

                Vector3 position = new Vector3(Mathf.Cos(angle) * explosion.Radius, Mathf.Sin(angle) * explosion.Radius);
                return explosion.Origin + position;
            }
        }

        private float GetAngleOnExplosion(Vector3 position)
        {
            Vector3 positionVector = position - explosion.Origin;
            float angle = Mathf.Atan2(positionVector.y, positionVector.x) * Mathf.Rad2Deg;
            if (angle < 0)
                return 360f + angle;
            return angle;
        }

        private void AddLinePosition(Point point, bool connectedToPrevious, bool connectedToNext = true)
        {
            point.ConnectedToPrevious = connectedToPrevious;
            point.ConnectedToNext = connectedToNext;
            point.PositionIndex = newLinePositions.Count;
            newLinePositions.Add(point.Position);
        }

        private void AddLinePositionRange(List<Vector3> positions, int index) => newLinePositions.InsertRange(index, positions);

        private void AddPoint(Point point, int index) => points.Insert(index, point);

        private int HandleRangeRemoval(Point startPoint)
        {
            int startIndex = points.IndexOf(startPoint);
            for (int i = startIndex; i > 0; --i)
                if (!points[i].ConnectedToPrevious)
                    return RemovePointRange(points[i], points[startIndex]);
            return 0;
        }

        private int RemovePointRange(Point firstPoint, Point lastPoint)
        {
            int count = lastPoint.PositionIndex - firstPoint.PositionIndex + 1;
            newLinePositions.RemoveRange(firstPoint.PositionIndex, count);
            firstPoint.ConnectedToPrevious = true;
            lastPoint.ConnectedToNext = true;
            Print($"Removed {count} objects betweed index {firstPoint.PositionIndex} and {lastPoint.PositionIndex}");
            return count;
        }

        private Vector3[] GetIntersections(Line line)
        {   // Calculates intersection between explosion circle and line
            // (x - a)^2 + (y - b)^2 = r^2
            // y = kx + m
            Vector3[] intersections = new Vector3[0];
            float k, m, a, b, r;
            float p, q;
            k = line.K;
            m = line.M;
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
                float x1 = -p / 2 + Mathf.Sqrt(discriminant);
                float x2 = -p / 2 - Mathf.Sqrt(discriminant);
                intersections = new Vector3[] { new Vector3(x1, k * x1 + m), new Vector3(x2, k * x2 + m) };
            }

            return intersections;
        }

        private Vector3 GetClosestValidPosition(Line line, Point targetPoint, Vector3[] positions)
        {

            if (!IsOnCircleEdge(positions[0]) && !IsOnCircleEdge(positions[1]))
                return targetPoint.Position;

            int closestIndex = 0;
            if (line.IsPointOnLineSegment(positions[0]) != line.IsPointOnLineSegment(positions[1]))
                closestIndex = line.IsPointOnLineSegment(positions[0]) ? 0 : 1;
            else
            {
                bool firstIsCloser = Vector3.Distance(targetPoint.Position, positions[0]) < Vector3.Distance(targetPoint.Position, positions[1]);
                closestIndex = firstIsCloser ? 0 : 1;
            }

            return positions[closestIndex];

            bool IsOnCircleEdge(Vector3 position)
            {
                float distanceToOrigin = Vector3.Distance(position, explosion.Origin);
                float marginOfError = 0.05f;
                return distanceToOrigin > explosion.Radius * (1 - marginOfError) && distanceToOrigin < explosion.Radius * (1 + marginOfError);
            }
        }

        private class Point
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

        private class Line
        {
            public Point PointA { get; set; }
            public Point PointB { get; set; }
            private Point LeftPoint => PointA.Position.x < PointB.Position.x ? PointA : PointB;
            private Point RightPoint => PointA.Position.x >= PointB.Position.x ? PointA : PointB;
            public float K => (RightPoint.Position.y - LeftPoint.Position.y) / (RightPoint.Position.x - LeftPoint.Position.x);
            public float M => LeftPoint.Position.y - K * LeftPoint.Position.x;
            public Line(Point pointA, Point pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }

            //TODO R�kna med linjer som �verskrids men som inte �r har punkter i explosioner
            public bool IsInExplosion(Explosion explosion) => LeftPoint.IsInExplosion(explosion) || RightPoint.IsInExplosion(explosion);
            public bool IsPointOnLineSegment(Vector3 point)
            {   //Checks if point is within bounds and if kx + m - y = 0
                float kxmy = K * point.x + M - point.y;
                float accuracy = 0.5f;
                return LeftPoint.Position.x <= point.x && RightPoint.Position.x > point.x && (kxmy < accuracy || kxmy > -accuracy);
            }
        }

        void GenerateExplosionEffect(Vector3 position, float radius)
        {
            GameObject ground = GameObject.FindGameObjectWithTag("Map");
            ParticleSystem.ShapeModule shape = groundParticles.shape;
            ParticleSystem.MainModule main = groundParticles.main;
            main.startColor = ground.GetComponent<Renderer>().material.color;
            shape.radius = radius;
            //groundParticles.startColor.min
            Instantiate(groundParticles, position, Quaternion.identity);

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

            public float[] GetX(float y)
            {
                float a = Origin.x;
                float b = Origin.y;
                float discriminant = Mathf.Sqrt(-y * y + 2 * y * b - b * b + Radius * Radius);
                float x1 = a + discriminant;
                float x2 = a - discriminant;
                float[] x = new float[] {x1, x2};
                return x;
            }

            public float[] GetY(float x)
            {
                float a = Origin.x;
                float b = Origin.y;
                float discriminant = Mathf.Sqrt(-x * x + 2 * x * a - a * a + Radius * Radius);
                float y1 = b + discriminant;
                float y2 = b - discriminant;
                float[] y = new float[] { y1, y2 };
                return y;
            }
        }
    }
}

