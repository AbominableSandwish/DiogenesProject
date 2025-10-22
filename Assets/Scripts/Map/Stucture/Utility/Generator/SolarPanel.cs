using UnityEngine;

public class SolarPanel : Generator
{
    [SerializeField] protected new float _electricProduction = 100.0f;
    [SerializeField] protected new float _electricPower;

    #region Public Method

    public SolarPanel(int x = 0, int y = 0)
    {
        this._type = StructureType.Generator;
        this._position =new Vector3Int(x, y);
    }

    public override float Output()
    {
        return _electricProduction;
    }

    public float ElectricProduction { get => _electricProduction; set => _electricProduction = value; }
  
    public void OnConnect()
    {
        _isEnabled = true;
    }

    public void OnDisconnect()
    {
        _isEnabled = false;
    }

    public override bool ToPlace(Vector3Int pos)
    {
        return Map.AddStructure<SolarPanel>(pos);
    }

    public override bool ToRemove(Vector3Int pos)
    {
        return Map.RemoveStructure<SolarPanel>(pos);
    }
    #endregion
}