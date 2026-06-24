using System;

namespace BT
{
    public class ActionNode : Node
    {
        private readonly Func<NodeState> action;

        public ActionNode(Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Evaluate()
        {
            return action.Invoke();
        }
    }
}