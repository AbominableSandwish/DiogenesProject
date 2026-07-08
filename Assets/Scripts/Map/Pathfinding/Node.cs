/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
public class Node
{
    public Vector3Int position;
    public bool walkable = true;
    public float gCost; // coût du départ au nœud
    public float hCost; // heuristique (distance estimée)
    public float fCost => gCost + hCost;
    public Node parent;

    public Node(Vector3Int pos, bool walkable)
    {
        position = pos;
        this.walkable = walkable;
    }
}