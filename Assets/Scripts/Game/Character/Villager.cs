using UnityEngine;

public class Villager : MonoBehaviour
{
    [SerializeField] private Agent agent;

    public enum State
    {
        Idle,
        Moving
    }

    public State currentState = State.Idle;

    private void Awake()
    {
        agent = UnityResolver.Resolve(agent, this, nameof(Agent));
    }

    public void MoveTo(Vector3Int target)
    {
        currentState = State.Moving;
        agent.MoveTo(target, OnMoveFinished);
    }

    private void OnMoveFinished()
    {
        currentState = State.Idle;
    }

    public void SetPosition(Vector3Int position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}