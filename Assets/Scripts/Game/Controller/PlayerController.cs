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
                    MapManager.AddStructure<Coil>(position);
                    break;
                case StructureType.Generator:
                    MapManager.AddStructure<Generator>(position);
                    break;
                case StructureType.Engine:
                    MapManager.AddStructure<Engine>(position);
                    break;
                case StructureType.Storage:
                    MapManager.AddStructure<Storage>(position);
                    break;
                case StructureType.Lamp:
                    MapManager.AddStructure<Lamp>(position);
                    break;
                case StructureType.SolarPanel:
                    MapManager.AddStructure<SolarPanel>(position);
                    break;
                case StructureType.WoodPlateform:
                    MapManager.AddStructure<WoodPlateform>(position);
                    break;
                case StructureType.Ladder:
                    MapManager.AddStructure<Ladder>(position);
                    break;
                case StructureType.Door:
                    MapManager.AddStructure<Door>(position);
                    break;
                case StructureType.Limit:
                    MapManager.AddStructure<Limit>(position);
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
