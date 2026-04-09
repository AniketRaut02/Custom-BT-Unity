using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{


    public abstract class DecoratorNode : BTNode
    {
        public BTNode child;
    }
    public abstract class DecoratorRuntime : BTNodeRuntime
    {
        public BTNode children;
        public DecoratorRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {

        }
    }
}