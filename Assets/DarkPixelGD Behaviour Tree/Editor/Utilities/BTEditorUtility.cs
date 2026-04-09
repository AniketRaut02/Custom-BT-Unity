using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{


    public static class BTEditorUtility
    {
        public static Blackboard GetActiveBlackboard()
        {
            if (!Application.isPlaying)
                return null;

            var runner = Object.FindObjectOfType<BehaviorTreeAgent>();

            if (runner == null)
                return null;

            return runner.Blackboard;
        }

        // Blackboard Asset (Edit Mode + Play Mode)
        public static BlackboardAsset GetActiveBlackboardAsset()
        {
            var runner = Object.FindObjectOfType<BehaviorTreeAgent>();

            if (runner == null)
                return null;

            return runner.blackboardAsset;
        }
    }
}
