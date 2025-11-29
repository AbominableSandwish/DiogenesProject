using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Structure;

public class Pathfinder : MonoBehaviour
{
    private GridManager grid;

    private void Awake()
    {
        grid = FindAnyObjectByType<GridManager>();
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        // 1. Listes pour A*
        List<Node> openList = new();
        HashSet<Node> closedList = new();

        // 2. Dictionnaire de noeuds (pour éviter recréation6)
        Dictionary<Vector3Int, Node> allNodes = new();

        Node GetNode(Vector3Int pos)
        {
            if (!allNodes.ContainsKey(pos))
                allNodes[pos] = new Node(pos, true);//map.IsCellFree(pos) || map.GetStructure(pos, Structure.StructureMap.Basic) != null
            return allNodes[pos];
        }

        Node startNode = GetNode(start);
        Node endNode = GetNode(end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 3. Trouver le noeud le moins cher
            Node current = openList.OrderBy(n => n.fCost).First();

            if (current.position == end)
                return RetracePath(startNode, current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector3Int neighborPos in GetNeighbors(current.position))
            {
                Node neighbor = GetNode(neighborPos);

                if (!neighbor.walkable || closedList.Contains(neighbor))
                    continue;

                float tentativeG = current.gCost + Vector3Int.Distance(current.position, neighbor.position);

                if (tentativeG < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Vector3Int.Distance(neighbor.position, end);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        // Aucun chemin trouvé
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
        }

        path.Reverse();
        return path;
    }

    // 🔹 Génère les voisins autorisés, avec gestion des échelles
    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        // Directions horizontales
        Vector3Int[] directions = {
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0)
    };

        foreach (var dir in directions)
        {
            Vector3Int neighbor = pos + dir;

            // Vérifie si le sol en dessous est praticable
            if (grid.IsWalkable(neighbor, StructureMap.Basic))
                yield return neighbor;
        }

        // 🔸 Mouvement vertical si échelle ou escalier
        Structure s = grid.GetStructure(pos, StructureMap.Basic);
        if (s != null && (s.Type == StructureType.Ladder || s.Type == StructureType.Stair))
        {
            Vector3Int up = new Vector3Int(pos.x, pos.y, pos.z + 1);
            Vector3Int down = new Vector3Int(pos.x, pos.y, pos.z - 1);

            // Monter si au-dessus praticable
            if (grid.GetStructure(up, StructureMap.Basic) == null || grid.IsWalkable(up, StructureMap.Basic))
                yield return up;

            // Descendre si au-dessous praticable
            if (grid.GetStructure(down, StructureMap.Basic) == null || grid.IsWalkable(down, StructureMap.Basic))
                yield return down;
        }
    }
}