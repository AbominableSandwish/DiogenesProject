using TMPro;
using UnityEngine;

public class VillagerTest : MonoBehaviour
{
    [SerializeField] private Villager villager;
    [SerializeField] private MapManager map;
    [SerializeField] private TestManager testManager;

    public bool IsFinish = true;
    public bool IsSuccess = false;

    private Vector3Int spawnPosition;
    private Vector3Int targetPosition;

    [SerializeField] private TextMeshProUGUI _result;

    public bool TrySetupFromMarkers()
    {
        if (!TryFindMarker(StructureType.Begin, out spawnPosition))
        {
            Debug.LogError("Missing Begin marker.");
            return false;
        }

        if (!TryFindMarker(StructureType.End, out targetPosition))
        {
            Debug.LogError("Missing End marker.");
            return false;
        }

        return true;
    }

    public void Respawn()
    {
        villager.SetPosition(spawnPosition);
    }


    private bool TryFindMarker(StructureType type, out Vector3Int position)
    {
        foreach (var kv in map.BasicMap.Structures)
        {
            if (kv.Value.Type == type)
            {
                position = kv.Key;
                return true;
            }
        }

        position = default;
        return false;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        villager = UnityResolver.Resolve(villager, this, "Villager");
        map = UnityResolver.Resolve(map, this, nameof(MapManager));
        testManager = UnityResolver.Resolve(testManager, this, nameof(TestManager));
    }

    public void LaunchTest()
    {

        Debug.Log($"BEGIN = {spawnPosition}");
        Debug.Log($"END = {targetPosition}");
        Debug.Log($"Villager start = {villager.transform.position}");
        // Le villageois cherche un chemin jusqu’au niveau supérieur
        villager.MoveTo(targetPosition);

        _result.text = "";
        _result.color = Color.white;
        IsFinish = false;
    }

    private void Update()
    {
        if (!IsFinish) {
            if (villager.currentState == Villager.State.Idle)
            {
                IsFinish = true;
                
                if (Vector3.Distance(villager.transform.position, targetPosition) < 0.01f)
                {
                    IsSuccess = true;
                    _result.text = "Success";
                    _result.color = Color.green;
                }
                else
                {
                    IsSuccess = false;
                    _result.text = "Fail";
                    _result.color = Color.red;
                }

                FindAnyObjectByType<TestManager>().NextTest();
            }
        }
    }
}
