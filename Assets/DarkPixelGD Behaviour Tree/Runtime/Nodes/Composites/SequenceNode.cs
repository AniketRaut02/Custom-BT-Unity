using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;


namespace DarkPixelGD.BehaviourTree
{


    public class SequenceNode : CompositeNode
    {
        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new SequenceRuntime(bb, owner);
        }
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.Add(new Label("Sequence Node")
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            });

            container.Add(new Label("Executes children in order until one fails."));

            container.Add(new Label("Children")
            {
                style =
        {
            unityFontStyleAndWeight = FontStyle.Bold,
            marginTop = 6
        }
            });

            if (children == null || children.Count == 0)
            {
                container.Add(new Label("None"));
            }
            else
            {
                foreach (var child in children)
                {
                    var label = new Label($"• {child.name}");
                    label.style.marginLeft = 10;
                    container.Add(label);
                }
            }

            return container;
        }
    }

    public class SequenceRuntime : CompositeRuntime
    {
        private int current;

        public SequenceRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
        }

        protected override void OnStart()
        {
            current = 0;
        }

        protected override BTStatus OnUpdate()
        {
            while (current < children.Count)
            {
                var status = children[current].Tick();

                if (status == BTStatus.Running)
                    return BTStatus.Running;

                if (status == BTStatus.Failure)
                    return BTStatus.Failure;

                current++;
            }

            return BTStatus.Success;
        }
    }
}
