using UnityEngine;

public class Element: MonoBehaviour
{
    [SerializeField] protected string _name = "";
    [SerializeField] protected float _temperature = 34;

    private void OnMouseOver()
    {
        UIManager.SetTextElement(_name);
    }

    private void OnMouseExit()
    {
        UIManager.SetTextElement("");
    }
}
