/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;

public class Villager : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private Agent agent;

    public enum State
    {
        Idle,
        Moving
    }

    protected void Update()
    {

    }

    public State currentState = State.Idle;
    private System.Action _onMoveFinishedCallback;

    protected void Awake()
    {
        animator = UnityResolver.Resolve(animator, this, nameof(Animator));
        agent = UnityResolver.Resolve(agent, this, nameof(Agent));
        animator.SetFloat("Velocity", 0.0f);
    }

    public void MoveTo(Vector3Int target, System.Action onFinished = null)
    {
        _onMoveFinishedCallback = onFinished;

        animator.SetFloat("Velocity", 1.0f);
        currentState = State.Moving;
        agent.MoveTo(target, OnMoveFinished);
    }

    private void OnMoveFinished()
    {
        animator.SetFloat("Velocity", 0.0f);
        currentState = State.Idle;

        _onMoveFinishedCallback?.Invoke();
        _onMoveFinishedCallback = null;
    }

    public void SetPosition(Vector3Int position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}