//using System.Collections.Generic;
//using UnityEditor.AddressableAssets.Build;
//using UnityEngine;
//using UnityEngine.Audio;
//using UnityEngine.Tilemaps;
//using UnityEngine.UIElements;

//public class TaskPriorityOverlay : MonoBehaviour
//{
//    [Header("References")]
//    [SerializeField] private Tilemap overlayTilemap;
//    [SerializeField] private TileRegistry tileRegistry;

//    [Header("Priority Tiles")]
//    [SerializeField] private List<TileBase> priorityTiles;
//    [SerializeField] private string priorityTileName = "Number_";

//    private readonly Dictionary<Vector3Int, Task> displayedTasks = new();

//    public void Refresh(IEnumerable<Task> tasks)
//    {
//        overlayTilemap.ClearAllTiles();
//        displayedTasks.Clear();

//        foreach (Task task in tasks)
//        {
//            if (task == null)
//                continue;

//            TileBase tile = GetTileForPriority(task.Priority);
//            // View


//            if (tile == null)
//                continue;



//            overlayTilemap.SetTile(task.TargetPosition, tile);
//            displayedTasks[task.TargetPosition] = task;
//        }
//    }

//    public void ShowTask(Task task)
//    {
//        if (task == null)
//            return;

//        TileBase tile = GetTileForPriority(task.Priority);

//        if (tile == null)
//            return;

//        overlayTilemap.SetTile(task.TargetPosition, tile);
//        displayedTasks[task.TargetPosition] = task;
//    }

//    public void HideTask(Task task)
//    {
//        if (task == null)
//            return;

//        overlayTilemap.SetTile(task.TargetPosition, null);
//        displayedTasks.Remove(task.TargetPosition);
//    }

//    private TileBase GetTileForPriority(int priority)
//    {
//        priority = Mathf.Clamp(priority, 1, 9);

//        Tile tile = new Tile
//        {
//            name = priorityTileName + priority.ToString(),
//            sprite = _tileRegistry.Get(name).sprite,
//            colliderType = Tile.ColliderType.Grid
//        };
//        Tilemap.SetTile(position, tile);
//        return
//    }
//}