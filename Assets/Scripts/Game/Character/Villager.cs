/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

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
    private System.Action _onMoveFinishedCallback;

    private void Awake()
    {
        agent = UnityResolver.Resolve(agent, this, nameof(Agent));
    }

    public void MoveTo(Vector3Int target, System.Action onFinished = null)
    {
        _onMoveFinishedCallback = onFinished;


        currentState = State.Moving;
        agent.MoveTo(target, OnMoveFinished);
    }

    private void OnMoveFinished()
    {

        currentState = State.Idle;

        _onMoveFinishedCallback?.Invoke();
        _onMoveFinishedCallback = null;
    }

    public void SetPosition(Vector3Int position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}