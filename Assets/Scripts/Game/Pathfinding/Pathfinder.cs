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

        // Vertical (Y) via Ladder/Stair en CHAÎNE
        {
            var up = new Vector3Int(pos.x, pos.y + 1, pos.z);
            var down = new Vector3Int(pos.x, pos.y - 1, pos.z);

            if (CanClimbBetween(pos, up)) yield return up;
            if (CanClimbBetween(pos, down)) yield return down;
        }
    }

    private bool CanClimbBetween(Vector3Int from, Vector3Int to)
    {
        if (!IsInsideLimits(to)) return false;

        // On ne grimpe verticalement que sur la même colonne X/Z
        if (from.x != to.x || from.z != to.z) return false;

        // Il faut que l'espace cible soit occupable (au moins vide pour le corps)
        // (On ne met pas grid.IsWalkable ici, sinon tu risques de bloquer le climb)
        if (!IsEmptyForBody(to)) return false;

        // Il faut une échelle/escalier sur la case de départ OU sur la case d'arrivée
        return IsClimbTile(from) || IsClimbTile(to);
    }

    private bool IsClimbTile(Vector3Int cell)
    {
        var s = grid.GetStructure(cell, StructureMap.Basic);
        if (s == null) return false;

        return s.Type == StructureType.Ladder;
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

        // 0) Si un obstacle est présent AU MÊME NIVEAU, il prend la priorité sur IsWalkable
        //    - grimpable -> on tente Y+1
        //    - non grimpable -> on bloque
        var obstacle = grid.GetStructure(front, StructureMap.Basic);

        if (obstacle != null)
        {
            // Grimpable (ex: WoodPlateform) => step-up d'1
            if (IsClimbableBlockType(obstacle.Type))
            {
                var ontoTop = new Vector3Int(front.x, front.y + 1, front.z);

                // Monter sur le bloc : espace libre pour le corps + dans les limites
                if (IsInsideLimits(ontoTop) && IsEmptyForBody(ontoTop))
                {
                    stepUp = ontoTop;
                }
                // sinon : bloqué (pas de sameLevel)
                return;
            }

            // Si c'est une échelle/escalier, on ne bloque pas le mouvement horizontal à cause de ça.
            // (Sinon une échelle posée sur une case empêcherait de marcher)
            if (obstacle.Type == StructureType.Ladder)
            {
                // on continue sur le check normal plus bas
            }
            else
            {
                // Obstacle non grimpable => bloqué net
                return;
            }
        }

        // 1) Déplacement normal à même niveau
        if (IsValidStep(from, front))
        {
            sameLevel = front;
            return;
        }

        // 2) Optionnel: step down
        var down = new Vector3Int(front.x, front.y - 1, front.z);
        if (IsValidStep(from, down))
            stepDown = down;
    }

    private bool IsClimbableBlockType(StructureType type)
    {
        // Mets ici tous les blocs "1 de haut" sur lesquels on peut monter
        return type == StructureType.WoodPlateform;
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
