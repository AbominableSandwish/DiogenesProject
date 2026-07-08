using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TaskPriorityOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap overlayTilemap;
    [SerializeField] private TileRegistry tileRegistry;
    [SerializeField] private TaskManager taskManager;

    [Header("Priority Tiles")]
    [SerializeField] private List<TileBase> priorityTiles;
    [SerializeField] private string priorityTileName = "Number_";

    private readonly Dictionary<Vector3Int, VillagerTask> displayedTasks = new();

    private void Awake()
    {
        taskManager = UnityResolver.Resolve(taskManager, this, nameof(TaskManager));
    }
    private void Start()
    {
        overlayTilemap = UnityResolver.Resolve(overlayTilemap, this, nameof(Tilemap));       
        tileRegistry = UnityResolver.Resolve(tileRegistry, this, nameof(TileRegistry));
    }

    private void Update()
    {
        Refresh(taskManager.GetTasks());
    }

    public void Refresh(IEnumerable<VillagerTask> tasks)
    {
        overlayTilemap.ClearAllTiles();
        displayedTasks.Clear();

        foreach (VillagerTask task in tasks)
        {
            if (task == null)
                continue;

            TileBase tile = GetTileForPriority(task.Priority);
            if (tile == null)
                continue;

            overlayTilemap.SetTile(task.TargetPosition, tile);
            displayedTasks[task.TargetPosition] = task;
        }
    }

    public void ShowTask(VillagerTask task)
    {
        if (task == null)
            return;

        Tile tile = GetTileForPriority(task.Priority);

        if (tile == null)
            return;

        overlayTilemap.SetTile(task.TargetPosition, tile);
        displayedTasks[task.TargetPosition] = task;
    }

    public void HideTask(VillagerTask task)
    {
        if (task == null)
            return;

        overlayTilemap.SetTile(task.TargetPosition, null);
        displayedTasks.Remove(task.TargetPosition);
    }

    private Tile GetTileForPriority(int priority)
    {
        priority = Mathf.Clamp(priority, 1, 9);

        string name = priorityTileName + priority.ToString();
        Tile tile = new Tile
        {   
            sprite = tileRegistry.Get(name).sprite,
            colliderType = Tile.ColliderType.Grid
        };

        return tile;
    }

    private void OnEnable()
    {
        Refresh(taskManager.GetTasks());
    }
}