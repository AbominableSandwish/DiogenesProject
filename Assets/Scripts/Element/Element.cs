using UnityEngine;

public class Element
{
    private string _name = "";
    private float _temperature = 34;
    private string _path;

    public string Name { get => _name; set => _name = value; }
    public float Temperature { get => _temperature; set => _temperature = value; }
    public string Path { get => _path; set => _path = value; }

    public Element()
    {

    }

    public virtual bool ToPlace(Vector2Int pos)
    {
        return false;
    }
}
