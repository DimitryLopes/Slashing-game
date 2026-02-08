using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SliceAreaPositionData
{
    [SerializeField]
    private string name; //for identification in inspector
    [SerializeField]
    public SliceAreaPosition[] sliceAreaPosition;

    public SliceAreaPosition EntireArea { get; private set; }

    public void Setup()
    {
        EntireArea = GetCombinedAreaConvexHull();
    }

    public SliceAreaPosition GetCombinedAreaConvexHull()
    {
        List<Vector2> allPoints = new List<Vector2>();

        foreach (var area in sliceAreaPosition)
        {
            if (area?.Positions == null) continue;
            foreach (var v in area.Positions)
                allPoints.Add(new Vector2(v.x, v.y));
        }

        if (allPoints.Count < 3)
            return null;

        List<Vector2> hull = ConvexHull(allPoints);

        Vector3[] hull2D = new Vector3[hull.Count];
        for (int i = 0; i < hull.Count; i++)
            hull2D[i] = new Vector3(hull[i].x, hull[i].y, 0);

        var combined = new SliceAreaPosition(hull2D);
        return combined;
    }

    private static List<Vector2> ConvexHull(List<Vector2> points)
    {
        points.Sort((a, b) =>
            a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

        List<Vector2> hull = new List<Vector2>();

        foreach (var p in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(p);
        }

        int t = hull.Count + 1;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            var p = points[i];
            while (hull.Count >= t && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(p);
        }

        hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    private static float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }
}
