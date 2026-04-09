using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace DarkPixelGD.BehaviourTree
{



    public class DebugLogNode : BTNode
    {
        public string message;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new DebugLogRuntime(bb, owner, message);
        }
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            var field = new TextField("Message");
            field.value = message;
            field.RegisterValueChangedCallback(evt =>
            {
                message = evt.newValue;
                EditorUtility.SetDirty(this);
            });

            container.Add(field);

            return container;
        }
    }

    public class DebugLogRuntime : BTNodeRuntime
    {
        private string message;

        public DebugLogRuntime(Blackboard bb, GameObject owner, string message)
            : base(bb, owner)
        {
            this.message = message;
        }

        protected override BTStatus OnUpdate()
        {
            Debug.Log(message);
            return BTStatus.Success;
        }

    }
}