/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private MapManager _map;
    [SerializeField] private GameInput _gameInput;

    [SerializeField] private Vector3 _center;
    [SerializeField] private float velocity = 5.0f;

    private Vector2 _maxArea;
  
    private void Awake()
    {
        _map = UnityResolver.Resolve(_map, this, nameof(MapManager));
        _gameInput = UnityResolver.Resolve(_gameInput, this, nameof(GameInput));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _center = transform.position;
        transform.position = new Vector3(_center.x, _center.y, transform.position.z);
        _maxArea = new Vector2(_map.Width, _map.Height);
    }

    private void Update()
    {
        Vector2 input = _gameInput.MoveInput;
        if (input.magnitude > 0)
        {
            Vector3 newPosition = transform.position + (Vector3)input * velocity * Time.deltaTime;

            if (newPosition.x > _center.x + _maxArea.x / 2.0f)
                newPosition.x = _center.x + _maxArea.x / 2.0f;

            if (newPosition.x < _center.x - _maxArea.x / 2.0f)
                newPosition.x = _center.x - _maxArea.x / 2.0f;

            if (newPosition.y > _center.y + _maxArea.y / 2.0f)
                newPosition.y = _center.y + _maxArea.y / 2.0f;

            if (newPosition.y < _center.y - _maxArea.y / 2.0f)
                newPosition.y = _center.y - _maxArea.y / 2.0f;

            transform.position = newPosition;
        }
    }
}
