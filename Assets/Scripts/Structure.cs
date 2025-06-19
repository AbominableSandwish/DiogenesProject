using System.Collections.Generic;
using UnityEngine;

public interface IElectrifiable
{
    bool isConnect();
}

public class Structure : Element 
{
    [SerializeField] protected int _defense = 10;
    protected bool _isEnabled = false;
}

public class CableCoil : Structure, IElectrifiable
{
    private float _electricPower;
    List<CableCoil> neighbours;
    public bool isConnect()
    {
        return true;
    }
}

