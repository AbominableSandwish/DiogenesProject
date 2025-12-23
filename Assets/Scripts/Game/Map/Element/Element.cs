using UnityEngine;

public class Element
{
    #region Private Data
    private string _name = "";
    private float _temperature = 34;
    private string _path;
    #endregion

    #region Public Data
    public string Name { get => _name; set => _name = value; }
    public float Temperature { get => _temperature; set => _temperature = value; }
    public string Path { get => _path; set => _path = value; }
    #endregion

    public Element()
    {

    }
}
