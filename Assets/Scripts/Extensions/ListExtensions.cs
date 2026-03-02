using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ListExtensions
{
    public static List<Vector2> ToUniVectors2(this List<VoronatorSharp.Vector2> vector2s)
    {
        return vector2s.Select(v => new Vector2(v.x, v.y)).ToList();
    }
}