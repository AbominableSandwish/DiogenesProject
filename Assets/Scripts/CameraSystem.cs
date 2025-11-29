using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Vector3 _center;

    private GridManager _grid;
    private Vector2 _maxArea;
    [SerializeField] private float velocity = 5.0f;

    private Vector2 _movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _grid = FindAnyObjectByType<GridManager>();

        _center = transform.position;
        transform.position = new Vector3(_center.x, _center.y, transform.position.z);
        _maxArea = new Vector2(_grid.Width, _grid.Height);
    }

    private void Update()
    {
        if (_movement.magnitude > 0)
        {
            Vector3 newPosition = transform.position + (Vector3)_movement * velocity * Time.deltaTime;

            if (newPosition.x > _center.x + _maxArea.x / 2.0f)
            {
                newPosition.x = _center.x + _maxArea.x / 2.0f;
            }

            if (newPosition.x < _center.x - _maxArea.x / 2.0f)
            {
                newPosition.x = _center.x - _maxArea.x / 2.0f;
            }

            if (newPosition.y > _center.y + _maxArea.y / 2.0f)
            {
                newPosition.y = _center.y + _maxArea.y / 2.0f;
            }

            if (newPosition.y < _center.y - _maxArea.y / 2.0f)
            {
                newPosition.y = _center.y - _maxArea.y / 2.0f;
            }

            transform.position = newPosition;
        }
    }

    public void Move(Vector2 movement)
    {
        _movement = movement;
    }
}
