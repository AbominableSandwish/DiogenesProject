using System.Collections.Generic;
using UnityEngine;

public class PathDebugData
{
    public Vector3Int Start;
    public Vector3Int End;
    public List<Vector3Int> Path;
    public bool Success;
    public float ExpireTime;
}