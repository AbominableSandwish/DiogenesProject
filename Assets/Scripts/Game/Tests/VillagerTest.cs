using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillagerTest : MonoBehaviour
{
    [SerializeField] private Villager villager;
    Vector3Int spawnPosition;

    public bool IsFinish = true;
    public bool IsSuccess = false;

    Vector3Int Targetposition = new Vector3Int(39, 0, 0);


    [SerializeField] private TextMeshProUGUI _result;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        villager = FindAnyObjectByType<Villager>();
        spawnPosition = new Vector3Int((int)villager.transform.position.x, (int)villager.transform.position.y);

        LaunchTest();
    }

    public void LaunchTest()
    {
     
        // Le villageois cherche un chemin jusqu’au niveau supérieur
        villager.MoveTo(Targetposition);

        _result.text = "";
        _result.color = Color.white;
        IsFinish = false;
    }

    public void Respawn()
    {
        villager.SetPosition(spawnPosition);
    }

    private void Update()
    {
        if (!IsFinish) {
            if (villager.currentState == Villager.State.Idle)
            {
                IsFinish = true;
                
                if (Vector3.Distance(villager.transform.position, Targetposition) < 0.01f)
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
