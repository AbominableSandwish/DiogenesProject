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
        registry = UnityResolver.Resolve(this, registry, nameof(SceneRegistry));
        map = UnityResolver.Resolve(this, map, nameof(MapManager));
        test = UnityResolver.Resolve(this, test, nameof(VillagerTest));

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
