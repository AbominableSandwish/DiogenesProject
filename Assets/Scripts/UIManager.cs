using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    protected static UIManager _instance = null;
    [SerializeField] private TextMeshProUGUI _elementText;

    public static UIManager Instance { get => _instance; protected set => _instance = value; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            Instance = this;
            DontDestroyOnLoad(this);
    }
    public static void SetTextElement(string nameElement)
    {
        Instance._elementText.text = nameElement;
    }
}
