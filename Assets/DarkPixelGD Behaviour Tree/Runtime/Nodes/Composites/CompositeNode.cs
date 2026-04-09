using UnityEngine;
using System.Collections.Generic;

namespace DarkPixelGD.BehaviourTree
{


    public abstract class CompositeNode : BTNode
    {
        public List<BTNode> children = new();
    }

    public abstract class CompositeRuntime : BTNodeRuntime
    {
        public List<BTNodeRuntime> children = new List<BTNodeRuntime>();
        public CompositeRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {

        }
        public override void ForceStop()
        {
            base.ForceStop();

            foreach (var child in children)
            {
                child.ForceStop();
            }
        }

    }
}