using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DarkPixelGD.BehaviourTree
{
    public enum ConditionValueType { Bool, Float, Int }

    public enum CompareType { Equal, NotEqual, Greater, Less, GreaterOrEqual, LessOrEqual }

    public class ConditionNode : DecoratorNode
    {
        public string key = "isChasing";

        public ConditionValueType valueType = ConditionValueType.Bool;
        public CompareType compareType = CompareType.Equal;

        public bool boolValue = true;
        public float floatValue = 0f;
        public int intValue = 0;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new ConditionRuntime(this, bb, owner);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            var keyField = new TextField("Blackboard Key") { value = key };
            keyField.RegisterValueChangedCallback(evt => { key = evt.newValue; EditorUtility.SetDirty(this); });

            var typeField = new EnumField("Value Type", valueType);
            var compareField = new EnumField("Compare", compareType);

            var boolField = new Toggle("Expected Value") { value = boolValue };
            var floatField = new FloatField("Expected Value") { value = floatValue };
            var intField = new IntegerField("Expected Value") { value = intValue };

            // Helper to hide/show fields based on the selected type
            void UpdateVisibility()
            {
                compareField.style.display = (valueType == ConditionValueType.Bool) ? DisplayStyle.None : DisplayStyle.Flex;
                boolField.style.display = (valueType == ConditionValueType.Bool) ? DisplayStyle.Flex : DisplayStyle.None;
                floatField.style.display = (valueType == ConditionValueType.Float) ? DisplayStyle.Flex : DisplayStyle.None;
                intField.style.display = (valueType == ConditionValueType.Int) ? DisplayStyle.Flex : DisplayStyle.None;
            }

            typeField.RegisterValueChangedCallback(evt => {
                valueType = (ConditionValueType)evt.newValue;
                UpdateVisibility();
                EditorUtility.SetDirty(this);
            });
            compareField.RegisterValueChangedCallback(evt => { compareType = (CompareType)evt.newValue; EditorUtility.SetDirty(this); });
            boolField.RegisterValueChangedCallback(evt => { boolValue = evt.newValue; EditorUtility.SetDirty(this); });
            floatField.RegisterValueChangedCallback(evt => { floatValue = evt.newValue; EditorUtility.SetDirty(this); });
            intField.RegisterValueChangedCallback(evt => { intValue = evt.newValue; EditorUtility.SetDirty(this); });

            container.Add(keyField);
            container.Add(typeField);
            container.Add(compareField);
            container.Add(boolField);
            container.Add(floatField);
            container.Add(intField);

            UpdateVisibility(); // Set initial state
            return container;
        }
    }

    public class ConditionRuntime : BTNodeRuntime
    {
        private ConditionNode node;
        public BTNodeRuntime child; // Set by BehaviorTree during BuildNode()

        public ConditionRuntime(ConditionNode node, Blackboard bb, GameObject owner) : base(bb, owner)
        {
            this.node = node;
        }

        public void SetChild(BTNodeRuntime childRuntime)
        {
            this.child = childRuntime;
        }

        protected override BTStatus OnUpdate()
        {
            // 1. Check the condition first
            if (!Evaluate())
            {
                return BTStatus.Failure;
            }

            // 2. If true, execute the child
            if (child != null)
            {
                return child.Tick();
            }

            // 3. If true but no child exists, succeed safely
            return BTStatus.Success;
        }

        private bool Evaluate()
        {
            // If the key doesn't exist on the blackboard, default to false
            if (!blackboard.TryGetValue(node.key, out object value))
                return false;

            if (node.valueType == ConditionValueType.Bool && value is bool b)
                return b == node.boolValue;

            if (node.valueType == ConditionValueType.Float && value is float f)
                return Compare(f, node.floatValue);

            if (node.valueType == ConditionValueType.Int && value is int i)
                return Compare(i, node.intValue);

            return false;
        }

        private bool Compare(float a, float b)
        {
            switch (node.compareType)
            {
                case CompareType.Equal: return Mathf.Approximately(a, b);
                case CompareType.NotEqual: return !Mathf.Approximately(a, b);
                case CompareType.Greater: return a > b;
                case CompareType.Less: return a < b;
                case CompareType.GreaterOrEqual: return a >= b;
                case CompareType.LessOrEqual: return a <= b;
            }
            return false;
        }
    }
}