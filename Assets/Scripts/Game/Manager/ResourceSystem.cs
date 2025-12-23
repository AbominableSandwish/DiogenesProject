using UnityEngine;

class ResourceSystem : MonoBehaviour
{
    [SerializeField] private int _food, _wood, _stone, _copper;
    public int Food { get => _food; set => _food = value; }
    public int Wood { get => _wood; set => _wood = value; }
    public int Stone { get => _stone; set => _stone = value; }
    public int Copper { get => _copper; set => _copper = value; }
}