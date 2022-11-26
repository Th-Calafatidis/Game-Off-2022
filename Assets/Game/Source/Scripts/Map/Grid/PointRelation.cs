using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PointRelation
{
    Vector2Int PointA;
    Vector2Int PointB;
    bool ValidPath;
    int distance;

    public PointRelation(Vector2Int pointA, Vector2Int pointB, bool validPath, int distance)
    {
        PointA = pointA;
        PointB = pointB;
        ValidPath = validPath;
        this.distance = distance;
    }
}
