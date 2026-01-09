using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Structure;

public class Pathfinder : MonoBehaviour
{
    private MapManager grid;

    private void Awake()
    {
        grid = FindAnyObjectByType<MapManager>();
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        List<Node> openList = new();
        HashSet<Node> closedList = new();
        Dictionary<Vector3Int, Node> allNodes = new();

        Node GetNode(Vector3Int pos)
        {
            if (!allNodes.TryGetValue(pos, out var node))
            {
                // Walkable = dans les limites + walkable sur la map
                bool walkable = IsInsideLimits(pos) && grid.IsWalkable(pos, StructureMap.Basic);

                node = new Node(pos, walkable);

                // Important si ton Node.gCost démarre à 0 par défaut :
                // ça évite des comparaisons bizarres (tentativeG < neighbor.gCost).
                node.gCost = float.PositiveInfinity;

                allNodes[pos] = node;
            }
            return node;
        }

        Node startNode = GetNode(start);
        Node endNode   = GetNode(end);

        if (!startNode.walkable || !endNode.walkable)
            return null;

        startNode.gCost = 0f;
        startNode.hCost = Heuristic(start, end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
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
        // Déplacement horizontal (X)
        TryAddHorizontalWithStep(pos, -1, out var leftSame, out var leftUp, out var leftDown);
        if (leftSame.HasValue) yield return leftSame.Value;
        if (leftUp.HasValue) yield return leftUp.Value;
        if (leftDown.HasValue) yield return leftDown.Value;

        TryAddHorizontalWithStep(pos, +1, out var rightSame, out var rightUp, out var rightDown);
        if (rightSame.HasValue) yield return rightSame.Value;
        if (rightUp.HasValue) yield return rightUp.Value;
        if (rightDown.HasValue) yield return rightDown.Value;

        // Vertical (Y) uniquement via Ladder/Stair (comme avant)
        Structure s = grid.GetStructure(pos, StructureMap.Basic);
        bool canChangeHeight = s != null && (s.Type == StructureType.Ladder || s.Type == StructureType.Stair);

        if (canChangeHeight)
        {
            var up = new Vector3Int(pos.x, pos.y + 1, pos.z);
            var down = new Vector3Int(pos.x, pos.y - 1, pos.z);

            if (IsValidStep(pos, up)) yield return up;
            if (IsValidStep(pos, down)) yield return down;
        }
    }

    private void TryAddHorizontalWithStep(
     Vector3Int from,
     int dx,
     out Vector3Int? sameLevel,
     out Vector3Int? stepUp,
     out Vector3Int? stepDown
 )
    {
        sameLevel = null;
        stepUp = null;
        stepDown = null;

        var front = new Vector3Int(from.x + dx, from.y, from.z);

        // Si c'est un bloc grimpable devant, on NE veut PAS traverser "en ligne droite"
        // => on tente directement de monter dessus.
        if (IsClimbableBlockAt(front))
        {
            var ontoTop = new Vector3Int(front.x, front.y + 1, front.z);

            // Monter sur le bloc = espace libre au-dessus + on reste dans les limites.
            // IMPORTANT: ne pas utiliser grid.IsWalkable(ontoTop) si ton walkable nécessite un sol "tile"
            // car ici le sol est justement le bloc front.
            if (IsInsideLimits(ontoTop) && IsEmptyForBody(ontoTop))
            {
                stepUp = ontoTop;
                return;
            }

            // Si on ne peut pas monter, alors c'est réellement bloqué.
            return;
        }

        // 1) Déplacement normal à même niveau (pas de bloc grimpable devant)
        if (IsValidStep(from, front))
        {
            sameLevel = front;
            return;
        }

        // 2) Optionnel: step down (descendre d'1 en avançant)
        var toDown = new Vector3Int(front.x, front.y - 1, front.z);
        if (IsValidStep(from, toDown))
            stepDown = toDown;
    }

    private bool IsClimbableBlockAt(Vector3Int cell)
    {
        var st = grid.GetStructure(cell, StructureMap.Basic);
        if (st == null) return false;

        // Ici tu mets les blocs "1 de haut" qu'on peut escalader
        return st.Type == StructureType.WoodPlateform;
    }

    // --- Helpers "bloc / espace libre" (à adapter à tes types réels) ---

    private bool IsSolidBlockAt(Vector3Int cell)
    {
        var st = grid.GetStructure(cell, StructureMap.Basic);
        if (st == null) return false;

        // Mets ici le type exact qui “bloque” au même niveau
        return st.Type == StructureType.WoodPlateform; // <- adapte (Wall, Rock, etc.)
    }

    /// <summary>
    /// Si ton PNJ occupe 1 case, tu peux juste retourner true.
    /// Si tu veux gérer une “tête” (2 cases de haut), vérifie la case au-dessus.
    /// </summary>
    private bool IsEmptyForBody(Vector3Int cell)
    {
        // 1 case de haut:
        // return grid.GetStructure(cell, StructureMap.Basic) == null;

        // 2 cases de haut (recommandé si ton perso fait 2 de haut):
        // - la case cell doit être libre
        // - la case au-dessus doit être libre
        if (grid.GetStructure(cell, StructureMap.Basic) != null) return false;

        var head = new Vector3Int(cell.x, cell.y + 1, cell.z);
        if (!IsInsideLimits(head)) return false;
        if (grid.GetStructure(head, StructureMap.Basic) != null) return false;

        return true;
    }

    private bool IsValidStep(Vector3Int from, Vector3Int to)
    {
        // Limites (zone max)
        if (!IsInsideLimits(to))
            return false;

        // Walkable (sol/obstacle/etc.)
        if (!grid.IsWalkable(to, StructureMap.Basic))
            return false;

        // Si tu as la notion de "block qui bloque sur la même ligne",
        // c’est souvent un obstacle sur la case cible => IsWalkable couvre déjà.
        // Sinon, tu peux ajouter un test spécifique ici :
        // if (grid.GetStructure(to, StructureMap.Basic)?.Type == StructureType.Block) return false;

        return true;
    }

    // ------------------------------------------------------------
    // LIMIT ZONE (à adapter selon ton MapManager)
    // ------------------------------------------------------------

    private bool IsInsideLimits(Vector3Int pos)
    {
        // OPTION A (recommandée) : tu ajoutes une fonction côté MapManager
        // return grid.IsInsideLimits(pos);

        // OPTION B : si ton "Limit" est une tuile/structure placée sur la map
        // et que sortir de la zone revient à tomber sur un Limit ou "no cell":
        // - soit tu as un IsInBounds(pos)
        // - soit tu testes la présence de limites autour
        //
        // Exemple minimal si tu as une taille de grille:
        // return grid.IsInBounds(pos);

        // Par défaut, on ne bloque rien tant que tu n'as pas branché.
        return true;
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
        // Tu peux pénaliser le changement de hauteur si tu veux :
        // return (from.y != to.y) ? 2f : 1f;
        return 1f;
    }
}
