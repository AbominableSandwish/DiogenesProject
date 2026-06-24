using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField] private float tickRate = 0.25f;

        private readonly List<VillagerBehaviourTree> trees = new();
        private float nextTick;

        public void Register(VillagerBehaviourTree tree)
        {
            if (!trees.Contains(tree))
                trees.Add(tree);
        }

        public void Unregister(VillagerBehaviourTree tree)
        {
            trees.Remove(tree);
        }

        private void Update()
        {
            if (Time.time < nextTick)
                return;

            nextTick = Time.time + tickRate;

            foreach (VillagerBehaviourTree tree in trees)
                tree.Tick();
        }
    }
}