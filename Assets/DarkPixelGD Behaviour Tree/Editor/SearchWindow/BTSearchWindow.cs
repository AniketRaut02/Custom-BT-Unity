using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace DarkPixelGD.BehaviourTree
{


    public class BTSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BTGraphView graphView;
        private EditorWindow window;

        public void Initialize(BTGraphView graphView, EditorWindow window)
        {
            this.graphView = graphView;
            this.window = window;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));

            // ===== COMPOSITES =====
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Composite"), 1));

            tree.Add(new SearchTreeEntry(new GUIContent("Sequence"))
            {
                level = 2,
                userData = typeof(SequenceNode)
            });

            tree.Add(new SearchTreeEntry(new GUIContent("Selector"))
            {
                level = 2,
                userData = typeof(SelectorNode)
            });

            // ===== ACTIONS =====
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Action"), 1));

            tree.Add(new SearchTreeEntry(new GUIContent("Wait"))
            {
                level = 2,
                userData = typeof(WaitNode)
            });

            tree.Add(new SearchTreeEntry(new GUIContent("Debug Log"))
            {
                level = 2,
                userData = typeof(DebugLogNode)
            });

            tree.Add(new SearchTreeEntry(new GUIContent("Move Forward"))
            {
                level = 2,
                userData = typeof(MoveForwardNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Force Failure"))
            {
                level = 2,
                userData = typeof(FailureNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Check Distance"))
            {
                level = 2,
                userData = typeof(CheckDistanceNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Face Target"))
            {
                level = 2,
                userData = typeof(FaceTargetNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Move To"))
            {
                level = 2,
                userData = typeof(MoveToNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Check Cooldown"))
            {
                level = 2,
                userData = typeof(CheckCooldownNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Set Cooldown"))
            {
                level = 2,
                userData = typeof(SetCooldownNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Play Animation"))
            {
                level = 2,
                userData = typeof(PlayAnimationNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Get Next Waypoint"))
            {
                level = 2,
                userData = typeof(GetNextWaypointNode)
            });

            //DECORATORS
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Decorator"), 1));

            tree.Add(new SearchTreeEntry(new GUIContent("Inverter"))
            {
                level = 2,
                userData = typeof(InverterNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Repeater"))
            {
                level = 2,
                userData = typeof(RepeaterNode)
            });
            tree.Add(new SearchTreeEntry(new GUIContent("Condition"))
            {
                level = 2,
                userData = typeof(ConditionNode)
            });

            //UTILITY
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Utility"), 1));
            tree.Add(new SearchTreeEntry(new GUIContent("Comment Group"))
            {
                level = 2,
                userData = typeof(UnityEditor.Experimental.GraphView.Group)
            });

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry.userData is not System.Type type)
                return false;

            // Convert screen → window space
            Vector2 mousePosition = context.screenMousePosition;
            mousePosition -= window.position.position;

            // Convert to GraphView space
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

            if (entry.userData == typeof(UnityEditor.Experimental.GraphView.Group))
            {
                graphView.CreateGroupBlock("New Group", graphMousePosition);
                return true;
            }

            graphView.CreateNode(type, graphMousePosition);

            return true;
        }
    }
}