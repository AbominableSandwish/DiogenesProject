/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;

[System.Serializable]
public class SpecialConnection
{
    public Vector3Int From;
    public Vector3Int To;
    public float Cost = 1f;
    public bool Bidirectional = true;
    public string Type;
}

public enum SpecialConnectionType
{
    Elevator,
    CableCar,
    Portal,
    Rail
}