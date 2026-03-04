using TMPro;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    SceneRegistry registry;
    MapManager map;
    VillagerTest test;

    [SerializeField] private TextMeshProUGUI _title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        registry = FindAnyObjectByType<SceneRegistry>();
        map = FindAnyObjectByType<MapManager>();
        test = FindAnyObjectByType<VillagerTest>();

        NextTest();
    }

    private void Update()
    {
        
    }

    public void Action()
    {
        test.Respawn();
        test.LaunchTest();
    }

    public void NextTest()
    {
        if (registry != null)
        {
            if (registry.sceneNames.Count != 0)
            {
                _title.text = registry.sceneNames[0].name + " Test:";

                map.LoadWorld(registry.sceneNames[0]);
                registry.sceneNames.Remove(registry.sceneNames[0]);
            }
            else
            {
                Debug.Log("All tests passed");
            }
        }
    }

}
