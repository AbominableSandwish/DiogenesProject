using System;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : Generator
{
    [SerializeField] protected new float _electricProduction = 100.0f;
    [SerializeField] protected new float _electricPower;

    private void FixedUpdate()
    {
        if (_isEnabled)
        {
            _electricPower = _electricProduction;
        }
    }

    public override float Production()
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
}