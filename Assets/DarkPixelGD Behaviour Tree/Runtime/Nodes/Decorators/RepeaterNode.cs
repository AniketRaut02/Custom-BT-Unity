using UnityEngine;


namespace DarkPixelGD.BehaviourTree
{


    public class RepeaterNode : DecoratorNode
    {
        public int repeatCount = 3; // -1 = infinite

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new RepeaterRuntime(bb, owner, repeatCount);
        }
    }
    public class RepeaterRuntime : BTNodeRuntime
    {
        public BTNodeRuntime child;
        private int repeatCount;
        private int currentCount;

        public RepeaterRuntime(Blackboard bb, GameObject owner, int repeatCount)
            : base(bb, owner)
        {
            this.repeatCount = repeatCount;
        }

        public void SetChild(BTNodeRuntime child)
        {
            this.child = child;
        }

        protected override void OnStart()
        {
            currentCount = 0;
        }

        protected override BTStatus OnUpdate()
        {
            if (child == null)
                return BTStatus.Failure;

            var status = child.Tick();

            if (status == BTStatus.Running)
                return BTStatus.Running;

            if (status == BTStatus.Failure)
                return BTStatus.Failure;

            // Success → repeat
            currentCount++;

            if (repeatCount == -1 || currentCount < repeatCount)
                return BTStatus.Running;

            return BTStatus.Success;
        }
    }
}