using UnityEngine;


namespace DarkPixelGD.BehaviourTree
{


    public class RootNode : BTNode
    {
        public BTNode child;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new RootRuntime(bb, owner);
        }
    }

    public class RootRuntime : BTNodeRuntime
    {
        public BTNodeRuntime child;
        public RootRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
        }
        protected override BTStatus OnUpdate()
        {
            if (child == null)
                return BTStatus.Failure;

            return child.Tick();
        }
    }
}