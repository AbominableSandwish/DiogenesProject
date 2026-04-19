using TMPro;
using UnityEngine;
using static UnityResolver;

public class TestManager : MonoBehaviour
{
    [SerializeField] private SceneRegistry registry;
    [SerializeField] MapManager map;
    [SerializeField] VillagerTest test;

    [SerializeField] private TextMeshProUGUI _title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        registry = UnityResolver.Resolve(registry, this, nameof(SceneRegistry));
        map = UnityResolver.Resolve(map, this, nameof(MapManager));
        test = UnityResolver.Resolve(test, this, nameof(VillagerTest));

        map.RegisterOnExecute(Action);

        NextTest();
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
