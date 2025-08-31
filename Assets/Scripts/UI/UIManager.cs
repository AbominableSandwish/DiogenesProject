using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    protected static UIManager _instance = null;
    [SerializeField] private TextMeshProUGUI _elementText;
    private PlayerController _player;

    [SerializeField] private List<Image> _images;

    public static UIManager Instance { get => _instance; protected set => _instance = value; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        _player = FindFirstObjectByType<PlayerController>();
    }
    public static void SetTextElement(string nameElement)
    {
        Instance._elementText.text = nameElement;
    }
    
    public void SelectStructure(int id)
    {
        foreach(Image image in _images)
        {
            image.color = Color.grey;
        }

        bool isSelect = this._player.SelectStructure(id);
        if (isSelect)
        {
            _images[id].color = Color.green;
        }
        else
        {
            _images[id].color = Color.grey;
        }
        
    }
}
