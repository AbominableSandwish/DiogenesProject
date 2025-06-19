using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Character
{
    Map _map;

    Vector2 input = Vector2.zero;
    [SerializeField] private float _timeToMove = 2.0f;
    private float _timer = 0.0f;

    private Vector3 _targetPos = Vector3.zero;
    private Vector3 _lastPos = Vector3.zero;
    private void Start()
    {
        _map = GameManager.GetMap();

        _targetPos = transform.position;
        _lastPos = transform.position;
        _name = "Francis";
    }
    void Update()
    {
        _timer += Time.deltaTime;
        transform.position = Vector3.Lerp(_lastPos, _targetPos, _timer * (1.0f / _timeToMove));
        Vector3 move = Vector3.zero;
        
        if (_timer >= _timeToMove)
        {
            
            if (input.magnitude != 0)
            {
                _lastPos = _targetPos;
                if (input.x > 0)
                {
                    move += Vector3.right;
                }

                if (input.x < 0)
                {
                    move += Vector3.left;
                }

                if (input.y > 0)
                {
                    move += Vector3.up;
                }

                if (input.y < 0)
                {
                    move += Vector3.down;
                }

                TileBase wall = _map.GetTile(new Vector2Int((int)_targetPos.x + (int)move.x, (int)_targetPos.y + (int)move.y));
                if(wall == null) {
                   
                    _targetPos += new Vector3((int)move.x, (int)move.y);
                }

                _timer = 0.0f;
            }
        }


        //transform.position += Time.deltaTime * ((Vector3)move) * PIXEL_PER_UNIT;
      

    }

    public void Move(InputAction.CallbackContext contex)
    {
        input = contex.ReadValue<Vector2>();
    }  
}
