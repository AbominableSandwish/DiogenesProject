using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class SpriteSystem : MonoBehaviour
{
    //Coil
    public Sprite ZeroConnect;
    public List<Sprite> OneConnect;
    public List<Sprite> TwoConnect;
    public List<Sprite> ThreeConnect;
    public Sprite FourConnect;

    //Solar Panel
    public Sprite SolarPanel;

    //Storage Battery
    public TileBase StorageBattery;

    //Storage Lamp
    public TileBase Lamp;

    public void LoadSprite()
    {
        ZeroConnect = Resources.LoadAll<Sprite>("Sprite/Coils/0_connect")[0];

        OneConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/1_connect"))
        {
            OneConnect.Add(sprite);
        }

        TwoConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/2_connect"))
        {
            TwoConnect.Add(sprite);
        }

        ThreeConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/3_connect"))
        {
            ThreeConnect.Add(sprite);
        }

        FourConnect = Resources.LoadAll<Sprite>("Sprite/Coils/4_connect")[0];
        SolarPanel = Resources.LoadAll<Sprite>("Sprite/Generator/SolarPanel")[0];
        Lamp = Resources.LoadAll<TileBase>("Sprite/Engine/Lamp")[0];
    }
}
