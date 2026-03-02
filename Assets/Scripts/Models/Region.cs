using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region
{
    public int Id;
    public bool IsFocused;
    public bool IsOpen;
    public List<Vector2> Vertices;
    public List<int> NeighborIds;
    public Vector2 Center;

    public bool IsNeighbor(int id)
    {
        return NeighborIds.Contains(id);
    }

    public void Restart()
    {
        IsFocused = false;
        IsOpen = false;
    }

    public void Activate()
    {
        IsOpen = true;
    }
}