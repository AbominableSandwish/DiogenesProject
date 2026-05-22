using UnityEngine;

public class PathDebugDrawer : MonoBehaviour
{
    [SerializeField] private PathRequestManager pathRequestManager;

    private void OnDrawGizmos()
    {
        if (pathRequestManager == null)
            return;

        foreach (PathDebugData data in pathRequestManager.DebugPaths)
        {
            Gizmos.color = data.Success ? Color.green : Color.red;

            Vector3 start = CellToWorld(data.Start);
            Vector3 end = CellToWorld(data.End);

            Gizmos.DrawSphere(start, 0.15f);
            Gizmos.DrawWireSphere(end, 0.2f);
            Gizmos.DrawLine(start, end);

            if (data.Path == null)
                continue;

            for (int i = 0; i < data.Path.Count; i++)
            {
                Vector3 current = CellToWorld(data.Path[i]);

                Gizmos.DrawCube(current, Vector3.one * 0.2f);

                if (i < data.Path.Count - 1)
                {
                    Vector3 next = CellToWorld(data.Path[i + 1]);
                    Gizmos.DrawLine(current, next);
                }
            }
        }
    }

    private Vector3 CellToWorld(Vector3Int cell)
    {
        return new Vector3(cell.x, cell.y, 0f);
    }
}