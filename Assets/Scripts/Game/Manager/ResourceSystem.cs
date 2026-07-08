/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    [SerializeField] private int _food, _wood, _stone, _copper;
    public int Food { get => _food; set => _food = value; }
    public int Wood { get => _wood; set => _wood = value; }
    public int Stone { get => _stone; set => _stone = value; }
    public int Copper { get => _copper; set => _copper = value; }
}