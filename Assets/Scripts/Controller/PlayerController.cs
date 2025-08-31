using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    Map _map;
    private Character _character;
    Vector2 input = Vector2.zero;
    [SerializeField] private float _timeToMove = 2.0f;
    private float _timer = 0.0f;

    private Vector3 _targetPos = Vector3.zero;
    private Vector3 _lastPos = Vector3.zero;
    private void Start()
    {
        _map = Map.Instance;

        _targetPos = transform.position;
        _lastPos = transform.position;
        this._character  = new Character("Francis");

        this._character.Structures.Add(new Coil()); 
        this._character.Structures.Add(new SolarPanel());
        this._character.Structures.Add(new Lamp());
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

                TileBase structure = _map?.GetStructure(new Vector2Int((int)_targetPos.x + (int)move.x, (int)_targetPos.y + (int)move.y), StructureType.Basic);
                if(structure == null) {
                   
                    _targetPos += new Vector3((int)move.x, (int)move.y);
                }

                _timer = 0.0f;
            }
        }
    }

    public void Move(InputAction.CallbackContext contex)
    {
        input = contex.ReadValue<Vector2>();
    }

    public void AddElement(InputAction.CallbackContext contex)
    {
        if (contex.performed)
        {
            float action = contex.ReadValue<float>();

            if (_character.Select != null)
            {
               _character.Select.ToPlace(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            }
        }
    }

    public void RemoveElement(InputAction.CallbackContext contex)
    {
        float action = contex.ReadValue<float>();
    }



    public bool SelectStructure(int id)
    {
        bool isSelect = false;
        if (this._character.Select != this._character.Structures[id])
        {
            this._character.Select = this._character.Structures[id];
            isSelect = true;
        }
        else
        {
            this._character.Select = null;
        }
        return isSelect;
    }
}
