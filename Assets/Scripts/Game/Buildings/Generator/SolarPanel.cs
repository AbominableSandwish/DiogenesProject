/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.Tilemaps;

public class SolarPanel : Generator
{
    [SerializeField] protected new float _electricProduction = 100.0f;
    [SerializeField] protected new float _electricPower;

    new protected string _name = "SolarPanel";
    public override string TileAssetReference => "SolarPanel";
    public override StructureType Type => StructureType.SolarPanel;

    #region Public Method

    public SolarPanel(Tilemap tilemap = null, Vector3Int position = new Vector3Int()): base(tilemap, position)
    {
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