using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace DarkPixelGD.BehaviourTree
{


    public class BehaviorTreeEditor : EditorWindow
    {
        private BTGraphView graphView;
        private BehaviorTree tree;
        private bool wasPlaying;

        private VisualElement inspectorView;
        private TwoPaneSplitView splitView;

        [MenuItem("Tools/BT/Behavior Tree Editor")]
        public static void Open()
        {
            var window = GetWindow<BehaviorTreeEditor>();
            window.titleContent = new GUIContent("Behavior Tree");
        }

        private void OnEnable()
        {
            // Subscribe ONLY once here
            EditorApplication.update += OnEditorUpdate;

        }

        public void CreateGUI()
        {
            CreateGraphView();
            OnSelectionChange();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }
        private void OnEditorUpdate()
        {
            if (graphView == null) return;

            // Detect the exact moment we stop playing
            if (wasPlaying && !Application.isPlaying)
            {
                graphView.ClearNodeStates(); // Set all nodes back to Gray
            }
            wasPlaying = Application.isPlaying;

            if (Application.isPlaying)
            {
                graphView.UpdateNodeStates();
                graphView.UpdateEdgeStates();
            }
        }
        private void Update()
        {
            if (Application.isPlaying)
            {
                graphView.UpdateNodeStates();
                graphView.UpdateEdgeStates(); // 🔥 ADD THIS
            }
        }
        private void CreateGraphView()
        {
            rootVisualElement.Clear();

            splitView = new TwoPaneSplitView(1, 300, TwoPaneSplitViewOrientation.Horizontal);

            // LEFT → Graph
            graphView = new BTGraphView(this);
            graphView.style.flexGrow = 1;
            splitView.Add(graphView);

            // RIGHT → Inspector
            inspectorView = new VisualElement();
            inspectorView.style.paddingLeft = 10;
            inspectorView.style.paddingTop = 10;
            inspectorView.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            splitView.Add(inspectorView);

            rootVisualElement.Add(splitView);

            graphView.OnNodeSelected = OnNodeSelected;

            // Rebind tree
            if (tree != null)
            {
                graphView.SetTree(tree);
                graphView.PopulateView(tree);
            }
        }
        private void OnNodeSelected(NodeView nodeView)
        {
            inspectorView.Clear();

            if (nodeView == null || nodeView.node == null)
            {
                inspectorView.Add(new Label("No node selected"));
                return;
            }

            var node = nodeView.node;
           

            // DEFAULT: USE NODE'S OWN INSPECTOR
            var ui = node.CreateInspectorGUI();

            if (ui != null)
                inspectorView.Add(ui);
        }
        private void OnSelectionChange()
        {
            // 1. SAFETY LOCK: Don't do anything if the UI isn't ready
            if (graphView == null) return;

            BehaviorTree treeToDisplay = null;

            // 2. Check if we clicked a GameObject in the scene hierarchy
            if (Selection.activeGameObject != null)
            {
                var agent = Selection.activeGameObject.GetComponent<BehaviorTreeAgent>(); // Ensure this uses your Agent class name
                if (agent != null)
                {
                    // If playing, grab the live clone. If stopped, grab the base asset.
                    treeToDisplay = Application.isPlaying ? agent.RuntimeTree : agent.treeAsset;
                }
            }

            // 3. If no GameObject was selected, check if we clicked a raw Asset in the project folder
            if (treeToDisplay == null && Selection.activeObject is BehaviorTree selectedAsset)
            {
                treeToDisplay = selectedAsset;
            }

            // 4. Update the Graph View if we found a valid tree
            if (treeToDisplay != null)
            {
                // Only rebuild if we are actually looking at a different tree
                if (tree != treeToDisplay)
                {
                    tree = treeToDisplay;
                    graphView.SetTree(tree);
                    graphView.PopulateView(tree);
                }
            }
        }
    }
}