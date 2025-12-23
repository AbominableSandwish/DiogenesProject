using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CircuitData
{
    public List<Vector3Int> coils = new();
    public List<ConnMaskData> connections = new();
    public List<EntityData> generators = new();
    public List<EntityData> engines = new();
    public List<EntityData> storages = new();
}

[Serializable]
public class ConnMaskData
{
    public Vector3Int position;
    public byte mask; // correspond à ton enum Conn (Up/Right/Down/Left)
}

[Serializable]
public class EntityData
{
    public Vector3Int position;
    public string id; // ou type d’entité, selon ton besoin
}