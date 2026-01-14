using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Structure;

public class Villager : MonoBehaviour
{
    public Vector3Int currentGridPos;
    public Vector3Int targetGridPos;
    public float moveSpeed = 2f;
    public enum State { Idle, Moving, Working }
    public State currentState = State.Idle;

    private Pathfinder pathfinder;
    private Queue<Vector3Int> pathQueue = new();

    private MapManager map;

    private void Awake()
    {
        map = FindAnyObjectByType<MapManager>();
        pathfinder = FindAnyObjectByType<Pathfinder>();
    }

    private void Update()
    {
        //if (currentState == State.Moving)
        //{
        //    Vector3 targetPos = map.GetWorldPosition(targetGridPos);
        //    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        //    if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        //    {
        //        currentGridPos = targetGridPos;
        //        currentState = State.Idle;
        //    }
        //}
    }

    public void SetPosition(Vector3Int position)
    {
        transform.position = position;
        currentGridPos = position;
    }

    public bool CanMoveTo(Vector3Int destination)
    {
        Vector3Int diff = destination - currentGridPos;

        // Mouvement horizontal normal
        if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1 && diff.z == 0)
            return true;

        // Mouvement vertical uniquement si échelle ou escalier
        if (Mathf.Abs(diff.z) == 1)
        {
            Structure s = map.GetStructure(currentGridPos, Structure.StructureMap.Basic);
            Structure s2 = map.GetStructure(destination, Structure.StructureMap.Basic);

            bool hasLadderOrStair =
                (s != null && (s.Type == StructureType.Ladder)) ||
                (s2 != null && (s2.Type == StructureType.Ladder));

            return hasLadderOrStair;
        }

        return false;
    }

    public void MoveTo(Vector3Int destination)
    {
        StopAllCoroutines();
        currentState = State.Moving;
        List<Vector3Int> path = pathfinder.FindPath(currentGridPos, destination);

        if (path == null)
        {
            Debug.LogWarning($"{name} ne trouve pas de chemin vers {destination}");
            return;
        }

        pathQueue = new Queue<Vector3Int>(path);
        
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        while (pathQueue.Count > 0)
        {
            Vector3Int next = pathQueue.Dequeue();
            Vector3 worldTarget = map.GetWorldPosition(next);

            while (Vector3.Distance(transform.position, worldTarget) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, worldTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentGridPos = next;            
        }
        currentState = State.Idle;
    }

}