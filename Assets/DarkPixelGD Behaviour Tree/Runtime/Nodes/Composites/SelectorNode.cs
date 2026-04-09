using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;


namespace DarkPixelGD.BehaviourTree
{


    public class SelectorNode : CompositeNode
    {
        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {

            return new SelectorRuntime(bb, owner);
        }
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;

            container.Add(new Label("Selector Node")
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            });

            container.Add(new Label("Executes children until one succeeds."));

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
                    var childLabel = new Label($"• {child.name}");
                    childLabel.style.marginLeft = 10;
                    container.Add(childLabel);
                }
            }

            return container;
        }
    }

    public class SelectorRuntime : CompositeRuntime
    {

        public SelectorRuntime(Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
        }

        protected override BTStatus OnUpdate()
        {
            foreach (var child in children)
            {
                var status = child.Tick();

                if (status == BTStatus.Success)
                    return BTStatus.Success;

                if (status == BTStatus.Running)
                    return BTStatus.Running;
            }

            return BTStatus.Failure;
        }
    }
}