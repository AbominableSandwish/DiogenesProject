using System.Collections.Generic;

namespace BT
{
    public class Selector : Node
    {
        private readonly List<Node> children;

        public Selector(List<Node> children)
        {
            this.children = children;
        }

        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                NodeState result = child.Evaluate();

                if (result == NodeState.Success)
                    return NodeState.Success;

                if (result == NodeState.Running)
                    return NodeState.Running;
            }

            return NodeState.Failure;
        }
    }
}