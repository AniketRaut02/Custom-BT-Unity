using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace DarkPixelGD.BehaviourTree
{


    public class BTGraphView : GraphView
    {
        private BTSearchWindow searchWindow;
        private EditorWindow editorWindow;
        private BehaviorTree tree;
        private bool isLoading = false;
        private int lastEdgeCount = 0;

        public System.Action<NodeView> OnNodeSelected;
        public BTGraphView(EditorWindow window)
        {
            editorWindow = window;

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            searchWindow = ScriptableObject.CreateInstance<BTSearchWindow>();
            searchWindow.Initialize(this, editorWindow);

            nodeCreationRequest = context =>
            {
                if (tree == null)
                {
                    Debug.LogWarning("Select a BehaviorTree first!");
                    return;
                }

                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
            };

            graphViewChanged += OnGraphViewChanged;
            schedule.Execute(CheckGraphChanges).Every(100);

        }
        public void SetTree(BehaviorTree tree)
        {
            this.tree = tree;

            if (tree == null)
                return;

            //  Ensure root node exists
            if (tree.rootNode == null)
            {
                var root = ScriptableObject.CreateInstance<RootNode>();
                root.name = "Root";

                AssetDatabase.AddObjectToAsset(root, tree);

                tree.rootNode = root;
                //tree.nodes.Add(root);

                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }
        }
        public NodeView CreateNode(System.Type type, Vector2 position)
        {
            BTNode node = ScriptableObject.CreateInstance(type) as BTNode;

            if (node == null)
                return null;

            node.guid = System.Guid.NewGuid().ToString();
            node.name = type.Name;

            AssetDatabase.AddObjectToAsset(node, tree);

            tree.nodes.Add(node);

            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();

            var nodeView = new NodeView(node);
            nodeView.OnNodeSelected = (n) =>
            {
                OnNodeSelected?.Invoke(n);
            };
            nodeView.SetPosition(new Rect(position, new Vector2(200, 150)));

            AddElement(nodeView);

            return nodeView;
        }
        public void PopulateView(BehaviorTree tree)
        {
            isLoading = true;

            graphElements.ForEach(RemoveElement);

            if (tree == null)
            {
                isLoading = false;
                return;
            }

            Dictionary<BTNode, NodeView> nodeViews = new();
            //Set Root node
            if (tree.rootNode != null)
            {
                var rootView = CreateNodeView(tree.rootNode);
                nodeViews[tree.rootNode] = rootView;
            }
            //set other nodes
            foreach (var node in tree.nodes)
            {
                var nodeView = CreateNodeView(node);
                nodeView.OnNodeSelected = (n) =>
                {
                    OnNodeSelected?.Invoke(n);
                };
                nodeViews[node] = nodeView;
            }

            // ===== REBUILD GROUPS =====
            foreach (var groupData in tree.groups)
            {
                var groupView = new CommentGroupView(groupData, tree);
                AddElement(groupView);

                // Put nodes back into the group
                foreach (var guid in groupData.nodeGuids)
                {
                    var node = tree.nodes.FirstOrDefault(n => n.guid == guid);
                    if (node != null && nodeViews.ContainsKey(node))
                    {
                        groupView.AddElement(nodeViews[node]);
                    }
                }
            }
            // Rebuild edges

            // ROOT CONNECTION
            if (tree.rootNode != null && tree.rootNode.child != null)
            {
                var parentView = nodeViews[tree.rootNode];
                var childView = nodeViews[tree.rootNode.child];

                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            }

            foreach (var node in tree.nodes)
            {
                if (node is CompositeNode composite)
                {
                    foreach (var child in composite.children)
                    {
                        if (node == child) continue;

                        var parentView = nodeViews[node];
                        var childView = nodeViews[child];

                        var edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }
                }
            }
            // ===== DECORATOR EDGE REBUILD =====
            foreach (var node in tree.nodes)
            {
                if (node is DecoratorNode decorator && decorator.child != null)
                {
                    if (nodeViews.TryGetValue(decorator, out var parentView) &&
                        nodeViews.TryGetValue(decorator.child, out var childView))
                    {
                        var edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }
                }
            }
            UpdateExecutionOrder();
            isLoading = false;
        }
        public NodeView CreateNodeView(BTNode node)
        {
            var nodeView = new NodeView(node);
            nodeView.OnNodeSelected = (n) =>
            {
                OnNodeSelected?.Invoke(n);
            };

            nodeView.SetPosition(new Rect(node.position, new Vector2(200, 150)));

            AddElement(nodeView);

            return nodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (isLoading)
                return change;
            if (change.movedElements != null)
            {
                RebuildTreeFromGraph(); //IMPORTANT
            }

            // EDGE CREATION (THIS WAS MISSING)
            if (change.edgesToCreate != null && change.edgesToCreate.Count > 0)
            {
                EditorApplication.delayCall += () =>
                {
                    RebuildTreeFromGraph();
                    ForceRemoveRootFromChildren();
                    CleanInvalidChildren();
                };
            }

            // Handle node deletion
            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is NodeView nodeView)
                    {
                        if (tree.nodes.Contains(nodeView.node))
                        {
                            tree.nodes.Remove(nodeView.node);
                            UnityEngine.Object.DestroyImmediate(nodeView.node, true);
                            EditorUtility.SetDirty(tree);
                        }
                    }

                    // Edge deletion
                    if (element is Edge)
                    {
                        EditorApplication.delayCall += () =>
                        {
                            RebuildTreeFromGraph();
                            ForceRemoveRootFromChildren();
                            CleanInvalidChildren();
                        };
                    }
                    // ===== FOR DELETING GROUPS =====
                    if (element is CommentGroupView groupView)
                    {
                        tree.groups.Remove(groupView.data);
                        EditorUtility.SetDirty(tree);
                    }
                }
            }

            return change;
        }
        private void RebuildTreeFromGraph()
        {
            if (tree == null) return;

            //  STEP 1: Reset everything cleanly
            foreach (var node in tree.nodes)
            {
                if (node is CompositeNode comp)
                {
                    comp.children = new List<BTNode>();
                    EditorUtility.SetDirty(comp);
                }

                if (node is RootNode root)
                {
                    root.child = null;
                    EditorUtility.SetDirty(root);
                }
                // Clear decorator child
                if (node is DecoratorNode decorator)
                {
                    decorator.child = null;
                    EditorUtility.SetDirty(decorator);
                }
            }


            //  STEP 2: Process ONLY valid edges
            foreach (var edge in edges.ToList())
            {
                if (edge.input == null || edge.output == null)
                    continue;

                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;

                if (parentView == null || childView == null)
                    continue;

                var parentNode = parentView.node;
                var childNode = childView.node;

                //  CRITICAL FIX 1: Prevent self-child
                if (parentNode == childNode)
                    continue;

                //  CRITICAL FIX 2: Root can NEVER be a child
                if (childNode is RootNode)
                    continue;

                //  CRITICAL FIX 3: Only allow parent types
                if (!(parentNode is CompositeNode) &&
                    !(parentNode is RootNode) &&
                    !(parentNode is DecoratorNode))
                    continue;

                //  HANDLE ROOT
                if (parentNode is RootNode root)
                {
                    root.child = childNode;
                    EditorUtility.SetDirty(root);
                    continue;
                }

                //  HANDLE COMPOSITE
                if (parentNode is CompositeNode comp)
                {
                    if (!comp.children.Contains(childNode))
                    {
                        comp.children.Add(childNode);
                    }
                }

                // ===== HANDLE DECORATOR =====

                if (parentNode is DecoratorNode decorator)
                {
                    decorator.child = childNode;
                    EditorUtility.SetDirty(decorator);
                }
            }
            // SORT CHILDREN BY Y POSITION (TOP → BOTTOM)
            foreach (var node in tree.nodes)
            {
                if (node is CompositeNode comp)
                {
                    comp.children.Sort((a, b) =>
                    {
                        return a.position.y.CompareTo(b.position.y);
                    });

                    EditorUtility.SetDirty(comp);
                }
            }
            UpdateExecutionOrder();
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
        private void ForceRemoveRootFromChildren()
        {
            foreach (var node in tree.nodes)
            {
                if (node is CompositeNode comp)
                {
                    comp.children.RemoveAll(child => child is RootNode);
                    EditorUtility.SetDirty(comp);
                }
            }
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                    return;

            // Prevent connecting to same node
            if (startPort.node == port.node)
                    return;

            // Only allow Output → Input
            if (startPort.direction == port.direction)
                    return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        private void CleanInvalidChildren()
        {
            foreach (var node in tree.nodes)
            {
                if (node is CompositeNode comp)
                {
                    comp.children.RemoveAll(child => !tree.nodes.Contains(child));
                    UnityEditor.EditorUtility.SetDirty(comp);
                }
            }
        }


        private void CheckGraphChanges()
        {
            if (isLoading || tree == null)
                return;

            int currentEdgeCount = edges.ToList().Count;

            if (currentEdgeCount != lastEdgeCount)
            {
                lastEdgeCount = currentEdgeCount;

                RebuildTreeFromGraph();
                ForceRemoveRootFromChildren();
                CleanInvalidChildren();
            }
        }
        public void UpdateNodeStates()
        {
            foreach (var element in graphElements)
            {
                if (element is NodeView nodeView)
                {
                    nodeView.UpdateState();
                    nodeView.MarkDirtyRepaint();
                }
            }
        }
        public void ClearNodeStates()
        {
            style.backgroundColor = Color.gray;
        }

        public void UpdateExecutionOrder()
        {
            // Reset all
            foreach (var element in graphElements)
            {
                if (element is NodeView nv)
                {
                    nv.executionIndex = -1;
                    nv.UpdateExecutionOrder();
                }
            }

            if (tree == null || tree.rootNode == null)
                return;

            int counter = 1;

            TraverseAndAssign(tree.rootNode, ref counter);
        }
        private void TraverseAndAssign(BTNode node, ref int counter)
        {
            if (node == null) return;

            var nodeView = GetNodeView(node);
            if (nodeView != null)
            {
                nodeView.executionIndex = counter++;
                nodeView.UpdateExecutionOrder();
            }

            // Root
            if (node is RootNode root && root.child != null)
            {
                TraverseAndAssign(root.child, ref counter);
            }

            // Composite
            if (node is CompositeNode comp)
            {
                foreach (var child in comp.children)
                {
                    TraverseAndAssign(child, ref counter);
                }
            }

            // Decorator
            if (node is DecoratorNode dec && dec.child != null)
            {
                TraverseAndAssign(dec.child, ref counter);
            }
        }
        private NodeView GetNodeView(BTNode node)
        {
            foreach (var element in graphElements)
            {
                if (element is NodeView nv && nv.node == node)
                    return nv;
            }

            return null;
        }
        public void UpdateEdgeStates()
        {
            foreach (var edge in edges)
            {
                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;

                if (parentView?.node?.runtime == null || childView?.node?.runtime == null)
                    continue;

                // EDGE ACTIVE ONLY IF BOTH NODES EXECUTED
                if (parentView.node.runtime.executedThisFrame &&
                    childView.node.runtime.executedThisFrame)
                {
                    edge.edgeControl.inputColor = Color.yellow;
                    edge.edgeControl.outputColor = Color.yellow;
                }
                else
                {
                    edge.edgeControl.inputColor = Color.gray;
                    edge.edgeControl.outputColor = Color.gray;
                }
            }
        }
        public CommentGroupView CreateGroupBlock(string title, Vector2 position)
        {
            var groupData = new BTGroupData
            {
                id = System.Guid.NewGuid().ToString(),
                title = title,
                position = new Rect(position, new Vector2(200, 150))
            };

            tree.groups.Add(groupData);
            EditorUtility.SetDirty(tree);

            var groupView = new CommentGroupView(groupData, tree);
            AddElement(groupView);

            // Automatically add any currently selected nodes into the new group
            foreach (var element in selection)
            {
                if (element is NodeView nodeView)
                {
                    groupView.AddElement(nodeView);
                }
            }

            return groupView;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (selection.Count > 0 && selection.Any(s => s is NodeView))
            {
                evt.menu.AppendAction("Group Selected Nodes", a =>
                {
                    Vector2 mousePos = contentViewContainer.WorldToLocal(a.eventInfo.localMousePosition);
                    CreateGroupBlock("New Group", mousePos);
                });
            }
        }
    }
}

