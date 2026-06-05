using UnityEngine;

[CreateAssetMenu(menuName = "Diogenes/Villager Schedule")]
public class Schedule : ScriptableObject
{
    [SerializeField] private VillagerActivity[] hours = new VillagerActivity[24];

    public VillagerActivity GetActivity(float hour)
    {
        int index = Mathf.FloorToInt(hour) % 24;
        return hours[index];
    }

    public void SetActivity(int hour, VillagerActivity activity)
    {
        if (hour < 0 || hour >= 24)
            return;

        hours[hour] = activity;
    }
}