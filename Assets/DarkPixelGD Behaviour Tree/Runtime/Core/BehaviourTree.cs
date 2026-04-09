using UnityEngine;
using System.Collections.Generic;
using System;

namespace DarkPixelGD.BehaviourTree
{

    [CreateAssetMenu(menuName = "BT/Behavior Tree")]
    public class BehaviorTree : ScriptableObject
    {
        public RootNode rootNode;
        public BTNodeRuntime rootRuntime;
        public List<BTNode> nodes = new();
        public List<BTGroupData> groups = new();

        public static BehaviorTree ActiveTree;

        public BTNodeRuntime CreateRuntimeTree(Blackboard blackboard, GameObject owner)
        {
            return rootNode.CreateRuntime(blackboard, owner);
        }
        public void Initialize(Blackboard blackboard, GameObject owner)
        {
            if (rootNode == null)
            {
                Debug.LogError("BehaviorTree has no RootNode!");
                return;
            }
            ActiveTree = this;
            rootRuntime = BuildNode(rootNode, blackboard, owner);
        }
        private BTNodeRuntime BuildNode(BTNode node, Blackboard bb, GameObject owner)
        {
            if (node == null)
                return null;

            //  CREATE ONCE
            var runtime = node.CreateRuntime(bb, owner);

            // STORE IT (VERY IMPORTANT)
            node.runtime = runtime;

            // ===== ROOT =====
            if (node is RootNode rootNode)
            {
                if (runtime is RootRuntime rootRuntime && rootNode.child != null)
                {
                    rootRuntime.child = BuildNode(rootNode.child, bb, owner);
                }
            }

            // ===== COMPOSITE =====
            else if (node is CompositeNode compNode)
            {
                if (runtime is CompositeRuntime compRuntime)
                {
                    foreach (var child in compNode.children)
                    {
                        var childRuntime = BuildNode(child, bb, owner);
                        childRuntime.parent = runtime;
                        compRuntime.children.Add(childRuntime);
                    }
                }
            }
            // ===== DECORATOR =====
            else if (node is DecoratorNode decoratorNode)
            {
                var childRuntime = BuildNode(decoratorNode.child, bb, owner);
                childRuntime.parent = runtime;
                if (runtime is InverterRuntime inverter)
                {
                    inverter.child = childRuntime;
                }
                else if (runtime is RepeaterRuntime repeater)
                {
                    repeater.SetChild(childRuntime);
                }
                else if (runtime is ConditionRuntime condition)
                {
                    condition.SetChild(childRuntime);
                }
            }

            return runtime;
        }

        public void ResetTree()
        {
            if (rootRuntime == null)
                return;

            ResetNodeRecursive(rootRuntime);
        }

        private void ResetNodeRecursive(BTNodeRuntime node)
        {
            if (node == null)
                return;

            node.ResetNode();

            //root
            if (node is RootRuntime root && root.child != null)
            {
                ResetNodeRecursive(root.child);
            }
            // Composite
            if (node is CompositeRuntime comp)
            {
                foreach (var child in comp.children)
                {
                    ResetNodeRecursive(child);
                }
            }

            // Decorator
            if (node is InverterRuntime inverter && inverter.child != null)
            {
                ResetNodeRecursive(inverter.child);
            }

            if (node is RepeaterRuntime repeater)
            {
                ResetNodeRecursive(repeater.child);
            }

            if (node is ConditionRuntime condition)
            {
                ResetNodeRecursive(condition.child);
            }
        }
        public void ResetExecutionState()
        {
            ResetExecutionRecursive(rootRuntime);
        }

        private void ResetExecutionRecursive(BTNodeRuntime node)
        {
            if (node == null) return;

            node.executedThisFrame = false;
            node.visualState = BTNodeRuntime.BTVisualState.Inactive;

            //Root
            if (node is RootRuntime root && root.child != null)
            {
                ResetExecutionRecursive(root.child);
            }
            // Composite
            if (node is CompositeRuntime comp)
            {
                foreach (var child in comp.children)
                    ResetExecutionRecursive(child);
            }
            // Decorators
            if (node is InverterRuntime inverter && inverter.child != null)
                ResetExecutionRecursive(inverter.child);

            if (node is RepeaterRuntime repeater)
                ResetExecutionRecursive(repeater.child);

            if (node is ConditionRuntime condition && condition.child != null)
                ResetExecutionRecursive(condition.child);
        }
        public void RequestAbort(BTNodeRuntime source, AbortType type)
        {
            if (type == AbortType.None)
                return;

            Debug.Log($"[ABORT] Requested by {source}");

            switch (type)
            {
                case AbortType.Self:
                    AbortSelf(source);
                    break;

                case AbortType.LowerPriority:
                    AbortLowerPriority(source);
                    break;

                case AbortType.Both:
                    AbortSelf(source);
                    AbortLowerPriority(source);
                    break;
            }
        }

        private void AbortLowerPriority(BTNodeRuntime source)
        {
            var current = source;

            while (current.parent != null)
            {
                var parent = current.parent;

                if (parent is CompositeRuntime composite)
                {
                    int index = composite.children.IndexOf(current);

                    if (index >= 0)
                    {
                        // Abort ALL lower priority siblings
                        for (int i = index + 1; i < composite.children.Count; i++)
                        {
                            composite.children[i].ForceStop();
                        }
                    }
                }

                current = parent;
            }
        }

        private void AbortSelf(BTNodeRuntime node)
        {
            node.ForceStop();

            // Also stop all children recursively
            if (node is CompositeRuntime composite)
            {
                foreach (var child in composite.children)
                {
                    child.ForceStop();
                }
            }
        }


        private void ResetLowerPriority(BTNodeRuntime source)
        {
            var parent = source.parent;

            // Traverse upward until we find a composite
            while (parent != null && parent is not CompositeRuntime)
            {
                parent = parent.parent;
            }

            if (parent is CompositeRuntime composite)
            {
                AbortLowerPriorityInComposite(composite, source);
            }
        }
        private void AbortLowerPriorityInComposite(CompositeRuntime composite, BTNodeRuntime source)
        {
            int index = composite.children.IndexOf(source);

            if (index == -1)
                return;

            // Abort ALL lower-priority siblings
            for (int i = index + 1; i < composite.children.Count; i++)
            {
                ResetNodeRecursive(composite.children[i]);
            }
        }
        private List<BTNodeRuntime> GetRunningPath(BTNodeRuntime node)
        {
            List<BTNodeRuntime> path = new();

            while (node != null)
            {
                path.Add(node);
                node = node.parent;
            }

            return path;
        }


    }


    //For the comment box
    [System.Serializable]
    public class BTGroupData
    {
        public string id;
        public string title;
        public Rect position;
        public List<string> nodeGuids = new List<string>();
    }
}