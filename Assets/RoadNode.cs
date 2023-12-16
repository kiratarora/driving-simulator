using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode
{
    public Vector3 position;
    public List<RoadNode> neighbors;
    public float gScore;
    public float hScore;
    public float fScore { get { return gScore + hScore; } }
    public RoadNode parent;

    public RoadNode(Vector3 pos)
    {
        position = pos;
        neighbors = new List<RoadNode>();
        gScore = float.MaxValue;
        hScore = float.MaxValue;
        parent = null;
    }
}
