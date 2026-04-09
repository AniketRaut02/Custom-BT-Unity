using UnityEngine;


namespace DarkPixelGD.BehaviourTree
{
    public class InverterNode : DecoratorNode
    {
        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new InverterRuntime(bb, owner);
        }
    }

    public class InverterRuntime : BTNodeRuntime
    {
        public BTNodeRuntime child;

        public InverterRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
        }

        protected override BTStatus OnUpdate()
        {
            if (child == null)
                return BTStatus.Failure;

            var status = child.Tick();

            switch (status)
            {
                case BTStatus.Success:
                    return BTStatus.Failure;

                case BTStatus.Failure:
                    return BTStatus.Success;

                case BTStatus.Running:
                    return BTStatus.Running;
            }

            return BTStatus.Failure;
        }
    }
}