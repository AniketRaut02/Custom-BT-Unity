using UnityEngine;
namespace DarkPixelGD.BehaviourTree
{


    public class FailureNode : BTNode
    {
        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new FailureRuntime(bb, owner);
        }
    }

    public class FailureRuntime : BTNodeRuntime
    {
        public FailureRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
        }

        protected override BTStatus OnUpdate()
        {
            return BTStatus.Failure; // always fail
        }
    }
}