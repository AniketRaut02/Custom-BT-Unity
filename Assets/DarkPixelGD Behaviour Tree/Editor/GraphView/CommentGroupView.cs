using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace DarkPixelGD.BehaviourTree
{


    public class CommentGroupView : Group
    {
        public BTGroupData data;
        private BehaviorTree tree;

        public CommentGroupView(BTGroupData data, BehaviorTree tree)
        {
            this.data = data;
            this.tree = tree;
            this.title = data.title;
            this.SetPosition(data.position);

            // Save the title when the user renames the group
            this.RegisterCallback<FocusOutEvent>(e =>
            {
                if (this.data.title != this.title)
                {
                    this.data.title = this.title;
                    EditorUtility.SetDirty(tree);
                }
            });
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            data.position = newPos;
            EditorUtility.SetDirty(tree);
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            base.OnElementsAdded(elements);
            foreach (var element in elements)
            {
                if (element is NodeView nodeView && !data.nodeGuids.Contains(nodeView.node.guid))
                {
                    data.nodeGuids.Add(nodeView.node.guid);
                }
            }
            EditorUtility.SetDirty(tree);
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            base.OnElementsRemoved(elements);
            foreach (var element in elements)
            {
                if (element is NodeView nodeView)
                {
                    data.nodeGuids.Remove(nodeView.node.guid);
                }
            }
            EditorUtility.SetDirty(tree);
        }
    }
}