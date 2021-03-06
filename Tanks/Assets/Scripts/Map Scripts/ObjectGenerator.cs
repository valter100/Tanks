using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] protected float minimumObjectDistance;
    [SerializeField] protected float heightAdjustment;

    protected Vector3[] linePositions;
    protected float depth;
    protected List<Vector3> usedPositions;

    public virtual void GenerateObjects(Vector3[] linePositions, float width, float depth)
    {
        this.linePositions = linePositions;
        this.depth = depth;
        usedPositions = new List<Vector3>();
    }

    protected int RandomInt(int max) => (int)(Random.value * max);
    protected int RandomInt(int max, int positionsFromEdge)
    {
        if (positionsFromEdge >= max)
            positionsFromEdge = max - 1;
        return Random.Range(positionsFromEdge, max - positionsFromEdge);
    }

    protected bool RandomBool() => Random.value > 0.5f;

    protected float RandomAngle() => Random.value * 360.0f;

    protected bool ValidDistanceFromObjects(Vector3 vector)
    {
        foreach (Vector3 position in usedPositions)
            if ((position - vector).magnitude <= minimumObjectDistance)
                return false;

        return true;
    }

    protected virtual void CreateObjectAtPosition(Vector3 position, GameObject gameObject, float rotation = 0.0f)
    {
        GameObject instantiatedObject = Instantiate(gameObject, transform, true);
        instantiatedObject.transform.position = position;
        instantiatedObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        if (rotation != 0 && transform.childCount > 0)
            instantiatedObject.transform.GetChild(0).rotation = Quaternion.Euler(instantiatedObject.transform.GetChild(0).eulerAngles + new Vector3(0, 0, rotation));
    }
}

