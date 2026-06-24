using UnityEngine;

namespace BT
{
    public class VillagerBlackboard
    {
        public VillagerWorker Worker { get; }
        public VillagerTask CurrentTask { get; set; }
        public VillagerTask PreviousTask { get; set; }
        public Vector3Int TargetPosition { get; set; }

        public VillagerBlackboard(VillagerWorker worker)
        {
            Worker = worker;
        }

        public bool HasCurrentTask => CurrentTask != null;
        public bool HasPreviousTask => PreviousTask != null;
    }
}