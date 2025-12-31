using UnityEngine;

public class VillagerTest : MonoBehaviour
{
    [SerializeField] private Villager villager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MapManager grid = FindAnyObjectByType<MapManager>();
       
        // Construction d’une échelle entre 2 niveaux
        MapManager.AddStructure<Ladder>(new Vector3Int(3, 4, 1));

        // Le villageois cherche un chemin jusqu’au niveau supérieur
        villager.MoveTo(new Vector3Int(39, 0, 0));
    }
}
