using UnityEngine;

public class SolarPanel : Generator
{
    [SerializeField] protected new float _electricProduction = 100.0f;
    [SerializeField] protected new float _electricPower;

    new protected string _name = "SolarPanel";
    public override string TileAssetReference => "SolarPanel";
    public override StructureType Type => StructureType.SolarPanel;

    #region Public Method

    public SolarPanel(int x = 0, int y = 0)
    {
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
    #endregion
}