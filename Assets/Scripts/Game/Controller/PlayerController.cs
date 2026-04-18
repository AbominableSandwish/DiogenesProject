using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Private Data

    private MapManager _map;
    private Vector2 _input = Vector2.zero;
    private Vector3 _targetPos = Vector3.zero;
    private Vector3 _lastPos = Vector3.zero;
    #endregion

    public StructureType type;

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

    public void AddStructure(Vector3Int position)
    {
        if (this.type != StructureType.NONE)
        {
            switch (this.type)
            {
                case StructureType.Coil:
                    _map.AddStructure(new Coil(), position);
                    break;
                case StructureType.Generator:
                    _map.AddStructure(new Generator(), position);
                    break;
                case StructureType.Engine:
                    _map.AddStructure(new Engine(), position);
                    break;
                case StructureType.Storage:
                    _map.AddStructure(new Storage(), position);
                    break;
                case StructureType.Lamp:
                    _map.AddStructure(new Lamp(), position);
                    break;
                case StructureType.SolarPanel:
                    _map.AddStructure(new SolarPanel(), position);
                    break;
                case StructureType.WoodPlateform:
                    _map.AddStructure(new WoodPlateform(), position);
                    break;
                case StructureType.Ladder:
                    _map.AddStructure(new Ladder(), position);
                    break;
                case StructureType.Door:
                    _map.AddStructure(new Door(), position);
                    break;
                case StructureType.Limit:
                    _map.AddStructure(new Limit(), position);
                    break;
            }
        }
    }

    public void AddElement(InputAction.CallbackContext contex)
    {
        if (contex.performed)
        {
            float action = contex.ReadValue<float>();

            Vector3Int pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y);
            AddStructure(pos);
        }
    }

    public void RemoveElement(InputAction.CallbackContext contex)
    {
        if (contex.performed)
        {
            float action = contex.ReadValue<float>();
            this.type = StructureType.NONE;
        }
    }


    public bool SelectStructure(StructureType type)
    {
        bool isSelect = false;
        if (this.type != type)
        {
            this.type = type;
            isSelect = true;
         }
        else
        {
            this.type = StructureType.NONE;
        }
        return isSelect;
    }
    #endregion
}
