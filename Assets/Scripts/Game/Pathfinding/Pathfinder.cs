/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private MapManager grid;
    [SerializeField] private List<SpecialConnection> specialConnections = new();

    private void Awake()
    {
        grid = UnityResolver.Resolve(grid, this, "MapManager");
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        Debug.Log($"FindPath start={start} end={end}");

        List<Node> openList = new();
        HashSet<Node> closedList = new();
        Dictionary<Vector3Int, Node> allNodes = new();

        Node GetNode(Vector3Int pos)
        {
            if (!allNodes.TryGetValue(pos, out var node))
            {
                node = new Node(pos, true);
                node.gCost = float.PositiveInfinity;
                allNodes[pos] = node;
            }

            return node;
        }

        if (!IsInsideLimits(start) || !IsInsideLimits(end))
            return null;

        Node startNode = GetNode(start);
        Node endNode = GetNode(end);

        startNode.gCost = 0f;
        startNode.hCost = Heuristic(start, end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = openList.OrderBy(n => n.fCost).First();

            if (current.position == end)
            {
                Debug.Log($"PATH FOUND at {current.position}");
                return RetracePath(startNode, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector3Int neighborPos in GetNeighbors(current.position))
            {
                Node neighbor = GetNode(neighborPos);

                if (closedList.Contains(neighbor))
                    continue;

                float tentativeG = current.gCost + MoveCost(current.position, neighbor.position);

                if (tentativeG < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighbor.position, end);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }

    private List<Vector3Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3Int> path = new();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current.position);
            current = current.parent;
            if (current == null) return null; // sécurité
        }

        path.Reverse();
        return path;
    }

    // ------------------------------------------------------------
    // RÈGLES DE DÉPLACEMENT
    // ------------------------------------------------------------

    /// <summary>
    /// Déplacement libre uniquement en X (même hauteur = même Y).
    /// Changement de Y uniquement via Ladder/Stair.
    /// </summary>
    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        foreach (var n in GetHorizontalNeighbors(pos))
            yield return n;

        foreach (var n in GetVerticalNeighbors(pos))
            yield return n;

        foreach (var n in GetSpecialNeighbors(pos))
            yield return n;
    }

    private IEnumerable<Vector3Int> GetHorizontalNeighbors(Vector3Int pos)
    {
        foreach (var n in GetHorizontalMoves(pos, -1))
            yield return n;

        foreach (var n in GetHorizontalMoves(pos, +1))
            yield return n;
    }

    private IEnumerable<Vector3Int> GetVerticalNeighbors(Vector3Int pos)
    {
        Vector3Int up = new(pos.x, pos.y + 1, pos.z);
        Vector3Int down = new(pos.x, pos.y - 1, pos.z);

        Debug.Log(
            $"Vertical check from {pos} | " +
            $"up={up} upLadder={IsClimbTile(up)} upEmpty={IsEmptyForBody(up)} | " +
            $"down={down} downLadder={IsClimbTile(down)} downEmpty={IsEmptyForBody(down)}"
            );

        if (CanClimbBetween(pos, up))
        {
            Debug.Log($"ADD UP {pos} -> {up}");
            yield return up;
        }

        if (CanClimbBetween(pos, down))
        {
            Debug.Log($"ADD DOWN {pos} -> {down}");
            yield return down;
        }
    }

    private IEnumerable<Vector3Int> GetHorizontalMoves(Vector3Int from, int dx)
    {
        Vector3Int front = new(from.x + dx, from.y, from.z);

        if (!IsInsideLimits(front))
            yield break;

        Structure obstacle = grid.GetStructure(front, StructureLayer.Basic);

        // Obstacle présent à même niveau
        if (obstacle != null)
        {
            // Bloc grimpable => tentative de montée d'1 case
            if (IsClimbableBlockType(obstacle.Type))
            {
                Vector3Int ontoTop = new(front.x, front.y + 1, front.z);

                if (IsInsideLimits(ontoTop) && IsEmptyForBody(ontoTop))
                    yield return ontoTop;

                yield break;
            }

            if (obstacle != null)
            {
                if (IsClimbableBlockType(obstacle.Type))
                {
                    Vector3Int ontoTop = new(front.x, front.y + 1, front.z);

                    if (IsInsidePlayableArea(ontoTop) && IsEmptyForBody(ontoTop))
                        yield return ontoTop;

                    yield break;
                }

                if (!obstacle.IsTraversable)
                    yield break;
            }
        }

        // Déplacement à même niveau
        if (IsValidStep(from, front))
            yield return front;

        // Descente d'un niveau
        Vector3Int down = new(front.x, front.y - 1, front.z);
        if (IsValidStep(from, down))
            yield return down;
    }

    private IEnumerable<Vector3Int> GetSpecialNeighbors(Vector3Int pos)
    {
        foreach (var connection in specialConnections)
        {
            if (connection.From == pos)
                yield return connection.To;

            if (connection.Bidirectional && connection.To == pos)
                yield return connection.From;
        }
    }

    private bool IsValidStep(Vector3Int from, Vector3Int to)
    {
        if (to.x >= 36)
        {
            Debug.Log($"STEP CHECK {to} | inside={IsInsidePlayableArea(to)} empty={IsEmptyForBody(to)} support={HasSupport(to)} type={grid.GetStructure(to, StructureLayer.Basic)?.Type.ToString() ?? "NULL"}");
        }

        if (!IsInsidePlayableArea(to))
            return false;

        if (!IsEmptyForBody(to))
            return false;

        if (IsClimbTile(to))
            return true;
       

        return HasSupport(to);
    }



    private bool CanClimbBetween(Vector3Int from, Vector3Int to)
    {
        if (!IsInsidePlayableArea(to))
            return false;

        if (from.x != to.x || from.z != to.z)
            return false;

        bool fromIsLadder = IsClimbTile(from);
        bool toIsLadder = IsClimbTile(to);

        bool empty = IsEmptyForBody(to);

        Debug.Log($"Climb {from}->{to} | fromLadder={fromIsLadder} toLadder={toIsLadder} empty={empty}");

        if (!fromIsLadder && !toIsLadder)
            return false;

        if (!empty)
            return false;

        return true;
    }

    private bool IsClimbTile(Vector3Int cell)
    {
        var s = grid.GetStructure(cell, StructureLayer.Basic);
        if (s == null) return false;

        return s.Type == StructureType.Ladder;
    }

   

    private bool IsClimbableBlockType(StructureType type)
    {
        // Mets ici tous les blocs "1 de haut" sur lesquels on peut monter
        return type == StructureType.WoodPlateform;
    }


    /// <summary>
    /// Si ton PNJ occupe 1 case, tu peux juste retourner true.
    /// Si tu veux gérer une “tête” (2 cases de haut), vérifie la case au-dessus.
    /// </summary>
    private bool IsEmptyForBody(Vector3Int cell)
    {
     if (!IsInsidePlayableArea(cell))
    return false;

        var body = grid.GetStructure(cell, StructureLayer.Basic);

        if (body != null && !body.IsTraversable)
            return false;

        // Vérification de la tête (PNJ = 2 cases de haut)
        var head = new Vector3Int(cell.x, cell.y + 1, cell.z);
        if (!IsInsideLimits(head))
            return false;

        var headStruct = grid.GetStructure(head, StructureLayer.Basic);
        if (headStruct != null && !headStruct.IsTraversable)
            return false;

        return true;
    }

    private bool HasSupport(Vector3Int cell)
    {
        Vector3Int below = new(cell.x, cell.y - 1, cell.z);

        // Cas spécial : le sol de base est la Limit en y = -1
        if (below.y == -1)
            return true;

        Structure belowStruct = grid.GetStructure(below, StructureLayer.Basic);

        Debug.Log($"SUPPORT for {cell} | below={below} | type={belowStruct?.Type.ToString() ?? "NULL"}");

        if (belowStruct == null)
            return false;

        return belowStruct.Type == StructureType.Limit
            || belowStruct.Type == StructureType.WoodPlateform
            || belowStruct.Type == StructureType.Ladder;
    }

    private bool IsInsidePlayableArea(Vector3Int pos)
    {
        return grid != null
            && pos.x >= 0
            && pos.x < grid.Width
            && pos.y >= 0
            && pos.y < grid.Height;
    }

    // ------------------------------------------------------------
    // Limit ZONE (à adapter selon ton MapManager)
    // ------------------------------------------------------------

    private bool IsInsideLimits(Vector3Int pos)
    {
        return grid != null && grid.IsInBounds(pos);
    }

    // ------------------------------------------------------------
    // COSTS
    // ------------------------------------------------------------

    private float Heuristic(Vector3Int a, Vector3Int b)
    {
        // Manhattan (souvent mieux que Distance float sur une grille)
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private float MoveCost(Vector3Int from, Vector3Int to)
    {
        foreach (var connection in specialConnections)
        {
            if (connection.From == from && connection.To == to)
                return connection.Cost;

            if (connection.Bidirectional && connection.To == from && connection.From == to)
                return connection.Cost;
        }

        return 1f;
    }

    private bool IsTraversableStructure(Structure structure)
    {
        return structure == null || structure.IsTraversable;
    }
}
