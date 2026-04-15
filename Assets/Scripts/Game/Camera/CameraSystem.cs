using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Vector3 _center;

    private MapManager _grid;
    private Vector2 _maxArea;
    [SerializeField] private float velocity = 5.0f;

    private GameInput _gameInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _grid = FindAnyObjectByType<MapManager>();
        _gameInput = FindAnyObjectByType<GameInput>();
        if(_gameInput == null)
        {
            Debug.LogWarning("GameInput is not initialized");
        }

        _center = transform.position;
        transform.position = new Vector3(_center.x, _center.y, transform.position.z);
        _maxArea = new Vector2(_grid.Width, _grid.Height);
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
