using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coil : Element
{
    Structure StructureConnect;

    public bool isConnect()
    {
        return StructureConnect != null;
    }

    public override bool ToPlace(Vector2Int pos)
    {
        bool canPlace = CoilMap.Add(pos);
        if (canPlace)
        {
           //TODO
           //Add structure if has engine or generator in this position
            //AddStructure();
        }
        return canPlace;
    }

    public void AddStructure(Structure structure)
    {
        StructureConnect = structure;
    }
}

