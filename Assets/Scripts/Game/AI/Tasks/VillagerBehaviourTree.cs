
using System.Collections.Generic;
using UnityEngine;

namespace BT
{

    public class VillagerBehaviourTree : MonoBehaviour
    {
        [SerializeField] private VillagerWorker worker;

        private Node root;
        private VillagerBlackboard blackboard;

        private void Awake()
        {
            if (worker == null)
                worker = GetComponent<VillagerWorker>();

            blackboard = new VillagerBlackboard(worker);
            root = BuildTree();

            FindAnyObjectByType<AIManager>()?.Register(this);
        }
        private Node BuildTree()
        {
            return new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new ConditionNode(() => blackboard.HasPreviousTask),
                new ActionNode(ResumePreviousTask)
            }),

            new Sequence(new List<Node>
            {
                new ActionNode(FindNewTask),
                new ActionNode(GoToTask)
            }),

            new ActionNode(Idle)
        });
        }

        private NodeState ResumePreviousTask()
        {
            blackboard.CurrentTask = blackboard.PreviousTask;
            blackboard.PreviousTask = null;
            return NodeState.Success;
        }

        private NodeState FindNewTask()
        {
            // À brancher avec ton TaskManager
            return NodeState.Failure;
        }

        private NodeState GoToTask()
        {
            if (!blackboard.HasCurrentTask)
                return NodeState.Failure;

            // À brancher avec ton pathfinding / worker
            return NodeState.Running;
        }

        private NodeState Idle()
        {
            // Le villageois attend ou cherche plus tard
            return NodeState.Running;
        }

        public void Tick()
        {
            root?.Evaluate();
        }

        private void OnDestroy()
        {
            FindAnyObjectByType<AIManager>()?.Unregister(this);
        }
    }
}