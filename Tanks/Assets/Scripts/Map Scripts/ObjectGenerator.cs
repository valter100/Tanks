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

    protected bool RandomBool() => Random.value > 0.5f;

    protected bool ValidDistanceFromObjects(Vector3 vector)
    {
        foreach (Vector3 position in usedPositions)
            if ((position - vector).magnitude <= minimumObjectDistance)
                return false;

        return true;
    }

    protected void CreateObjectAtPosition(Vector3 position, GameObject gameObject)
    {
        gameObject.transform.position = position;
        Instantiate(gameObject, transform, true);
    }
}
