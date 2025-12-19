using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Private Data

    private MapManager _map;
    private float _timer = 0.0f;
    private Vector2 _input = Vector2.zero;
    private Vector3 _targetPos = Vector3.zero;
    private Vector3 _lastPos = Vector3.zero;
    #endregion

    public Structure.StructureType type;

    #region Mono
    private void Start()
    {
        _map = MapManager.Instance;

        _targetPos = transform.position;
        _lastPos = transform.position;
    }
    #endregion

    #region Public Method
    public void Move(InputAction.CallbackContext contex)
    {
        _input = contex.ReadValue<Vector2>();
    }

    public void AddElement(InputAction.CallbackContext contex)
    {
        if (contex.performed)
        {
            float action = contex.ReadValue<float>();

            Vector3Int pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y);
            if (this.type != Structure.StructureType.NONE)
            {
                switch (this.type)
                {
                    case Structure.StructureType.Coil:
                        MapManager.AddStructure<Coil>(pos);
                        break;
                    case Structure.StructureType.Generator:
                        MapManager.AddStructure<Generator>(pos);
                        break;
                    case Structure.StructureType.Engine:
                        MapManager.AddStructure<Engine>(pos);
                        break;
                    case Structure.StructureType.Storage:
                        MapManager.AddStructure<Storage>(pos);
                        break;
                    case Structure.StructureType.Lamp:
                        MapManager.AddStructure<Lamp>(pos);
                        break;
                    case Structure.StructureType.SolarPanel:
                        MapManager.AddStructure<SolarPanel>(pos);
                        break;
                    case Structure.StructureType.Ground:
                        MapManager.AddStructure<Ground>(pos);
                        break;
                    case Structure.StructureType.Ladder:
                        MapManager.AddStructure<Ladder>(pos);
                        break;
                    case Structure.StructureType.Door:
                        MapManager.AddStructure<Door>(pos);
                        break;
                }
            }
        }
    }

    public void RemoveElement(InputAction.CallbackContext contex)
    {
        if (contex.performed)
        {
            float action = contex.ReadValue<float>();
            this.type = Structure.StructureType.NONE;
        }
    }


    public bool SelectStructure(Structure.StructureType type)
    {
        bool isSelect = false;
        if (this.type != type)
        {
            this.type = type;
            isSelect = true;
         }
        else
        {
            this.type = Structure.StructureType.NONE;
        }
        return isSelect;
    }
    #endregion
}
