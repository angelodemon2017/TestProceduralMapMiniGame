using System.Collections.Generic;
using UnityEngine;

public class ProceduralRegion
{
    public static Mesh CreateRegionMesh(List<Vector2> points2D, Vector2 center2D, float zHeight, float regionInset = 0f)
    {
        Mesh mesh = new Mesh();
        mesh.name = Dicts.FixNames.ProceduralRegionMeshName;

        int n = points2D.Count;

        Vector3[] vertices = new Vector3[n + 1];
        vertices[0] = new Vector3(center2D.x, center2D.y, zHeight);

        for (int i = 0; i < n; i++)
        {
            float distanceTotal = Vector2.Distance(center2D, points2D[i]);
            float t = regionInset / distanceTotal;
            var point = Vector2.Lerp(center2D, points2D[i], 1 - t);
            var x = point.x;
            var y = point.y;

            vertices[i + 1] = new Vector3(x, y, zHeight);
        }

        int[] triangles = new int[(n) * 3];

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;

            int baseIdx = i * 3;

            triangles[baseIdx + 0] = 0;
            triangles[baseIdx + 1] = i + 1;
            triangles[baseIdx + 2] = next + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; i++)
            normals[i] = Vector3.up;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[vertices.Length];
        uv[0] = new Vector2(0.5f, 0.5f);

        float maxDist = 0f;
        for (int i = 1; i < vertices.Length; i++)
        {
            float d = Vector2.Distance(center2D, points2D[i - 1]);
            if (d > maxDist) maxDist = d;
        }

        for (int i = 1; i < vertices.Length; i++)
        {
            Vector2 dir = (points2D[i - 1] - center2D).normalized;
            uv[i] = new Vector2(0.5f + dir.x * 0.5f, 0.5f + dir.y * 0.5f);
        }

        mesh.uv = uv;

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}