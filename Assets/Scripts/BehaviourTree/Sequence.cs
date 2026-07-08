using System.Collections.Generic;

namespace BT
{
    public class Sequence : Node
    {
        private readonly List<Node> children;

        public Sequence(List<Node> children)
        {
            this.children = children;
        }

        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                NodeState result = child.Evaluate();

                if (result == NodeState.Failure)
                    return NodeState.Failure;

                if (result == NodeState.Running)
                    return NodeState.Running;
            }

            return NodeState.Success;
        }
    }
}