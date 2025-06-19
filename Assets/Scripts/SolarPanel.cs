using System.Collections.Generic;

public class SolarPanel : Structure, IElectrifiable
{

    private float _electricProduction = 10.0f;
    private float _electricPower;
    List<CableCoil> neighbours;
    public bool isConnect()
    {
        return true;
    }

    private void FixedUpdate()
    {
        if (_isEnabled)
        {
            _electricPower = _electricProduction;
        }
    }
    public void OnConnect()
    {
        _isEnabled = true;
    }

    public void OnDisconnect()
    {
        _isEnabled = false;
    }
}