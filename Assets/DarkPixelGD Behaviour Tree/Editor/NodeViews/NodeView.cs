using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DarkPixelGD.BehaviourTree
{


    public class NodeView : Node
    {
        public string GUID;
        public BTNode node;

        public Port input;
        public Port output;

        public int executionIndex = -1;

        public System.Action<NodeView> OnNodeSelected;
        public NodeView(BTNode node)
        {
            this.node = node;

            GUID = node.guid;

            title = ObjectNames.NicifyVariableName(node.GetType().Name.Replace("Node", ""));

            CreatePorts();
            style.backgroundColor = Color.gray;
            this.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    OnNodeSelected?.Invoke(this);
                }
            });

            var orderLabel = new Label();
            orderLabel.name = "execution-order";

            orderLabel.style.position = Position.Absolute;
            orderLabel.style.right = 6;
            orderLabel.style.top = 4;

            orderLabel.style.backgroundColor = new Color(0, 0, 0, 0.6f);
            orderLabel.style.color = Color.white;
            orderLabel.style.paddingLeft = 4;
            orderLabel.style.paddingRight = 4;

            orderLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            style.borderTopLeftRadius = 6;
            style.borderTopRightRadius = 6;
            style.borderBottomLeftRadius = 6;
            style.borderBottomRightRadius = 6;

            style.paddingLeft = 6;
            style.paddingRight = 6;
            style.paddingTop = 4;
            style.paddingBottom = 4;

            Add(orderLabel);
            ApplyBaseStyle();
            AddIcon();
        }

        private void CreatePorts()
        {
            // ROOT HAS NO INPUT
            if (!(node is RootNode))
            {
                input = InstantiatePort(
                    Orientation.Vertical,
                    Direction.Input,
                    Port.Capacity.Single,
                    typeof(bool)
                );

                input.portName = "In";
                inputContainer.Add(input);
            }

            // OUTPUT PORT
            output = InstantiatePort(
                Orientation.Vertical,
                Direction.Output,
                node is CompositeNode ? Port.Capacity.Multi : Port.Capacity.Single,
                typeof(bool)
            );

            output.portName = "Out";
            outputContainer.Add(output);
        }
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            node.position = newPos.position;
            UnityEditor.EditorUtility.SetDirty(node);
        }
        public void UpdateState()
        {
            if (!Application.isPlaying || node.runtime == null)
            {
                ResetStyle();
                return;
            }

            // Not executed → inactive
            if (!node.runtime.executedThisFrame)
            {
                ResetStyle();
                return;
            }

            switch (node.runtime.visualState)
            {
                case BTNodeRuntime.BTVisualState.Inactive:
                    ResetStyle();
                    break;

                case BTNodeRuntime.BTVisualState.Running:
                    SetRunningStyle();
                    break;

                case BTNodeRuntime.BTVisualState.Success:
                    SetSuccessStyle();
                    break;
                case BTNodeRuntime.BTVisualState.Failure:
                    SetFailureStyle();
                    break;
            }
        }
        public void UpdateExecutionOrder()
        {
            var label = this.Q<Label>("execution-order");

            if (executionIndex >= 0)
            {
                label.text = executionIndex.ToString();
                label.style.display = DisplayStyle.Flex;
            }
            else
            {
                label.style.display = DisplayStyle.None;
            }
        }

        //Color Methods
        void ResetStyle()
        {
            ApplyBaseStyle();
            style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            style.borderTopWidth = 1;
            style.borderBottomWidth = 1;
            style.borderLeftWidth = 1;
            style.borderRightWidth = 1;
            style.borderTopColor = Color.black;
            style.borderBottomColor = Color.black;
            style.borderLeftColor = Color.black;
            style.borderRightColor = Color.black;
        }
        void SetRunningStyle()
        {
            style.backgroundColor = new Color(1f, 0.85f, 0.2f); // Yellow

            style.borderTopWidth = 3;
            style.borderBottomWidth = 3;
            style.borderLeftWidth = 3;
            style.borderRightWidth = 3;

            style.borderTopColor = Color.yellow;
            style.borderBottomColor = Color.yellow;
            style.borderLeftColor = Color.yellow;
            style.borderRightColor = Color.yellow;
        }
        void SetSuccessStyle()
        {
            style.backgroundColor = new Color(0.3f, 0.8f, 0.3f);

            style.borderTopWidth = 2;
            style.borderBottomWidth = 2;
            style.borderLeftWidth = 2;
            style.borderRightWidth = 2;

            style.borderTopColor = Color.green;
            style.borderBottomColor = Color.green;
            style.borderLeftColor = Color.green;
            style.borderRightColor = Color.green;
        }
        void SetFailureStyle()
        {
            style.backgroundColor = new Color(0.8f, 0.3f, 0.3f);

            style.borderTopWidth = 2;
            style.borderBottomWidth = 2;
            style.borderLeftWidth = 2;
            style.borderRightWidth = 2;

            style.borderTopColor = Color.red;
            style.borderBottomColor = Color.red;
            style.borderLeftColor = Color.red;
            style.borderRightColor = Color.red;
        }
        private void ApplyBaseStyle()
        {
            if (node is RootNode)
                style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);

            else if (node is CompositeNode)
                style.backgroundColor = new Color(0.2f, 0.3f, 0.6f); // Blue

            else if (node is DecoratorNode)
                style.backgroundColor = new Color(0.6f, 0.4f, 0.2f); // Orange

            else
                style.backgroundColor = new Color(0.3f, 0.3f, 0.3f); // Default
        }
        private void AddIcon()
        {
            var icon = new Image();

            icon.style.width = 16;
            icon.style.height = 16;
            icon.style.marginRight = 4;

            if (node is SequenceNode)
                icon.image = EditorGUIUtility.IconContent("d_PlayButton").image;

            else if (node is SelectorNode)
                icon.image = EditorGUIUtility.IconContent("d_FilterByType").image;

            else if (node is ConditionNode)
                icon.image = EditorGUIUtility.IconContent("d_TestPassed").image;

            else if (node is DebugLogNode)
                icon.image = EditorGUIUtility.IconContent("d_console.infoicon").image;

            else if (node is MoveForwardNode)
                icon.image = EditorGUIUtility.IconContent("d_Transform Icon").image;

            else
                icon.image = EditorGUIUtility.IconContent("d_NodeIcon").image;

            titleContainer.Insert(0, icon);
        }
    }
}