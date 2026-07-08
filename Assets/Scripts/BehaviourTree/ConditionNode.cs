using System;

namespace BT
{
    public class ConditionNode : Node
    {
        private readonly Func<bool> condition;

        public ConditionNode(Func<bool> condition)
        {
            this.condition = condition;
        }

        public override NodeState Evaluate()
        {
            return condition.Invoke()
                ? NodeState.Success
                : NodeState.Failure;
        }
    }
}